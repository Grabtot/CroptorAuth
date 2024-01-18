using Croptor.Infrastructure.Persistence.Repositories;
using CroptorAuth.Data;
using CroptorAuth.Models;
using CroptorAuth.Options;
using CroptorAuth.Services;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;
using System.Net.Mail;

namespace CroptorAuth
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddRazorPages();

            string connectionString = builder.Configuration.GetConnectionString("Postgres")
                ?? throw new Exception("Postgres connection string doesnt provided");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString, options =>
                    options.EnableRetryOnFailure());
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ абвгґдеєжзиіїйклмнопрстуфхцчшщьюяАБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                // .AddProfileService<CroptorProfileService>()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddAspNetIdentity<ApplicationUser>();

            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                });
            //.AddFacebook(options =>
            //{
            //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

            //    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
            //    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];

            //});

            EmailOptions emailOptions = new();//builder.Configuration.GetSection(EmailOptions.SectionName)
            builder.Configuration.Bind(EmailOptions.SectionName, emailOptions);

            builder.Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(emailOptions));

            builder.Services.AddFluentEmail(emailOptions.EmailAddress, emailOptions.EmailName)
                .AddSmtpSender(new SmtpClient()
                {
                    Host = emailOptions.SmtpHost,
                    Port = emailOptions.SmtpPort,
                    EnableSsl = emailOptions.EnableSsl,
                    Credentials = new NetworkCredential(emailOptions.EmailAddress, emailOptions.EmailPassword)
                });

            builder.Services.AddScoped<IEmailSender<ApplicationUser>, CroptorEmailSender>();
            builder.Services.AddScoped<IEmailSender, CroptorEmailSender>();
            builder.Services.AddScoped<UserProvider>();
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<OrderRepository>();
            builder.Services.AddScoped<WayForPayService>();

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapRazorPages()
                .RequireAuthorization();

            return app;
        }
    }
}