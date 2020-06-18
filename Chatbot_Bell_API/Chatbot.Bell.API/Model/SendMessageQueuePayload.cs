using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class SendMessageQueuePayload
    {
        public string To { get; set; }
        public string TextToReply { get; set; }
        public WhatsAppApiCredentials WhatsAppCredentials { get; set; }

        public string DbConnection { get; set; }
    }
}
