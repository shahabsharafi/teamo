using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Teamo.Assets.Email
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage emailMessage);
    }
}
