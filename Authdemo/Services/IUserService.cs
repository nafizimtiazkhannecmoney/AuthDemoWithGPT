using Authdemo.Enums;
using Authdemo.Models;

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

    }
}
