using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class APIResponse
    {
        public bool isError { get; set; } = false;
        public object Data { get; set; }
    }
}
