using System;
using backend.Models;
using backend.Models.Dtos;

namespace backend.Repository;

public interface IUserRepository
{
    Task<ICollection<UserDataDto>> GetUsers();
    Task<UserDataDto?> GetUser(string id);
    Task<bool> IsUsernameUnique(string username);
    Task<bool> IsEmailUnique(string email);
    Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
    Task<UserDataDto> Register(CreateUserDto userRegisterDto);
    Task<UserLoginResponseDto> CheckStatus(string userId);
    Task<UserLoginResponseDto?> UpdateUser(string userId, UpdateUserDto updateUserDto, bool isAdmin = false);

}
