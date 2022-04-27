using Kavenegar;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Teamo.Assets.SMS
{
    public class KavenegarConfiguration
    {
        public string ServiceName { get; set; } // Kavenegar
        public string SenderNumber { get; set; } // 1000596446
        public string UserApiKey { get; set; } //434661577332716E624F714533566B592F524E7638413D3D
    }


    public class KavenegarService : ISMSService
    {
        private readonly KavenegarConfiguration _kavenegarConfiguration;
        private readonly KavenegarApi _kavenegarApi;
        public KavenegarService(KavenegarConfiguration kavenegarConfiguration)
        {
            _kavenegarConfiguration = kavenegarConfiguration;
            _kavenegarApi = new KavenegarApi(kavenegarConfiguration.UserApiKey);
        }

        public async Task SendAsync(SMS sms)
        {
            try
            {
                await _kavenegarApi.Send(_kavenegarConfiguration.SenderNumber,
                    sms.ReciverNumber.ToList(), sms.Message);
            }
            //log these exceptions 
            catch (Kavenegar.Core.Exceptions.ApiException ex)
            {
                // در صورتی که خروجی وب سرویس 200 نباشد این خطارخ می دهد.
                Console.Write("Message : " + ex.Message);
                throw new Exception("خطایی در سرویس پیامکی رخ داده است");
            }
            catch (Kavenegar.Core.Exceptions.HttpException ex)
            {
                // در زمانی که مشکلی در برقرای ارتباط با وب سرویس وجود داشته باشد این خطا رخ می دهد
                Console.Write("Message : " + ex.Message);
                throw new Exception("خطایی در سرویس پیامکی رخ داده است");
            }
        }
    }
}
