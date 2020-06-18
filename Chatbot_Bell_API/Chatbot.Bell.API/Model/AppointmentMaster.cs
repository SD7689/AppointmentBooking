using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class AppointmentMaster
    {
        public int TenantID { get; set; }
        public string StoreID { get; set; }
        public string ProgramCode { get; set; }
        public string CustomerID { get; set; }
        public string AppointmentDate { get; set; }
        public int SlotID { get; set; }
        public string MobileNo { get; set; }
        public int NOofPeople { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }

    public class AppointmentDetails
    {
        public int AppointmentID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobileNo { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string NoOfPeople { get; set; }
        public string StoreManagerMobile { get; set; }
    }
}
