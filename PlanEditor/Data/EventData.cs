﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.Data
{
    public class EventData
    {
        //[Key]
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AllDay { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int EventState { get; set; }
        public int LabelId { get; set; }
        public int StatusId { get; set; }
        public bool IsPrivate
        {
            get
            {
                return (EventState & 1) != 0;
            }
            set
            {
                EventState = (EventState & -2) | (value ? 1 : 0);
            }
        }
        public EventPriority Priority
        {
            get
            {
                int result = EventState & 6 >> 1;
                if (result == 6)
                    return EventPriority.None;
                return (EventPriority)result;
            }
            set
            {
                EventState = (EventState & -7) | (((int)value) << 1);
            }
        }
        public string Location { get; set; }
        public string ReminderInfo { get; set; }
        public string RecurrenceInfo { get; set; }
        public int EventType { get; set; }
        public string CalendarIds { get; set; }

        public double PercentComplete { get; set; }
        public double Qty { get; set; }
        public string ItemCode { get; set; }
        public double TimeWork { get; set; } = 0;
    }
    public enum EventPriority { None = 0, Important = 1, NotImportant = 2 }
}
