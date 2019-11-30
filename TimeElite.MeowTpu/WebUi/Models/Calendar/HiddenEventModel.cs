using System;
using Core.Enums;

namespace WebUi.Models.Calendar
{
    public class HidableEventModel
    {
        public string ParentItemHash { get; set; } = "";

        public WeekType WeekType { get; set; }

        public DayOfWeek WeekDay { get; set; }

        public byte EventIndex { get; set; }

        public string Place { get; set; } = "";

        public string HiddenEventData => ToString();

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var segments = new[]
            {
                ParentItemHash,
                ((byte) WeekType).ToString(),
                ((byte) WeekDay).ToString(),
                EventIndex.ToString(),
                Place
            };

            return string.Join(':', segments);
        }
    }
}