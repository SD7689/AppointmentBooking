using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class ItemDetails
    {
        public string ProductName { get; set; }
        public string UniqueItemCode { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string Color { get; set; }
        public string ColorCode { get; set; }
        public string BrandName { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string ImageURL { get; set; }
        public string Size { get; set; }
    }
    public class ItemHSN
    {
        public string SKUID { get; set; }
        public string HSNCode { get; set; }
        public string ItemName { get; set; }
    }
    public class ItemDetailsBySkuID
    {
        public string ProductName { get; set; }
        public string UniqueItemCode { get; set; }
        public string Price { get; set; }
        public string ImageURL { get; set; }
        //public string BrandName { get; set; }
    }
}
