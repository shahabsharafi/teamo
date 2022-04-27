using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Teamo.Assets.Email
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection configSection = configuration.GetSection("EmailConfiguration");
            services.AddSingleton<IEmailConfiguration>(configSection.Get<EmailConfiguration>());
            services.AddSingleton<IEmailService, EmailKitService>();
            return services;
        }
    }
}
