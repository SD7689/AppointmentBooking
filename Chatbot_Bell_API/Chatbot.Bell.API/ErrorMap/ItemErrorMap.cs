using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.ErrorMap
{
    public static class ItemErrorMap
    {
        public static IDictionary<string, string> ErrorItemDic;
        static ItemErrorMap()
        {
            ErrorItemDic = new Dictionary<string, string>()
            {
                { "125","Input parameters are not provided correctly." },
                {"225","Program not configured." },
                {"226","Invalid Pincode." },
                {"1001", "Technical error occured. Please try after sometime."} ,
                {"1002", "No Item Code available"},
                 {"1003", "Appointment date is not in correct format."},
                 {"1004", "Store code does not exist."}
            };
        }
    }
}
