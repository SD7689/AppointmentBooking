using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class SMSSendRequest
    {
        public string MobileNumber { get; set; }
        public string SenderId { get; set; }
        public string SMSText { get; set; }
    }

    public class SMSSendResponse
    {
        public string GUID { get; set; }
        public string SubmitDate { get; set; }
        public string ID { get; set; }
        public string ErrorSEQ { get; set; }
        public string ErrorCODE { get; set; }
    }
}
