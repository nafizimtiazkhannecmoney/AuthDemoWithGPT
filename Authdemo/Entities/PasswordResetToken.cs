namespace Authdemo.Entities
{
    public class PasswordResetToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsUsed { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
