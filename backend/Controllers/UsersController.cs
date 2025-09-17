using System.Security.Claims;
using AutoMapper;
using backend.Models.Dtos;
using backend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            return Ok(users);
        }


        [Authorize]
        [HttpGet("{id}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userRepository.GetUser(id);
            if (user == null)
            {
                return NotFound($"El usuario con el id {id} no existe");
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost(Name = "RegisterUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
        {
            //Validaciones
            if (createUserDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _userRepository.IsUsernameUnique(createUserDto.Username))
            {
                ModelState.AddModelError(nameof(createUserDto.Username), "El nombre de usuario ya está en uso");
                return BadRequest(ModelState);
            }

            if (!await _userRepository.IsEmailUnique(createUserDto.Email))
            {
                ModelState.AddModelError(nameof(createUserDto.Email), "El email ya está registrado");
                return BadRequest(ModelState);
            }
            var result = await _userRepository.Register(createUserDto);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al registrar el usuario");
            }
            return CreatedAtRoute("GetUser", new { id = result.Id }, result);

        }

        [AllowAnonymous]
        [HttpPost("Login", Name = "LoginUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLoginDto)
        {
            //Validaciones
            if (userLoginDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userRepository.Login(userLoginDto);
            if (result == null)
            {
                return Unauthorized("Credenciales inválidas");
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("check-status")]
        public async Task<IActionResult> CheckAuthStatus()
        {
            // Obtener usuario actual del contexto
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userResponse = await _userRepository.CheckStatus(userId!);

            if (userResponse == null)
                return Unauthorized();

            return Ok(userResponse);            
        }

        [Authorize]
        [HttpPut("{id}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                  // Validaciones básicas
                if (updateUserDto == null || !ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }      

                // Verificar que al menos un campo viene para actualizar
                if (string.IsNullOrEmpty(updateUserDto.Username) && string.IsNullOrEmpty(updateUserDto.NewPassword))
                {
                    ModelState.AddModelError("", "Debe proporcionar al menos un campo para actualizar");
                    return BadRequest(ModelState);
                }

                // Obtener el usuario actual del token
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Validar lógica de contraseñas
                var isAdmin = User.IsInRole("Admin");
                if (!string.IsNullOrEmpty(updateUserDto.NewPassword))
                {
                    // Si hay nueva password, debe haber confirmación
                    if (string.IsNullOrEmpty(updateUserDto.ConfirmPassword))
                    {
                        ModelState.AddModelError(nameof(updateUserDto.ConfirmPassword), "La confirmación de contraseña es requerida");
                        return BadRequest(ModelState);
                    }

                    // Para usuarios no admin, debe proporcionar la password actual
                    if (currentUserId == id && !isAdmin && string.IsNullOrEmpty(updateUserDto.CurrentPassword))
                    {
                        ModelState.AddModelError(nameof(updateUserDto.CurrentPassword), "La contraseña actual es requerida");
                        return BadRequest(ModelState);
                    }
                }

                // Verificar que el usuario existe
                var existingUser = await _userRepository.GetUser(id);
                if (existingUser == null)
                {
                    return NotFound($"El usuario con el id {id} no existe");
                }        
                
                // Verificar permisos: solo el propio usuario o un admin pueden editar
                if (currentUserId != id && !isAdmin)
                {
                    return Forbid("No tenes permisos para editar este usuario");
                }

                // Validar username único (solo si se está cambiando)
                if (!string.IsNullOrEmpty(updateUserDto.Username) && 
                    updateUserDto.Username != existingUser.UserName &&
                    !await _userRepository.IsUsernameUnique(updateUserDto.Username))
                {
                    ModelState.AddModelError(nameof(updateUserDto.Username), "El nombre de usuario ya está en uso");
                    return BadRequest(ModelState);
                }

                // Actualizar el usuario
                var result = await _userRepository.UpdateUser(id, updateUserDto, isAdmin);
                return Ok(result);
                }
            catch (Exception ex)
            {
                // Manejar errores específicos
                if (ex.Message.Contains("La contraseña actual es incorrecta"))
                {
                    return BadRequest(new { message ="La contraseña actual es incorrecta"});
                }
                else if (ex.Message.Contains("Error al cambiar la contraseña"))
                {
                    return BadRequest(new { message ="Error al cambiar la contraseña"});
                }
                else if (ex.Message.Contains("Usuario no encontrado"))
                {
                    return NotFound(new { message ="Usuario no encontrado"});
                }
                else if (ex.Message.Contains("nueva contraseña debe ser diferente")) 
                {
                    return BadRequest(new { message ="La nueva contraseña debe ser diferente a la actual"});
                }
                
                // Loggear error inesperado

                return StatusCode(500, "Error interno del servidor");
            }
          
        }

    }
}
