using Authdemo.DTO;
using Authdemo.Entities;
using Authdemo.Enums;
using Authdemo.Models;
using Authdemo.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Authdemo.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, PasswordHasher<User> passwordHasher, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _roleRepository = roleRepository;
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return users.Select(user => new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RoleId = user.RoleId,
                Role = user.RoleNavigation!.Name,
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
                RoleId = user.RoleId,
                Role = user.RoleNavigation!.Name,
                Department = user.Department,
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted
            };
        }

        public async Task<UpdateUserResult> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return UpdateUserResult.NotFound;
            }

            if (user.IsDeleted)
            {
                return UpdateUserResult.Deleted;
            }

            if (await _userRepository.UsernameExistsAsync(request.Username, id))
            {
                return UpdateUserResult.UsernameExists;
            }

            if (await _userRepository.EmailExistsAsync(request.Email, id))
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

            if (user == null)
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

            if (user == null || user.IsDeleted || !user.IsActive)
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

        public async Task<(CreateUserResult Result, UserResponseDto? User)> CreateUserAsync(CreateUserRequest request)
        {
            var role = await _roleRepository.GetActiveRoleByIdAsync(request.RoleId);

            if (role == null)
            {
                return (CreateUserResult.InvalidRole, null);
            }

            if (await _userRepository.UsernameExistsAsync(request.Username, 0))
            {
                return (CreateUserResult.UsernameExists, null);
            }

            if (await _userRepository.EmailExistsAsync(request.Email, 0))
            {
                return (CreateUserResult.EmailExists, null);
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                RoleId = request.RoleId,
                Department = request.Department,
                IsActive = true,
                IsDeleted = false,
                TokenVersion = 1
            };
            

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _userRepository.CreateUserAsync(user);

            return (CreateUserResult.Success, new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RoleId = user.RoleId,
                Role = role.Name,
                Department = user.Department,
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted
            });

        }

        public async Task<ChangeUserRoleResult> ChangeUserRoleAsync(int id, ChangeUserRoleRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return ChangeUserRoleResult.NotFound;
            }

            if (user.IsDeleted)
            {
                return ChangeUserRoleResult.Deleted;
            }

            var role = await _roleRepository.GetActiveRoleByIdAsync(request.RoleId);

            if (role == null)
            {
                return ChangeUserRoleResult.InvalidRole;
            }

            if (user.RoleId == request.RoleId)
            {
                return ChangeUserRoleResult.Success;
            }

            user.RoleId = request.RoleId;
            user.TokenVersion++;

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.RevokeAllRefreshTokensAsync(user.Id);

            return ChangeUserRoleResult.Success;
        }

        public async Task<string?> GetRoleNameByIdAsync(int roleId)
        {
            var role = await _roleRepository.GetActiveRoleByIdAsync(roleId);
            return role?.Name;
            //return role?.Name;
        }
    }
}
