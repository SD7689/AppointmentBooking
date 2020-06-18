using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class Search
    {
    }
    public class StoreSearch
    {
        [Required]
        public string programcode { get; set; }
        [Required]
        public string latitude { get; set; }
        [Required]
        public string longitude { get; set; }
        [Required]
        public string distance { get; set; }
    }
    public class StoreSearchByPin
    {
        [Required]
        public string programcode { get; set; }
        [Required]
        public string Pincode { get; set; }
    }

    public class ItemSearch
    {
        [Required]
        public string programcode { get; set; }
        [Required]
        public string SearchCriteria { get; set; }

    }
    public class StoreSearchByStoreCode
    {
        [Required]
        public string programcode { get; set; }
        [Required]
        public string StoreCode { get; set; }
    }
    public class ProgramConfigSearch
    {
        [Required]
        public string Programcode { get; set; }
    }
}
