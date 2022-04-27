using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Teamo.Assets.SMS
{
    public class SMSIRConfiguration
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SenderNumber { get; set; }
        public string UserApiKey { get; set; }
        public string SecretKey { get; set; }
    }


    public class SMSIRService : ISMSService
    {
        private readonly SMSIRConfiguration _smsConfiguration;

        public SMSIRService(SMSIRConfiguration smsConfiguration)
        {
            _smsConfiguration = smsConfiguration;
        }

        //ایده-پردازان
        public async Task SendAsync(SMS sms)
        {
            string r = @"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$";
            
            if (sms.ReciverNumber == null || sms.ReciverNumber.Length < 1 || sms.ReciverNumber.ToList().Any(o => !Regex.Match(o, r).Success))
                throw new Exception("correct receiver numbers is requaire");
            if (!Regex.Match(sms.SenderNumber, r).Success)
                throw new Exception("correct sender numbers is requaire");
            if (string.IsNullOrWhiteSpace(_smsConfiguration.UserName))
                throw new Exception("username is requaire");
            if (string.IsNullOrWhiteSpace(_smsConfiguration.Password))
                throw new Exception("password is requaire");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            var tokenRequestBody = new
            {
                UserApiKey = _smsConfiguration.UserApiKey,
                SecretKey = _smsConfiguration.SecretKey
            };
            string textOfTokenRequestBody = JsonConvert.SerializeObject(tokenRequestBody);
            var contentOfTokenRequestBody = new StringContent(textOfTokenRequestBody, Encoding.UTF8, "application/json");
            var requestOfTokenRequest = await client.PostAsync(@"http://ws.sms.ir/api/Token", contentOfTokenRequestBody);
            if (requestOfTokenRequest.IsSuccessStatusCode)
            {
                var responseOfTokenRequest = await requestOfTokenRequest.Content.ReadAsStringAsync();
                var objOfTokenResponce = JsonConvert.DeserializeAnonymousType(responseOfTokenRequest, new
                {
                    IsSuccessful = true,
                    Message = "",
                    TokenKey = ""
                });
                if (objOfTokenResponce.IsSuccessful)
                {
                    client.DefaultRequestHeaders.Add("x-sms-ir-secure-token", objOfTokenResponce.TokenKey);
                    var smsRequestBody = new
                    {
                        Messages = sms.Message,
                        MobileNumbers = sms.ReciverNumber,
                        LineNumber = _smsConfiguration.SenderNumber,
                        SendDateTime = "",
                        CanContinueInCaseOfError = false,
                    };
                    string textOfSmsRequestBody = JsonConvert.SerializeObject(smsRequestBody);
                    var contentOfSmsRequestBody = new StringContent(textOfSmsRequestBody, Encoding.UTF8, "application/json");
                    var requestOfSmsRequest = await client.PostAsync(@"http://ws.sms.ir/api/MessageSend", contentOfSmsRequestBody);
                    if (requestOfSmsRequest.IsSuccessStatusCode)
                    {
                        var responseOfSmsRequest = await requestOfSmsRequest.Content.ReadAsStringAsync();
                        var objOfSmsResponse = JsonConvert.DeserializeAnonymousType(responseOfSmsRequest, new
                        {
                            Ids = new[]{
                                new {
                                    ID = 1,
                                    MobileNo = ""
                                }
                            },
                            BatchKey = "",
                            IsSuccessful = true,
                            Message = ""
                        });
                        if (!objOfSmsResponse.IsSuccessful)
                        {

                        }
                    }
                }
            }
        }
    }
}
