using System;
using System.Collections.Generic;
using System.Text;

namespace Teamo.Assets.Email
{
    public class ServiceConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
    }
}
