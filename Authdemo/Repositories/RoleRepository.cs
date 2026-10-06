using Authdemo.Data;
using Authdemo.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authdemo.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Role?> GetActiveRoleByIdAsync(int id)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => 
                r.Id == id && 
                r.IsActive);
        }
    }
}
