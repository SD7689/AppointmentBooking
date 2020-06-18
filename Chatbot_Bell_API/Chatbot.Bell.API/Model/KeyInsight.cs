using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class KeyInsight
    {
        public string InsightID { get; set; }
        public string InsightMessage { get; set; }
        //public string MobileNumber { get; set; }

    }
    public class KeyRawInsight
    {
        public string InsightID { get; set; }
        public string TagsDict { get; set; }
        public string Message { get; set; }
        public string MobileNumber { get; set; }
    }
}
