using CroptorAuth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CroptorAuth.Data
{
    public class UsersConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.OwnsOne("plan", user => user.Plan, planBuilder =>
            {
                planBuilder.Property(plan => plan.Type)
                .HasDefaultValue(PlanType.Free);
            });
        }
    }
}
