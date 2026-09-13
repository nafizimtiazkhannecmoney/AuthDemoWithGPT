using Authdemo.Entities;

namespace Authdemo.Services
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);

        Task<User?> GetByUsernameAsync(string username);

        Task CreateAsync(User user);

        Task AddUserAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(int id);

        Task<bool> UsernameExistsAsync(string username);

        Task<bool> EmailExistsAsync(string email);

        // Refresh Token
        Task AddRefreshTokenAsync(RefreshToken refreshToken);

        Task<RefreshToken?> GetRefreshTokenAsync(string token);

        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task<User?> GetByEmailAsync(string email);

        Task AddPasswordResetTokenAsync(PasswordResetToken token);
        Task<PasswordResetToken?>GetPasswordResetTokenAsync(string token);
        Task UpdatePasswordResetTokenAsync(PasswordResetToken token);
        Task UpdateUserAsync(User user);
        Task<List<User>> GetAllUsersAsync();
        Task RevokeAllRefreshTokensAsync(int userId);
    }
}
