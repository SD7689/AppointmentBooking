using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class Store
    {
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public string StoreStatus { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Region { get; set; }
        public string Zone { get; set; }
        public string Country { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string StoreTiming { get; set; }
        public string PinCode { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string CurrentStatus { get; set; }
    }

    public class StoreInfo
    {
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public string StoreStatus { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Region { get; set; }
        public string Zone { get; set; }
        public string Country { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string StoreTiming { get; set; }
        public string PinCode { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string StoreGSTNo { get; set; }
        public string StoreCIN { get; set; }
        public string StorePan { get; set; }
    }

    public class StoreCodeDetails
    {
        public string StoreCode { get; set; }
        public string StoreMessage { get; set; }
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Address { get; set; }
        public string Pincode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
