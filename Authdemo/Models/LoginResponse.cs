namespace Authdemo.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime? ExpiresAt { get; set; }
        public string? ExpiresAtString { get; set; }


        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
