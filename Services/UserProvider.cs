using IdentityModel;
using System.Security.Claims;

namespace CroptorAuth.Services
{
    public class UserProvider(IHttpContextAccessor contextAccessor)
    {
        private readonly ClaimsPrincipal? _user = contextAccessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                string? userIdClaim = _user?.FindFirstValue(JwtClaimTypes.Subject)
                    ?? _user?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userIdClaim))
                    return Guid.Parse(userIdClaim);
                else
                    return null;
            }
        }

        public string? UserName
        {
            get
            {
                if (_user is null)
                    return null;

                string? userNameClaim = _user.FindFirstValue(JwtClaimTypes.Name) ??
                    _user.FindFirstValue(ClaimTypes.Name);

                if (!string.IsNullOrEmpty(userNameClaim))
                    return userNameClaim;
                else
                    return null;
            }
        }

    }
}
