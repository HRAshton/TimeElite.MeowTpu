using System;
using System.Drawing;

namespace WebUi.Models.Calendar
{
    /// <summary>
    ///     Вью-модель события календаря.
    /// </summary>
    public class CalendarEventModel
    {
        /// <summary>
        ///     Дата.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Имя.
        /// </summary>
        public string Name { get; set; } = "[ не распознано ]";

        /// <summary>
        ///     Цвет.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     Место.
        /// </summary>
        public string Place { get; set; } = string.Empty;

        /// <summary>
        ///     Преподаватель.
        /// </summary>
        public string Teacher { get; set; } = string.Empty;

        /// <summary>
        ///     Цвет в формате Hex.
        /// </summary>
        public string ColorHex => $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}";

        /// <summary>
        ///     Прошло ли.
        /// </summary>
        public bool IsOutdated => Date < DateTime.Now;

        /// <summary>
        ///     Общая информация.
        /// </summary>
        public (string, string)[] Summary = new (string, string)[0];

        /// <summary>
        ///     Тип занятия.
        /// </summary>
        public string Type { get; set; } = string.Empty;
    }
}