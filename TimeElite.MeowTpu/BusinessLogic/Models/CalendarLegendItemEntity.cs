using System.Drawing;

namespace BusinessLogic.Models
{
    /// <summary>
    ///     Сущность элемента легенды.
    /// </summary>
    public class CalendarLegendItemEntity
    {
        /// <summary>
        ///     Цвет.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     Название календаря.
        /// </summary>
        public string Name { get; set; } = "[не распознано]";
    }
}