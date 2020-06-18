using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class ShoopingBagAPIRequestcs
    {
    }
    public class SRAuthRequest
    {
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
