using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class SendTextMessageDetails
    {
        public string To { get; set; }
        public string TextToReply { get; set; }
        public string ProgramCode { get; set; }
    }
}
