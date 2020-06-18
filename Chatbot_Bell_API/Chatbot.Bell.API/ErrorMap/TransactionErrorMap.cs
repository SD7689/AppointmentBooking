using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.ErrorMap
{
    public static class TransactionErrorMap
    {
        public static IDictionary<string, string> ErrorTransactionDic;
        static TransactionErrorMap()
        {
            ErrorTransactionDic = new Dictionary<string, string>()
            {
                { "125","Input parameters are not provided correctly." },
                {"225","Program not configured." },
                {"226","Invalid input parameter." }, 
                {"1001", "Technical error occured. Please try after sometime."} ,
                {"1002", "No transaction exist."}
            };
        }
    }
}
