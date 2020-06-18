using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class AppointmentRequest
    {
        public string MobileNumber { get; set; }

        public string ProgramCode { get; set; }

        public string StoreCode { get; set; }
    }
}
