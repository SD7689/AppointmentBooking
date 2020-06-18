using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class QueueModel
    {
        public string ApplicationId { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string TenantID { get; set; }
        public string UserID { get; set; }
        public string Exceptions { get; set; }
        public string MessageException { get; set; }
        public string IPAddress { get; set; }
    }
}
