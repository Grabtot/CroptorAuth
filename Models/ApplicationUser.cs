using Microsoft.AspNetCore.Identity;

namespace CroptorAuth.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Plan Plan { get; set; } = Plan.Create(PlanType.Free);
    }
}
