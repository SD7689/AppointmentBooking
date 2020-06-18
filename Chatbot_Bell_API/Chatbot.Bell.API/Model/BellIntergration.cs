using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class UserInfoRequest
    {
        [Required] 
        public string MobileNumber { get; set; }
        [Required] 
        public string ProgramCode { get; set; }
    }
}
