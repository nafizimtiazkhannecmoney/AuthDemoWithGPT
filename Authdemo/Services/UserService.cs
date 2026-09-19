using Authdemo.Enums;
using Authdemo.Models;

namespace Authdemo.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return users.Select(user => new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Department = user.Department,
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted
            }).ToList();
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Department = user.Department,
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted
            };
        }

        public async Task<UpdateUserResult> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if(user == null)
            {
                return UpdateUserResult.NotFound;
            }

            if(user.IsDeleted)
            {
                return UpdateUserResult.Deleted;
            }

            if(await _userRepository.UsernameExistsAsync(request.Username, id))
            {
                return UpdateUserResult.UsernameExists;
            }

            if(await _userRepository.EmailExistsAsync(request.Email, id))
            {
                return UpdateUserResult.EmailExists;
            }

            user.Username = request.Username;
            user.Email = request.Email;
            user.Department = request.Department;

            await _userRepository.UpdateUserAsync(user);
            return UpdateUserResult.Success;
        }
        public async Task<DeleteUserResult> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if(user == null)
            {
                return DeleteUserResult.NotFound;
            }

            if (user.IsDeleted)
            {
                return DeleteUserResult.AlreadyDeleted;
            }

            user.IsDeleted = true;
            user.TokenVersion++; // Increment the token version to invalidate existing tokens

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.RevokeAllRefreshTokensAsync(id);

            return DeleteUserResult.Success;
        }

        public async Task<bool> DeactivateUserAsync(int id) 
        {
            var user = await _userRepository.GetByIdAsync(id);

            if(user == null || user.IsDeleted || !user.IsActive)
            {
                return false;
            }

            user.IsActive = false;
            user.TokenVersion++; // Increment the token version to invalidate existing tokens

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.RevokeAllRefreshTokensAsync(user.Id);

            return true;
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null || user.IsDeleted || user.IsActive)
            {
                return false;
            }

            user.IsActive = true;
            user.TokenVersion++;

            await _userRepository.UpdateUserAsync(user);
            return true;
        }
    }
}
