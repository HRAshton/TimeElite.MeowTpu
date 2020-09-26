using System;
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
		/// Получить день недели.
		/// </summary>
		/// <param name="dt">Дата.</param>
		/// <returns>День недели.</returns>
		public static WeekType GetWeekType(this DateTime dt)
		{
			var numberOfWeek = GetAcademicYearNumberType(dt);
			var weekType = (WeekType) (numberOfWeek % 2);

			return weekType;
		}

		/// <summary>
		/// 	Получить индекс пары (номер от 1 до 7).
		/// </summary>
		/// <param name="dt">Время начала пары.</param>
		/// <returns>Номер пары (с 1).</returns>
		public static byte GetLessonIndex(this DateTime dt)
		{
			var index = (dt.Hour - 8) / 2 + 1;

			if (index == 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			return (byte) index;
		}

		/// <summary>
		/// 	Получить номер академического года (с 1 сентября).
		/// </summary>
		/// <param name="dt">Дата.</param>
		/// <returns>Академический год.</returns>
		private static ushort GetAcademicYearNumberType(this DateTime dt)
		{
			var begin = new DateTime(DateTime.Now.Year, 9, 1);
			var numberOfWeek = (dt - begin).TotalDays / 7;

			return (ushort) numberOfWeek;
		}
	}
}