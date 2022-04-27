using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Teamo.Assets.SMS
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSMS(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection configSection = configuration.GetSection("SMSConfiguration");
            var smsConfig = configSection.Get<SMSConfiguration>();
            if (smsConfig.ServiceName == "SMSIR")
            {
                var smsIRConfig = configSection.Get<SMSIRConfiguration>();
                services.AddSingleton<ISMSService>(new SMSIRService(smsIRConfig));
            }
            if (smsConfig.ServiceName == "Kavenegar")
            {
                var kavenegarConfig = configSection.Get<KavenegarConfiguration>();
                services.AddSingleton<ISMSService>(new KavenegarService(kavenegarConfig));
            } 
            return services;
        }
    }
}
