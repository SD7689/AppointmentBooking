using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model.DigitalReceipt
{
    public class DigitalReceiptError
    {
        public string Request { get; set; }
        public string TransactionID { get; set; }
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public string StoreCode { get; set; }
        public string ProgCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDiscription { get; set; }
        public int RepeatCount { get; set; } = 0;
    }

    public class DigitalReceiptErrorWithKey
    {

        public DigitalReceiptError ErrorDetails { get; set; }
        public IEnumerable<Claim> claims { get; set; }
    }
}
