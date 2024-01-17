using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CroptorAuth.Pages.Logout
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class LoggedOut : PageModel
    {
        private readonly IIdentityServerInteractionService _interactionService;

        public LoggedOutViewModel View { get; set; }

        public LoggedOut(IIdentityServerInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        public async Task OnGet(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            Duende.IdentityServer.Models.LogoutRequest logout = await _interactionService.GetLogoutContextAsync(logoutId);

            //  return Redirect(logout.PostLogoutRedirectUri ?? "http://localhost:3000");

            View = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = LogoutOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri ?? "http://localhost:3000",
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };
        }
    }
}