using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLogic.Extensions;
using BusinessLogic.Queries.GetCalendarQuery.InternalModels;
using Ical.Net;
using Microsoft.Extensions.Caching.Memory;
using RaspTpuIcalConverter;

namespace BusinessLogic.Queries.GetCalendarQuery
{
    /// <summary>
    ///     Запрос для получения календаря.
    /// </summary>
    public class GetCalendarQuery : QueryBase<string[], CalendarEntity>
    {
        private readonly RaspTruIcalConverter _raspTruIcalConverter;
        private readonly IMemoryCache _memoryCache;

        private readonly int[] palette =
        {
            0x4B85C0, 0xF6B540, 0xFB9550, 0x9581A0, 0xC4C24B, 
            0x3F67A7, 0x8BC653, 0x69D2E8, 0xE27F2E, 0x6A4B83,
            0xC35178, 0x3F67A7, 0x329F8F, 0x7E8FBE, 0xFFCDA1
        };

        /// <summary>
        ///     Конструктор.
        /// </summary>
        /// <param name="httpClient">Http-клиент.</param>
        /// <param name="memoryCache">TODO</param>
        public GetCalendarQuery(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _raspTruIcalConverter = new RaspTruIcalConverter(httpClient);
        }

        /// <summary>
        ///     Выполнить.
        /// </summary>
        /// <returns>Модель страницы календаря.</returns>
        public override QueryResult<CalendarEntity> Execute(string[] calendarLinks)
        {
            var calendars = GetCalendars(calendarLinks);

            calendars = calendars.Where(x => x.Item1 != null).ToList();

            var calendarMatrix = GetCalendarMatrix(calendars);
            var legend = GetCalendarLegend(calendars);

            var result = GetSuccessfulResult(new CalendarEntity
            {
                Legend = legend,
                Matrix = calendarMatrix
            });

            return result;
        }

        private List<(Calendar, Color)> GetCalendars(IReadOnlyList<string> groupHashes)
        {
            List<Task<(Calendar, Color)>> tuples = new List<Task<(Calendar, Color)>>();
            for (var i = 0; i < groupHashes.Count; i++)
            {
                var hash = groupHashes[i];
                var color = Color.FromArgb(palette[i % palette.Length]);

                var task = Task.Run(() =>
                {
                    var calendar = _memoryCache.GetOrCreate($"{hash}", entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(18); // NOTE: В кэше хранится двухнедельное расписание!
                        var cal = _raspTruIcalConverter.GetByHash(hash, 0, 1);

                        return cal;
                    });

                    return (calendar, color);
                });

                tuples.Add(task);
            }
            
            Task.WaitAll(tuples.ToArray<Task>());
            var result = tuples.Select(task => task.Result).ToList();

            return result;
        }

        private static List<CalendarLegendItemEntity> GetCalendarLegend(IEnumerable<(Calendar, Color)> calendars)
        {
            var result = calendars
                .Select(src => new CalendarLegendItemEntity
                {
                    Name = src.Item1.Name,
                    Color = src.Item2
                })
                .ToList();

            return result;
        }

        private static CalendarDayEntity[,] GetCalendarMatrix(ICollection<(Calendar, Color)> calendars)
        {
            var calendarMatrix = new CalendarDayEntity[2, 7];

            if (!calendars.Any())
            {
                calendars.Add((new Calendar(), Color.Transparent));
            }

            foreach (var (calendar, color) in calendars) 
                AddCalendarEventsToMatrix(calendar, color, ref calendarMatrix);

            return calendarMatrix;
        }

        private static void AddCalendarEventsToMatrix(Calendar cal, Color color,
            ref CalendarDayEntity[,] calendarMatrix)
        {
            var firstMonday = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                firstMonday = firstMonday.AddDays(7);
            }

            for (var deltaDay = 0; deltaDay < 2 * 7; deltaDay++)
            {
                var day = firstMonday.AddDays(deltaDay);
                var eventEntities = cal.Events
                    .Where(x => x.DtStart.Date == day.Date)
                    .Select(x => new CalendarEventEntity
                    {
                        Color = color,
                        Date = x.DtStart.AsSystemLocal,
                        Name = x.Name,
                        Place = x.Location,
                        Type = x.Categories.SingleOrDefault() ?? string.Empty,
                        Teacher = x.Contacts.FirstOrDefault() ?? string.Empty
                    })
                    .OrderBy(x => x.Date)
                    .ToList();


                if (calendarMatrix[deltaDay / 7, deltaDay % 7] == null)
                    calendarMatrix[deltaDay / 7, deltaDay % 7] = new CalendarDayEntity
                    {
                        Date = day,
                        Events = new List<CalendarEventEntity>()
                    };

                calendarMatrix[deltaDay / 7, deltaDay % 7].Events.AddRange(eventEntities);
            }
        }
    }
}