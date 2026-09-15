namespace Authdemo.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string Department { get; set; } = string.Empty;
        public int TokenVersion { get; set; } = 1;
        public ICollection<RefreshToken> RefreshTokens { get; set; }    = new List<RefreshToken>();
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; }   = new List<PasswordResetToken>();
    }
}
