using Authdemo.Entities;

namespace Authdemo.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetActiveRoleByIdAsync(int id);
        Task<Role?> GetActiveRoleByNameAsync(string name);
    }
}
