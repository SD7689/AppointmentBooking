using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.ErrorMap
{
    public class ProgConfigErrorMap
    {
        public static IDictionary<string, string> ErrorProgConfigDic;
        static ProgConfigErrorMap()
        {
            ErrorProgConfigDic = new Dictionary<string, string>()
            {
                { "124","Input parameters cannot be blank." },
                { "125","Input parameters are not provided correctly." },
                {"225","Program not configured." },
                {"1001", "Technical error occured. Please try after sometime."}
            };
        }
    }
}
