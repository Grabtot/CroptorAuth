using CroptorAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CroptorAuth.Pages.Account.Register
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        [BindProperty]
        public InputModel Input { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            ApplicationUser user = new()
            {
                Id = Guid.NewGuid(),
                UserName = Input.Username,
                Email = Input.Email
            };

            await _userManager.CreateAsync(user, Input.Password);

            await _userManager.AddClaimAsync(user, new Claim("plan", "Free"));

            return Redirect("~/Account/Login");
        }


    }
}
