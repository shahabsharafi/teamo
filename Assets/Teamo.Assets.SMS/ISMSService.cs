using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Teamo.Assets.SMS
{
    public interface ISMSService
    {
        Task SendAsync(SMS sms);
    }
}
