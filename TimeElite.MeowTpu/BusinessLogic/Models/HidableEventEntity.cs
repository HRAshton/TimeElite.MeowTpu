using System;
using Core.Enums;

namespace BusinessLogic.Models
{
    public class HidableEventEntity
    {
        public string ParentItemHash { get; set; } = "";

        public WeekType WeekType { get; set; }

        public DayOfWeek WeekDay { get; set; }

        public byte EventIndex { get; set; }

        public string Place { get; set; } = "";
    }
}