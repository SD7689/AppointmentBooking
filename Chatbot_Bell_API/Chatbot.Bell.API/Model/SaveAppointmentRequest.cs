using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class SaveAppointmentRequest
    {
        [Required]
        public DateTime AppointmentDate { get; set; }
        [Required]
        public string MobileNo { get; set; }
        [Required]
        public string ProgCode { get; set; }
        [Required]
        public string StoreCode { get; set; }
        [Required]
        public int SlotID { get; set; }
        [Required]
        public int NOofPeople { get; set; }
    }
}
