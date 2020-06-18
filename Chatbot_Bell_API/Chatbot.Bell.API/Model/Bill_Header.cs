using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class Bill_Header
    {
        public string BillNo { get; set; }
        public string BillAmount { get; set; }
        public string BillDateTime { get; set; }
        public string Quantity { get; set; }
        public string TotalTaxAmount { get; set; }
        public string TotalDiscount { get; set; }
    }
    public class Bill_Item
    {
        public string SKUID { get; set; }
        public string HSNCode { get; set; }
        public string ProductDescription { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }
        public string BillDateTime { get; set; }
        public string UnitPrice { get; set; }
        public string Qty { get; set; }
        public string Discount { get; set; }
        public string Tax { get; set; }
        public string NetPayable { get; set; }
        public string ReferenceBillNo { get; set; }
        public List<Bill_TaxSummary> TaxSummaries { get; set; }
        public string ItemType { get; set; }
    }

    public class Bill_GSTSummary
    {
        public string HSNCode { get; set; }
        public string TaxRateInPer { get; set; }
        public string TaxAmount { get; set; }
        public string CGST { get; set; }
        public string SGST { get; set; }
        public string IGST { get; set; }
    }

    public class Bill_Tender
    {
        public string TenderName { get; set; }
        public string TenderRef { get; set; }
        public string TenderAmount { get; set; }
    }

    public class Bill_TaxSummary
    {
        public string Amount { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Percentage { get; set; }
        public string TaxableAmount { get; set; }
    }
}
