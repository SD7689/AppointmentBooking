using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model
{
    public class DateofSchedule
    {
        public string Id { get; set; }
        public string Day { get; set; }
        public string Dates { get; set; }
        public List<AlreadyScheduleDetail> AlreadyScheduleDetails { get; set; }
    }

    public class AlreadyScheduleDetail
    {
        public int TimeSlotId { get; set; }
        public string AppointmentDate { get; set; }
        public int VisitedCount { get; set; }
        public int MaxCapacity { get; set; }
        public int Remaining { get; set; }
        public int StoreId { get; set; }
        public string TimeSlot { get; set; }
        public bool IsDisabled { get; set; }
    }

}
