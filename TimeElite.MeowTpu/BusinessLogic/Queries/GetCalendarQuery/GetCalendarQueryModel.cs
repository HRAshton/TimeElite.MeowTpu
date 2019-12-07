using BusinessLogic.Models;

namespace BusinessLogic.Queries.GetCalendarQuery
{
    public class GetCalendarQueryModel
    {
        public string[] ItemHashes { get; set; } = new string[0];

        public HidableEventEntity[] HiddenEvents { get; set; } = new HidableEventEntity[0];

        public bool ShowWindows { get; set; }

        public byte CountOfWeeksAfterCurrent { get; set; }
    }
}