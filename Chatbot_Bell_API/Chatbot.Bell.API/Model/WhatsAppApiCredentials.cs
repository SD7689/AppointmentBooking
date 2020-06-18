using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class WhatsAppApiCredentials
    {
        public string BaseUrl { get; set; }

        public string AuthToken { get; set; }

        public string ScenarioKey { get; set; }

        public string Uri { get; set; }

        public string CountryCode { get; set; }
    }

    public class ProgramQueueDetails
    {
        public string RabbitHost { get; set; }

        public string RabbitUserName { get; set; }

        public string RabbitPassword { get; set; }

        public string RabbitPort { get; set; }

        public string ExchangeName { get; set; }

        public string SendMessageQueue { get; set; }

        public string SendImageQueue { get; set; }

        public string SendCampaignQueue { get; set; }

        public string SendCustomerResponseQueue { get; set; }
    }
}
