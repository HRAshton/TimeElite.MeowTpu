﻿using System;
using System.Linq;
using Core.Enums;

namespace Core.Extensions
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static ushort GetAcademicYearNumberType(this DateTime dt)
        {
            var begin = new DateTime(DateTime.Now.Year, 9, 1);
            var numberOfWeek = (dt - begin).TotalDays / 7;

            return (ushort)numberOfWeek;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static WeekType GetWeekType(this DateTime dt)
        {
            var numberOfWeek = GetAcademicYearNumberType(dt);
            var weekType = (WeekType) (numberOfWeek % 2);

            return weekType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static byte GetLessonIndex(this DateTime dt)
        {
            var (index, _) = Dictionaries.LessonIndexesDictionary
                .SingleOrDefault(x => x.Value == (dt.Hour, dt.Minute));

            if (index == 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return index;
        }
    }
}