using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    //public class BillDetails
    //{
    //    public Bill_Header Header { get; set; }
    //    public List<Bill_Item> Items { get; set; }
    //    public Bill_GSTSummary GSTSummary { get; set; }
    //    public List<Bill_Tender> Tenders { get; set; }
    //}
    //public class Bill_Header
    //{
    //    public string BillNo { get; set; }
    //    public string BillAmount { get; set; }
    //    public string BillDateTime { get; set; }
    //    public string Quantity { get; set; }
    //    public string TotalTaxAmount { get; set; }
    //    public string TotalDiscount { get; set; }
    //}
    //public class Bill_Item
    //{
    //    public string SKUID { get; set; }
    //    public string HSNCode { get; set; }
    //    public string ProductDescription { get; set; }
    //    public string Department { get; set; }
    //    public string Category { get; set; }
    //    public string BillDateTime { get; set; }
    //    public string UnitPrice { get; set; }
    //    public string Qty { get; set; }
    //    public string Discount { get; set; }
    //    public string Tax { get; set; }
    //    public string NetPayable { get; set; }
    //    public string ReferenceBillNo { get; set; }
    //    public List<Bill_TaxSummary> TaxSummaries { get; set; }
    //    public string ItemType { get; set; }
    //}
    //public class Bill_GSTSummary
    //{
    //    public string hSNCode { get; set; }
    //    public string taxRateInPer { get; set; }
    //    public string taxAmount { get; set; }
    //    public string cGST { get; set; }
    //    public string sGST { get; set; }
    //    public string iGST { get; set; }
    //}

    //public class Bill_Tender
    //{
    //    public string TenderName { get; set; }
    //    public string TenderRef { get; set; }
    //    public string TenderAmount { get; set; }
    //}

    //public class Bill_TaxSummary
    //{
    //    public string amount { get; set; }
    //    public string code { get; set; }
    //    public string description { get; set; }
    //    public string percentage { get; set; }
    //    public string taxableAmount { get; set; }
    //}

    public class BillIMaster
    {
        public string StoreCode { get; set; }
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public string MobileNo { get; set; }
        public string ProgCode { get; set; }

    }
}
