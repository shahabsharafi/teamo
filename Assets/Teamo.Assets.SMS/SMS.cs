using System;
using System.Collections.Generic;
using System.Text;

namespace Teamo.Assets.SMS
{
    public class SMS
    {
        public string SenderNumber { get; set; }
        public string[] ReciverNumber { get; set; }
        public string Message { get; set; }
    }
}
