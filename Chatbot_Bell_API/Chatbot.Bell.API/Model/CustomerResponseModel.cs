using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class CustomerResponseModel
    {
        public string From { get; set; }

        public string StoreId { get; set; }

        public string Text { get;set; }
    }
}
