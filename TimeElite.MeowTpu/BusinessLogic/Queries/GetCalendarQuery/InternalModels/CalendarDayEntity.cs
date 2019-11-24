using System;
using System.Collections.Generic;

namespace BusinessLogic.Queries.GetCalendarQuery.InternalModels
{
    /// <summary>
    ///     Сущность дня календаря.
    /// </summary>
    public class CalendarDayEntity
    {
        /// <summary>
        ///     Дата.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Список событий.
        /// </summary>
        public List<CalendarEventEntity> Events { get; set; } = new List<CalendarEventEntity>();
    }
}