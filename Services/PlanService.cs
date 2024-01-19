using CroptorAuth.Data;
using CroptorAuth.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CroptorAuth.Services
{
    public class PlanService(ApplicationDbContext context)
    {

        private readonly DbSet<ApplicationUser> _dbSet = context.Set<ApplicationUser>();
        internal async Task UpdateSubscriptionsAsync()
        {
            try
            {
                List<ApplicationUser> users = await _dbSet
                    .Where(user => user.Plan.ExpireDate != null
                        && user.Plan.ExpireDate > DateOnly.FromDateTime(DateTime.Now)
                        && user.Plan.Type == PlanType.Pro)
                    .ToListAsync();

                int count = 0;
                foreach (ApplicationUser user in users)
                {
                    user.Plan = Plan.Create(PlanType.Free);
                    count++;
                }

                _dbSet.UpdateRange(users);

                await context.SaveChangesAsync();

                Log.Information($"User subscriptions updated. Updated {count} users");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fail while updating user subscriptions");
            }
        }

        internal async Task UpdateSubscriptionForUserAsync(ApplicationUser user)
        {
            try
            {
                if (user.Plan.ExpireDate != null && user.Plan.ExpireDate > DateOnly.FromDateTime(DateTime.Now) && user.Plan.Type == PlanType.Pro)
                {
                    user.Plan = Plan.Create(PlanType.Free);

                    _dbSet.Update(user);

                    await context.SaveChangesAsync();

                    Log.Information($"User subscription updated for user {user.Id}");
                }
                else
                {
                    Log.Information($"User subscription not updated for user {user.Id}. Conditions not met.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to update subscription for user {user.Id}");
            }
        }

    }
}
