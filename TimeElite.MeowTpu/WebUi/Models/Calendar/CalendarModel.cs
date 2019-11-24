using System.Collections.Generic;

namespace WebUi.Models.Calendar
{
    /// <summary>
    ///     Модель календаря.
    /// </summary>
    public class CalendarModel
    {
        /// <summary>
        ///     Матрица дней календаря.
        /// </summary>
        public CalendarDayModel[,] Matrix { get; set; }

        /// <summary>
        ///     Легенда.
        /// </summary>
        public List<CalendarLegendItemModel> Legend { get; set; }
    }
}