using System;
using System.Drawing;

namespace BusinessLogic.Queries.GetCalendarQuery.InternalModels
{
    /// <summary>
    ///     Сущность события календаря.
    /// </summary>
    public class CalendarEventEntity
    {
        /// <summary>
        ///     Дата.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Имя.
        /// </summary>
        public string Name { get; set; } = "[не распознано]";

        /// <summary>
        ///     Цвет.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     Место.
        /// </summary>
        public string Place { get; set; } = string.Empty;

        /// <summary>
        ///     Тип занятия.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        ///     Преподаватель.
        /// </summary>
        public string Teacher { get; set; } = string.Empty;
    }
}