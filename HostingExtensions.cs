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
            builder.Services.AddControllers();

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

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ абвгґдеєжзиіїйклмнопрстуфхцчшщьюяёАБВГҐДЕЄЁЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";
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
                .AddJwtBearer(options =>
                {
                    options.Authority = builder.Configuration["Authentication:Jwt:Authority"];
                    options.TokenValidationParameters.ValidateAudience = false;
                })
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

                    options.CallbackPath = "/auth/signin-google";
                });
            //.AddFacebook(options =>
            //{
            //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

            //    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
            //    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];

            //});

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Development", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

                options.AddPolicy("Production", policy =>
                {
                    policy.WithOrigins("https://croptor.com")
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

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

            builder.Services.AddScoped<PlanService>();

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            bool isDevelopment = app.Environment.IsDevelopment();

            if (isDevelopment)
            {
                app.UseDeveloperExceptionPage();
            }

            string corsPolicy = isDevelopment ? "Development" : "Production";
            app.UseCors(corsPolicy);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapRazorPages()
                .RequireAuthorization();

            app.MapControllers();

            return app;
        }
    }
}