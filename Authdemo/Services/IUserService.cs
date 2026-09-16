using Authdemo.Models;

namespace Authdemo.Services
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> ActivateUserAsync(int id);

    }
}
