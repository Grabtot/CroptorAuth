using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> OnGet(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            Duende.IdentityServer.Models.LogoutRequest logout = await _interactionService.GetLogoutContextAsync(logoutId);

            string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            string redirectUrl = "https://croptor.com";
            if (environment is not null && environment == "Development")
            {
                redirectUrl = "http://localhost:3000";
            }

            return Redirect(logout.PostLogoutRedirectUri ?? redirectUrl);

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