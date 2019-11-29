using System;
using System.Collections.Generic;

namespace WebUi.Models.Calendar
{
    /// <summary>
    ///     Модель дня календаря.
    /// </summary>
    public class CalendarDayModel
    {
        /// <summary>
        ///     Дата.
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        ///     Список событий.
        /// </summary>
        public List<CalendarEventModel> Events { get; set; } = new List<CalendarEventModel>();

        /// <summary>
        /// Название дня недели.
        /// </summary>
        public string Weekday => Date.ToString("dddd");

        /// <summary>
        /// Название месяца.
        /// </summary>
        public string Month => Date.ToString("MMMM");

        /// <summary>
        /// День месяца.
        /// </summary>
        public string Day => Date.Day.ToString();

        /// <summary>
        /// Год.
        /// </summary>
        public string Year => Date.Year.ToString();
    }
}