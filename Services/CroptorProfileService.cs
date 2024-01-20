using CroptorAuth.Models;
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Serilog;
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
            Log.Debug("CroptorProfileService invoced");

            ClaimsPrincipal principal = await GetUserClaimsAsync(user);
            ClaimsIdentity id = (ClaimsIdentity)principal.Identity;

            id?.AddClaim(new Claim("plan", user.Plan.Type.ToString()));

            Log.Debug($"user ={id.Claims.Select(claim => $"Type: {claim.Type} Value: {claim.Value}")}");
            Log.Debug($"Adding plan claim, Value: {id.FindFirst("plan")?.Value}");

            context.AddRequestedClaims(principal.Claims);
        }
    }
}

namespace CroptorAuth.Pages { }
