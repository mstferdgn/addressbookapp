using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookBL.EmailSenderProcess
{
    public class EmailMessageModel
    {
        //To
        //From
        //Subject
        //Body

        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
