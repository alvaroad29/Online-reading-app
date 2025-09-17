using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backend.Repository;

public class UserRepository : IUserRepository
{
    ApplicationDbContext _db;
    private string? secretKey;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    private readonly ITokenService _tokenService;
    public UserRepository(
        ApplicationDbContext db,
        IConfiguration configuration,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IMapper mapper,
        ITokenService tokenService
    )
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<UserDataDto?> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDataDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Roles = roles.ToList()
        };

    }

    public async Task<ICollection<UserDataDto>> GetUsers()
    {
        var users = await _db.Users
        .OrderBy(u => u.UserName)
        .ToListAsync();

        var usersDto = new List<UserDataDto>();

        foreach (var user in users)
        {
            // despues agregar config en autoMapper (y reemplazar en todo los lugares donde hago esto)
            var roles = await _userManager.GetRolesAsync(user);
            usersDto.Add(new UserDataDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Roles = roles.ToList()
            });
        }

        return usersDto;
    }

    public async Task<bool> IsUsernameUnique(string username)
    {
        return await _userManager.FindByNameAsync(username) == null;
    }
    public async Task<bool> IsEmailUnique(string email)
    {
        return await _userManager.FindByEmailAsync(email) == null;
    }

    public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.Email))
        {
            return null;
        }
        var user = await _userManager.FindByEmailAsync(userLoginDto.Email);

        if (user == null)
        {
            return null;
        }

        if (userLoginDto.Password == null)
        {
            return null;
        }

        var result = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
        if (!result)
        {
            return null;
        }



        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);
        var userDto = _mapper.Map<UserDataDto>(user);
        userDto.Roles = roles.ToList();


        return new UserLoginResponseDto()
        {
            Token = token,
            User = userDto,
            Message = "Login exitoso"
        };
    }

    public async Task<UserDataDto> Register(CreateUserDto createUserDto)
    {

        if (string.IsNullOrEmpty(createUserDto.DisplayName)) // si no viene es igual al username
        {
            createUserDto.DisplayName = createUserDto.Username;
        }

        var user = new User()
        {
            UserName = createUserDto.Username,
            NormalizedUserName = createUserDto.Username.ToLower(),
            DisplayName = createUserDto.DisplayName,
            Email = createUserDto.Email,
        };

        var result = await _userManager.CreateAsync(user, createUserDto.Password);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var userRole = "User";
        var roleExits = await _roleManager.RoleExistsAsync(userRole);

        if (!roleExits)
        {
            var identityRole = new IdentityRole(userRole);
            await _roleManager.CreateAsync(identityRole);
        }

        await _userManager.AddToRoleAsync(user, userRole);

        var roles = await _userManager.GetRolesAsync(user);
        
        return new UserDataDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Roles = roles.ToList()
        };
    }

    public async Task<UserLoginResponseDto> CheckStatus(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(user!);
        var token = _tokenService.GenerateToken(user!, roles);

        var userDto = _mapper.Map<UserDataDto>(user);
        userDto.Roles = roles.ToList();
        return new UserLoginResponseDto()
        {
            Token = token,
            User = userDto,
            Message = "Status Ok"
        };
    }
    
   public async Task<UserLoginResponseDto?> UpdateUser(string userId, UpdateUserDto updateUserDto, bool isAdmin = false)
{
    try
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new Exception("Usuario no encontrado");

        bool usernameChanged = false;
        var originalUsername = user.UserName;

        // Actualizar username si se proporcionó
        if (!string.IsNullOrEmpty(updateUserDto.Username) && 
            updateUserDto.Username != user.UserName)
        {
            user.UserName = updateUserDto.Username;
            usernameChanged = true;
            
            // Guardar cambios de username inmediatamente
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new Exception("Error al actualizar el username");
        }

        // Actualizar password si se proporcionó
        if (!string.IsNullOrEmpty(updateUserDto.NewPassword))
        {
            var isSamePassword = await _userManager.CheckPasswordAsync(user, updateUserDto.NewPassword);
            if (isSamePassword)
            {
                throw new Exception("La nueva contraseña debe ser diferente a la actual");
            }

            // Verificar contraseña actual para usuarios no admin
            if (!isAdmin && !string.IsNullOrEmpty(updateUserDto.CurrentPassword))
            {
                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, updateUserDto.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    // Revertir cambios de username si falla
                    if (usernameChanged)
                    {
                        user.UserName = originalUsername;
                        await _userManager.UpdateAsync(user);
                    }
                    throw new Exception("La contraseña actual es incorrecta"); // ← Específico
                }
            }

            // Cambiar la contraseña
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, updateUserDto.NewPassword);
            
            if (!result.Succeeded)
            {
                // Revertir cambios de username si falla
                if (usernameChanged)
                {
                    user.UserName = originalUsername;
                    await _userManager.UpdateAsync(user);
                }
                throw new Exception("Error al cambiar la contraseña");
            }
        }

        // Obtener usuario actualizado
        var roles = await _userManager.GetRolesAsync(user);
        var userDto = _mapper.Map<UserDataDto>(user);
        userDto.Roles = roles.ToList();

        string? tokenJwt = null;
        if (usernameChanged)
        {
            tokenJwt = _tokenService.GenerateToken(user, roles);
        }
        
        return new UserLoginResponseDto
        {
            Token = tokenJwt,
            User = userDto,
            Message = "Usuario actualizado"
        };
    }
    catch (Exception ex)
    {
        throw; // Relanzar con mensaje específico
    }
}
}
