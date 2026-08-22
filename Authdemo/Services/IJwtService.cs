using Authdemo.Entities;

namespace Authdemo.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
