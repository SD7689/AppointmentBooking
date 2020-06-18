using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{

    public class ItemImageLocationRequest
    {
        public string ProgramCode { get; set; }
    }

    public class ItemImageLocationResponse
    {
        public string ProgramCode { get; set; }
        public string ImageUrl { get; set; }
    }
}
