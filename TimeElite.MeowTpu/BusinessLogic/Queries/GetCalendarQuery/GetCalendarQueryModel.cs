using System;
using Core.Enums;

namespace BusinessLogic.Queries.GetCalendarQuery
{
    public class GetCalendarQueryModel
    {
        public string[] ItemHashes { get; set; } = new string[0];

        public HiddenEventModel[] HiddenEvents { get; set; } = new HiddenEventModel[0];

        public bool FeelWindows { get; set; }
    }

    public class HiddenEventModel
    {
        public string ParentItemHash { get; set; } = "";

        public WeekType WeekType { get; set; }

        public DayOfWeek WeekDay { get; set; }

        public byte EventIndex { get; set; }

        public string Place { get; set; } = "";
    }
}