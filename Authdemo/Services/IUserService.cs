using Authdemo.Enums;
using Authdemo.Models;
using Authdemo.DTO;

namespace Authdemo.Services
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task<UpdateUserResult> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<DeleteUserResult> DeleteUserAsync(int id);
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> ActivateUserAsync(int id);
        Task<(CreateUserResult Result, UserResponseDto? User)> CreateUserAsync(CreateUserRequest request);
        Task<ChangeUserRoleResult> ChangeUserRoleAsync(int id, ChangeUserRoleRequest request);
        Task<string?> GetRoleNameByIdAsync(int roleId);
    }
}
