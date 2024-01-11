using CroptorAuth.Models;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Serilog;
using System.Net.Mail;

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
            SendResponse response;
            try
            {
                IFluentEmail mail = _email
                    .To(email, user.UserName)
                    .Subject("Email confirmation")
                    .Body($"<div> " +
                    $"<h1>Hi {user.UserName}!</h1>" +
                    $"<p>To confirm your email click <a href=\"{confirmationLink}\">here</a></p>" +
                    $"</div>", true);

                response = await mail.SendAsync();
            }
            catch (SmtpException)
            {
                IFluentEmail mail = _email
                    .To(email, user.UserName)
                    .Subject("Email confirmation")
                    .Body($"<div> " +
                    $"<h1>Hi {user.UserName}!</h1>" +
                    $"<p>To confirm your email click <a href=\"{confirmationLink}\">here</a></p>" +
                    $"</div>", true);

                response = await mail.SendAsync();
            }


            if (response.Successful)
            {
                Log.Information($"Confirmation email send to {email} successful");
            }
            else
            {
                Log.Error($"Cant send confirmation email to {email}! \n Error{response.ErrorMessages.First()}");
            }
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
