using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class SaveAppointmentResponse
    {
        public int AppointmentID { get; set; }
        public string Latitude { get; set; }
        public string longitude { get; set; }
        public string AppointmentDate { get; set; }
        public string slot { get; set; }
        public string MaxPeopleAllowed { get; set; }
    }
    public class MainResponse
    {
        public string message { get; set; }
        public bool status { get; set; }
        public dynamic responseData { get; set; }
        public int statusCode { get; set; }
        
    }
}
