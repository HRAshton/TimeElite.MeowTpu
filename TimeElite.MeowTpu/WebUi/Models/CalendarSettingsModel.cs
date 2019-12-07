using System;
using System.Linq;
using System.Web;
using Core.Enums;
using WebUi.Enums;
using WebUi.Models.Calendar;

namespace WebUi.Models
{
    public class CalendarSettingsModel
    {
        public string[] Items { get; set; } = new string[0];

        public HidableEventModel[] HiddenEvents { get; set; } = new HidableEventModel[0];

        public ViewType ViewType { get; set; } = ViewType.Listed;

        public bool ShowWindows { get; set; }

        public byte CountOfWeeksAfterCurrent { get; set; } = 1;


        /// <summary>Сериализовать.</summary>
        /// <returns>Сериализованная модель.</returns>
        public string Serialize()
        {
            var viewType = Convert.ToByte(ViewType);
            var showWindow = Convert.ToByte(ShowWindows);
            var hiddenEvents = HiddenEvents.Select(x => x.ToString());

            var segments = new[]
            {
                // CountOfWeeksAfterCurrent.ToString(), // не для пользователей, поэтому не идет в url
                viewType.ToString(),
                showWindow.ToString(),
                string.Join(',', Items),
                string.Join(',', hiddenEvents)
            };

            return string.Join(';', segments);
        }

        /// <summary>Десериализовать.</summary>
        /// <param name="data">Сериализованная модель.</param>
        /// <returns>Можедб настроек календаря.</returns>
        public static CalendarSettingsModel Deserialize(string data)
        {
            var blocks = data.Split(";").ToList();

            if (blocks.Count != 4 && blocks.Count != 5) return new CalendarSettingsModel();

            if (blocks.Count == 4)
            {
                blocks.Insert(0, "2");
            }

            var countOfTakenWeeks = Math.Min(Convert.ToByte(blocks[0]), (byte)10);
            var viewType = (ViewType)Convert.ToByte(blocks[1]);
            var feelWindows = Convert.ToByte(blocks[2]) == 1;
            var itemHashes = blocks[3]
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Take(12)
                .ToArray();

            var hiddenEventModels = blocks[4].Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Take(500)
                .Select(x => x.Split(":"))
                .Where(x => x.Length == 5)
                .Select(x => new HidableEventModel
                {
                    ParentItemHash = x[0],
                    WeekType = (WeekType)Convert.ToByte(x[1]),
                    WeekDay = (DayOfWeek)Convert.ToByte(x[2]),
                    EventIndex = Convert.ToByte(x[3]),
                    Place = HttpUtility.UrlDecode(x[4])
                })
                .ToArray();

            var result = new CalendarSettingsModel
            {
                Items = itemHashes,
                HiddenEvents = hiddenEventModels,
                CountOfWeeksAfterCurrent = countOfTakenWeeks,
                ViewType = viewType,
                ShowWindows = feelWindows
            };

            return result;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => Serialize();
    }
}