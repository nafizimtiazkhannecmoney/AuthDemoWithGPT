namespace Authdemo.DTO
{
    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        //public string Role { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Department { get; set; } = string.Empty;
    }
}
