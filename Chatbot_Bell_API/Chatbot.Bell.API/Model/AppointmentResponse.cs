using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class AppointmentResponse
    {
        public string BrandName { get; set; }

        public string BrandLogo { get; set; }

        public string StoreCode { get; set; }

        public string StoreName { get; set; }

        public string StoreAddress { get; set; }

        public string StoreContactDetails { get; set; }

        public List<DateofSchedule> dateofSchedules { get; set; }
        
    }
}
