using CroptorAuth.Models;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Serilog;
using System.Net.Mail;

namespace CroptorAuth.Services
{
    public class CroptorEmailSender : IEmailSender<ApplicationUser>, IEmailSender
    {
        private readonly IFluentEmail _email;

        public CroptorEmailSender(IFluentEmail email)
        {
            _email = email;
        }

        public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            string message = $"<div> " +
                    $"<h1>Hi {user.UserName}!</h1>" +
                    $"<p>To confirm your email click <a href=\"{confirmationLink}\">here</a></p>" +
                    $"</div>";

            await SendEmailAsync(email, "Email confirmation", message);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SendResponse response;
            try
            {
                response = await _email
                   .To(email)
                   .Subject(subject)
                   .Body(htmlMessage, true)
                   .SendAsync();
            }
            catch (SmtpException)
            {
                response = await _email
                 .To(email)
                 .Subject(subject)
                 .Body(htmlMessage, true)
                 .SendAsync();
            }

            if (response.Successful)
            {
                Log.Information($"Email send to {email} successful");
            }
            else
            {
                Log.Error($"Cant send email to {email}! \n Error{response.ErrorMessages.First()}");
            }
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            throw new NotImplementedException();
        }

        public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            string message = $"<p>To reset your password click <a href=\"{resetLink}\">here</a></p>";

            await SendEmailAsync(email, "Reset Password", message);

        }
    }
}
