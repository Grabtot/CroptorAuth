using CroptorAuth.Models;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CroptorAuth.Services
{
    public class HostingerEmailSender : IEmailSender<ApplicationUser>, IEmailSender
    {
        private readonly IFluentEmail _email;

        public HostingerEmailSender(IFluentEmail email)
        {
            _email = email;
        }

        public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {

            await _email
                .To(email, user.UserName)
                .Subject("Email confirmation")
                .Body($"<p>To confirm your email address press <a href=\"{confirmationLink}\"> Confirm email</a></p>")
                .SendAsync();
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await _email
                 .To(email)
                 .Subject(subject)
                 .Body(htmlMessage)
                 .SendAsync();
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            throw new NotImplementedException();
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            throw new NotImplementedException();
        }
    }
}
