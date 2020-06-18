using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class TenantConfig
    {
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public char IsWhatsApp { get; set; }
        public char IsBell { get; set; }
        public char IsWebBot { get; set; }
        public char IsShoppingBag { get; set; }
        public char IsERPayment { get; set; }
        public char IsERShipping { get; set; }
        public char IsOrderSync { get; set; }
        public char IsSendToShipmentManual { get; set; }
        public char IsDigitalReceipt { get; set; }
        public char IsTaxBeforDiscount { get; set; }
    }
}
