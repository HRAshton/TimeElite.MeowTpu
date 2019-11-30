using System;
using System.Drawing;

namespace BusinessLogic.Models
{
    /// <summary>
    ///     Сущность события календаря.
    /// </summary>
    public class CalendarEventEntity
    {
        /// <summary>
        ///     Хэш родительского элемента (пары, преподавателя...).
        /// </summary>
        public string? HashOfParent { get; set; }

        /// <summary>
        ///     Скрыто ли событие пользователем.
        /// </summary>
        public bool IsHiddenByUser { get; set; }

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

        /// <summary>
        /// Является ли окном.
        /// </summary>
        public bool IsWindow { get; set; }
    }
}