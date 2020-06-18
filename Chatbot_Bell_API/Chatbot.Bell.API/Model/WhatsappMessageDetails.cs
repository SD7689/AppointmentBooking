using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class WhatsappMessageDetailsRequest
    {
        public string ProgramCode { get; set; }
    }

    public class WhatsappMessageDetailsResponse
    {
        public string ProgramCode { get; set; }
        public string TemplateName { get; set; }
        public string TemplateNamespace { get; set; }
        public string TemplateText { get; set; }
        public string TemplateLanguage { get; set; }
        public string Remarks { get; set; }
    }
}
