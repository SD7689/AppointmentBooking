using System.Collections.Generic;
using System.Security.Claims;

namespace Chatbot.Bell.API.Model.DigitalReceipt
{
    public class DRCommunicationReq
    {
        public string TransactionID { get; set; }
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public string storecode { get; set; }
        public string MobileNo { get; set; }
        public string CustomerName { get; set; }
        public string ProgCode { get; set; }
        public string BaseURL { get; set; }
        public string bitly { get; set; }
    }

    public class DRCommunicationInfo
    {
        public DRCommunicationReq CommunicationDetail { get; set; }
        public IEnumerable<Claim> claims { get; set; }
    }
}
