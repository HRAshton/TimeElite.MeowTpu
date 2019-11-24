using System.Collections.Generic;

namespace BusinessLogic.Queries.GetCalendarQuery.InternalModels
{
    /// <summary>
    ///     Сущность календаря.
    /// </summary>
    public class CalendarEntity
    {
        /// <summary>
        ///     Матрица дней.
        /// </summary>
        public CalendarDayEntity[,] Matrix { get; set; } = new CalendarDayEntity[0, 0];

        /// <summary>
        ///     Легенда.
        /// </summary>
        public List<CalendarLegendItemEntity> Legend { get; set; } = new List<CalendarLegendItemEntity>();
    }
}