using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLogic.Models;
using Core.Extensions;
using Ical.Net.CalendarComponents;
using Microsoft.Extensions.Caching.Memory;
using RaspTpuIcalConverter;
using RaspTpuIcalConverter.RaspTpuModels;

namespace BusinessLogic.Queries.GetCalendarQuery
{
    /// <summary>
    ///     Запрос для получения календаря.
    /// </summary>
    public class GetCalendarQuery : QueryBase<GetCalendarQueryModel, CalendarEntity>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly RaspTruIcalConverter _raspTruIcalConverter;

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
        /// <param name="memoryCache">Кэш.</param>
        public GetCalendarQuery(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _raspTruIcalConverter = new RaspTruIcalConverter(httpClient, memoryCache, TimeSpan.FromHours(18));
        }

        /// <summary>
        ///     Выполнить.
        /// </summary>
        /// <returns>Модель страницы календаря.</returns>
        public override QueryResult<CalendarEntity> Execute(GetCalendarQueryModel model)
        {
            var calendars = GetCalendars(model.ItemHashes, model.CountOfWeeksAfterCurrent);

            calendars = calendars.Where(x => x.Calendar != null).ToList();

            var calendarMatrix = GetCalendarMatrix(calendars, model.HiddenEvents, model.CountOfWeeksAfterCurrent);
            var legend = GetCalendarLegend(calendars);

            if (model.ShowWindows && calendars.FirstOrDefault()?.Calendar != null)
            {
                FeelWindows(ref calendarMatrix, calendars.First().Calendar!.LessonsTimes);
            }

            var result = GetSuccessfulResult(new CalendarEntity
            {
                Legend = legend,
                Matrix = calendarMatrix,
            });

            return result;
        }

        private static void FeelWindows(ref CalendarDayEntity[,] calendarMatrix,
            IReadOnlyCollection<(byte Hours, byte Minutes)> calendarLessonsTimes)
        {
            foreach (var calendarDayEntity in calendarMatrix)
            {
                var lastLesson = calendarDayEntity.Events
                    .OrderBy(x => x.Date)
                    .LastOrDefault();

                if (lastLesson == null)
                    continue;

                var lastLessonIndex = lastLesson.Date.GetLessonIndex();

                var timesBeforeCurrent = calendarLessonsTimes.Take(lastLessonIndex).ToArray();
                for (var index = 1; index <= timesBeforeCurrent.Length; index++)
                {
                    if (calendarDayEntity.Events.Any(ev => ev.Date.GetLessonIndex() == index))
                        break;

                    var (Hours, Minutes) = timesBeforeCurrent[index - 1];
                    calendarDayEntity.Events.Add(new CalendarEventEntity
                    {
                        Color = Color.Transparent,
                        Date = calendarDayEntity.Date.AddHours(Hours).AddMinutes(Minutes),
                        Name = "",
                        IsWindow = true,
                    });
                }
            }
        }

        private List<HashedCalendar> GetCalendars(IReadOnlyList<string> groupHashes, byte countOfTakenWeeks)
        {
            var tuples = new List<Task<HashedCalendar>>();
            for (var i = 0; i < groupHashes.Count; i++)
            {
                var hash = groupHashes[i];
                var color = Color.FromArgb(palette[i % palette.Length]);

                var task = Task.Run(() =>
                {
                    var calendar = _memoryCache.GetOrCreate($"{hash}{countOfTakenWeeks}", entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow =
                            TimeSpan.FromHours(18);
                        CalendarWithTimesModel? cal = null;
                        try
                        {
                            cal = _raspTruIcalConverter.GetByHash(hash, 0, 0, countOfTakenWeeks);
                        }
                        catch (Exception)
                        {
                            // IGNORE
                        }

                        return cal;
                    });
                    var hashedCalendar = new HashedCalendar
                    {
                        Calendar = calendar,
                        Color = color,
                        HashOfParent = hash
                    };

                    return hashedCalendar;
                });

                tuples.Add(task);
            }

            Task.WaitAll(tuples.ToArray<Task>());
            var result = tuples
                .Select(task => task.Result)
                .Where(r => r != null)
                .ToList();

            return result;
        }

        private static List<CalendarLegendItemEntity> GetCalendarLegend(IEnumerable<HashedCalendar> calendars)
        {
            var result = calendars
                .Where(src => src.Calendar != null)
                .Select(src => new CalendarLegendItemEntity
                {
                    Name = src.Calendar!.Name,
                    Color = src.Color
                })
                .ToList();

            return result;
        }

        private static CalendarDayEntity[,] GetCalendarMatrix(ICollection<HashedCalendar> calendars,
            HidableEventEntity[] hiddenEvents, byte countOfWeeksAfterCurrent)
        {
            var calendarMatrix = new CalendarDayEntity[countOfWeeksAfterCurrent + 1, 7];

            if (!calendars.Any())
                calendars.Add(new HashedCalendar());

            foreach (var cal in calendars)
                AddCalendarEventsToMatrix(cal, hiddenEvents, ref calendarMatrix);

            return calendarMatrix;
        }

        private static void AddCalendarEventsToMatrix(HashedCalendar calendar, HidableEventEntity[] hiddenEvents,
            ref CalendarDayEntity[,] calendarMatrix)
        {
            var firstMonday = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                firstMonday = firstMonday.AddDays(7);

            for (var deltaDay = 0; deltaDay < calendarMatrix.Length; deltaDay++)
            {
                var day = firstMonday.AddDays(deltaDay);
                var eventEntities = calendar.Calendar?.Events
                                        .Where(x => x.DtStart.Date == day.Date)
                                        .Select(x => new CalendarEventEntity
                                        {
                                            Color = calendar.Color,
                                            Date = x.DtStart.Value,
                                            Name = x.Name,
                                            Place = x.Location,
                                            Type = x.Categories.SingleOrDefault() ?? string.Empty,
                                            Teacher = x.Contacts.FirstOrDefault() ?? string.Empty,
                                            HashOfParent = calendar.HashOfParent,
                                            IsHiddenByUser = IsEventHidden(x, calendar.HashOfParent, hiddenEvents)
                                        })
                                        .OrderBy(x => x.Date)
                                        .ToArray() ?? new CalendarEventEntity[0];


                if (calendarMatrix[deltaDay / 7, deltaDay % 7] == null)
                    calendarMatrix[deltaDay / 7, deltaDay % 7] = new CalendarDayEntity
                    {
                        Date = day,
                        Events = new List<CalendarEventEntity>()
                    };

                calendarMatrix[deltaDay / 7, deltaDay % 7].Events.AddRange(eventEntities);
            }
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static bool IsEventHidden(CalendarEvent calendarEvent, string? calendarHashOfParent,
            IEnumerable<HidableEventEntity> hiddenEvents)
        {
            var isHidden = hiddenEvents.Any(ev =>
                ev.ParentItemHash == calendarHashOfParent
                && ev.WeekType == calendarEvent.DtStart.Date.GetWeekType()
                && ev.WeekDay == calendarEvent.DtStart.DayOfWeek
                && ev.EventIndex == calendarEvent.DtStart.Value.GetLessonIndex()
                && ev.Place == calendarEvent.Location);

            return isHidden;
        }

        private class HashedCalendar
        {
            public string? HashOfParent { get; set; }

            public Color Color { get; set; } = Color.Transparent;

            public CalendarWithTimesModel? Calendar { get; set; }
        }
    }
}