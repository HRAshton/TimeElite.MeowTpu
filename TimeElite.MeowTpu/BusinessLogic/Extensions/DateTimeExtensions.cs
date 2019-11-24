using System;

namespace BusinessLogic.Extensions
{
    /// <summary>
    ///     Предоставляет методы для типа <see cref="DateTime" />.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Получить дату начала недели.
        /// </summary>
        /// <param name="dt">Дата.</param>
        /// <param name="startOfWeek">День, с которого начинается неделя.</param>
        /// <returns>Дата дня недели, меньшая или равная заданной.</returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            var result = dt.AddDays(-1 * diff).Date;

            return result;
        }
    }
}