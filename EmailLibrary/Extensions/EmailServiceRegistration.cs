using EmailLibrary.Configuration;
using EmailLibrary.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailLibrary.Extensions
{
    public static class EmailServiceRegistration
    {
        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Step 1: Configure EmailSettings based on the configuration
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Step 2: Retrieve the EmailSettings instance from the configuration
            var settings = configuration.GetSection("EmailSettings").Get<EmailSettings>();

            // Step 3: Set up FluentEmail with the settings
            services
                .AddFluentEmail(settings.FromEmail) // Configure the 'From' email address
                .AddRazorRenderer()                 // Enable Razor templating for email content
                .AddSmtpSender(() =>                // Configure the SMTP client
                {
                    var client = new SmtpClient(settings.SmtpServer)
                    {
                        Port = settings.SmtpPort,
                        EnableSsl = true // or false if your SMTP doesn't require SSL
                    };

                    // If SMTP requires credentials, provide them
                    if (!string.IsNullOrWhiteSpace(settings.Username) && !string.IsNullOrWhiteSpace(settings.Password))
                    {
                        client.Credentials = new NetworkCredential(settings.Username, settings.Password);
                    }

                    return client; // Return the configured SMTP client
                });

            // Step 4: Register email service implementation
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }


}
