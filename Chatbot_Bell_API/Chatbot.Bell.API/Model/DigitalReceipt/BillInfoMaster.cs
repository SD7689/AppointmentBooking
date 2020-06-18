using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model.DigitalReceipt
{   
    public partial class BillInfoMaster
    {
        [JsonProperty("billInfo")]
        public BillInfo BillInfo { get; set; }

        [JsonProperty("items")]
        public Item[] Items { get; set; }

        [JsonProperty("tenderInfo")]

        public TenderInfo[] TenderInfo { get; set; }

       
    }
    public class BillInfoMasterWithKey
    {

        public BillInfoMaster BillInfo { get; set; }
        public IEnumerable<Claim> claims { get; set; }
    }

    public partial class BillInfo
    {
        [JsonProperty("billNumber")]
        public string BillNumber { get; set; }

        [JsonProperty("billAmount")]
        public string BillAmount { get; set; }

        [JsonProperty("storeCode")]
        public string StoreCode { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("billDateTime")]
        public string BillDateTime { get; set; }

        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("program_Code")]
        public string Program_Code { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("skuid")]
        public string Skuid { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("itemType")]
        public string ItemType { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("unitPrice")]
        public string UnitPrice { get; set; }

        [JsonProperty("tax")]
        public string Tax { get; set; }

        [JsonProperty("totalAmount")]
        public string TotalAmount { get; set; }

        [JsonProperty("discount")]
        public string Discount { get; set; }

        [JsonProperty("billedPice")]
        public string BilledPice { get; set; }

        [JsonProperty("taxes")]
        public Tax[] Taxes { get; set; }
    }

    public partial class Tax
    {
        [JsonProperty("componentName")]
        public string ComponentName { get; set; }

        [JsonProperty("taxRate")]
        public string TaxRate { get; set; }
    }

    public partial class TenderInfo
    {
        [JsonProperty("tenderName")]
        public string TenderName { get; set; }

        [JsonProperty("tenderID")]
        public string TenderId { get; set; }

        [JsonProperty("tenderValue")]
        public string TenderValue { get; set; }
    }
}
