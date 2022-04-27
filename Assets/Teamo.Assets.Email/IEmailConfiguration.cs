using System;
using System.Collections.Generic;
using System.Text;

namespace Teamo.Assets.Email
{
    public interface IEmailConfiguration
    {
        ServiceConfiguration Smtp { get; set; }
        string SenderAddress { get; set; }
    }
}
