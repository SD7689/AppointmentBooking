using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class Transaction
    {
        public string MobileNo { get; set; }
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public string StoreName { get; set; }
        public string Amount { get; set; }
        public List<ItemDetail> ItemDetails { get; set; }
    }
    public class ItemDetail
    {
        public string MobileNo { get; set; }
        public string Article { get; set; }
        public string Quantity { get; set; }
        public string Amount { get; set; }
    }
}
