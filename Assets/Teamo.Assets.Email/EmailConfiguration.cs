using System;
using System.Collections.Generic;
using System.Text;

namespace Teamo.Assets.Email
{    
    public class EmailConfiguration: IEmailConfiguration
    {
        public ServiceConfiguration Smtp { get; set; }
        public string SenderAddress { get; set; }
    }
}
