using CroptorAuth.Data;
using CroptorAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace Croptor.Infrastructure.Persistence.Repositories
{
    public class UserRepository(ApplicationDbContext context)
    {
        private readonly DbSet<ApplicationUser> _dbSet = context.Set<ApplicationUser>();

        public async Task<ApplicationUser> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync([userId], cancellationToken)
                   ?? throw new InvalidOperationException($"Could not find User {userId}");
        }
    }
}