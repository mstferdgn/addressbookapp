using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookBL.EmailSenderProcess
{
    public interface IEmailManager
    {
        bool SendEmail(EmailMessageModel model);
        Task SendMailAsync(EmailMessageModel model);


        bool SendEmailviaGmail(EmailMessageModel model);

        //Task SendEmailviaGmailAsync(EmailMessageModel model);

    }
}
