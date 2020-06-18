using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class Recommendation
    {
        public string MobileNumber { get; set; }
        public string ItemCode { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public string size { get; set; }
        public string Price { get; set; }
        public string URL { get; set; }
        public string ImageURL { get; set; }

    }
}
