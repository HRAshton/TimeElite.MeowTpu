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
    }
}