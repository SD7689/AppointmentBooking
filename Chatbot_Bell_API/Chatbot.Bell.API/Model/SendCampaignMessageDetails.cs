﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class SendCampaignMessageDetails
    {
        public string To { get; set; }

        public string ProgramCode { get; set; }

        public string TemplateName { get; set; }

        public List<string> AdditionalInfo { get; set; }
    }
}
