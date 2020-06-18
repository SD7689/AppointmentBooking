using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class ShoppingBag
    {
        public string ProgCode { get; set; }
        public string StoreCode { get; set; }
        public string ShoppingBagNo { get; set; }
        public string Date { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public List<SB_ItemDetails> ItemDetails { get; set; }
        public string Status { get; set; }
        public string DeleveryType { get; set; } = string.Empty;
        public string PickupDateTime { get; set; } = string.Empty;
        public Address address { get; set; }
    }
    public class SB_ItemDetails
    {
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemPrice { get; set; }
        public string Quantity { get; set; }
    }
    public class Address
    {
        public string Flat { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Landmark { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
    }
}
