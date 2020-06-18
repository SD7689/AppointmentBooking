using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.ErrorMap
{
    public static class AppointmentErrorMap
    {
        public static IDictionary<string, string> ErrorAppointmentDic;
        static AppointmentErrorMap()
        {
            ErrorAppointmentDic = new Dictionary<string, string>()
            {
                { "125","Input parameters are not provided correctly." },
                {"225","Program not configured." },
                {"226","Invalid Pincode." },
                {"1001", "Technical error occured. Please try after sometime."} ,
                {"1002", "No nearby stores available"},
                 {"1003", "Appointment date is not in correct format."},
                 {"1004", "Store code does not exist."}
            };
        }
    }
}
