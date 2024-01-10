using CroptorAuth.Models;
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CroptorAuth.Services
{
    public class CroptorProfileService : ProfileService<ApplicationUser>
    {
        public CroptorProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, ILogger<ProfileService<ApplicationUser>> logger) : base(userManager, claimsFactory, logger)
        {
        }

        protected override async Task GetProfileDataAsync(ProfileDataRequestContext context, ApplicationUser user)
        {
            ClaimsPrincipal principal = await GetUserClaimsAsync(user);
            ClaimsIdentity id = (ClaimsIdentity)principal.Identity;

            id.AddClaim(new Claim("plan", user.Plan.Type.ToString()));

            context.AddRequestedClaims(principal.Claims);
        }
    }
}

namespace CroptorAuth.Pages { }
