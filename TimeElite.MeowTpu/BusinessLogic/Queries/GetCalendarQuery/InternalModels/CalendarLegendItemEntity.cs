using System.Drawing;

namespace BusinessLogic.Queries.GetCalendarQuery.InternalModels
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