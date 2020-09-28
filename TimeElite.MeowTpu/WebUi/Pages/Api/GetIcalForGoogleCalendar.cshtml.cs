using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BusinessLogic.Models;
using BusinessLogic.Queries.GetCalendarQuery;
using Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Internal;
using WebUi.Models;
using WebUi.Models.Calendar;

namespace WebUi.Pages.Api
{
    public class GetIcalForGoogleCalendar : PageModel
    {
        /// <summary>Настройки календаря.</summary>
        public CalendarSettingsModel? SettingsModel { get; set; }


        /// <summary>
        ///     Конструтор.
        /// </summary>
        /// <param name="mapper">Автомаппер.</param>
        /// <param name="getCalendarQuery">Запросдля получения календаря.</param>
        public GetIcalForGoogleCalendar(IMapper mapper, GetCalendarQuery getCalendarQuery)
        {
            _mapper = mapper;
            _getCalendarQuery = getCalendarQuery;
        }

        private readonly IMapper _mapper;
        private readonly GetCalendarQuery _getCalendarQuery;

        [BindProperty(SupportsGet = true)] public string? Query { get; set; }

        // ReSharper disable once UnusedMember.Global
        public IActionResult OnGet()
        {
            if (string.IsNullOrWhiteSpace(Request.QueryString.Value))
            {
                throw new Exception("Неверный запрос на получение календаря.");
            }

            var encodedModel = Request.QueryString.Value.Substring(1);
            SettingsModel = CalendarSettingsModel.Deserialize(encodedModel);
            var schedule = GetSchedule(SettingsModel);

            var events = schedule.Matrix
                .Cast<CalendarDayModel>()
                .SelectMany(x => x.Events)
                .ToArray();

            var hiddenLessonIds = events.Where(x => x.IsHiddenByUser)
                .Select(x => x.Color.ToString() + x.Date.ToString())
                .Distinct()
                .ToArray();

            var filteredEvents = events
                .DistinctBy(x => x.Color.ToString() + x.Date.ToString())
                .Where(x => !hiddenLessonIds.Contains(x.Color.ToString() + x.Date.ToString()))
                .ToArray();

            var resultIcal = GetIcalContent(filteredEvents);

            return Content(resultIcal, "text/plain", Encoding.UTF8);
        }


        private string GetIcalContent(IEnumerable<CalendarEventModel> events)
        {
            var data = new List<string>
            {
                "BEGIN:VCALENDAR",
                "PRODID:-//Google Inc//Google Calendar 70.9054//EN",
                "VERSION:2.0",
                "CALSCALE:GREGORIAN",
                "METHOD:PUBLISH",
                "X-WR-TIMEZONE:Asia/Tomsk",
            };

            foreach (CalendarEventModel ev in events)
            {
                data.AddRange(new []
                {
                    "BEGIN:VEVENT",
                    $"DTSTART:{ev.Date:yyyyMMddTHHmmss}",
                    $"DTEND:{ev.Date.AddMinutes(95):yyyyMMddTHHmmss}",
                    $"DTSTAMP:{ev.Date:yyyyMMddTHHmmss}",
                    $"DESCRIPTION:{ev.Teacher}",
                    $"LOCATION:{ev.Place}",
                    $"SUMMARY:{ev.Name} ({ev.Type})",
                    "END:VEVENT",
                });
            }
            data.Add("END:VCALENDAR");

            return string.Join("\r\n", data);
        }

        private CalendarPageViewModel GetSchedule(CalendarSettingsModel settingsModel)
        {
            var queryModel = new GetCalendarQueryModel
            {
                ItemHashes = settingsModel.Items,
                HiddenEvents = _mapper.Map<List<HidableEventEntity>>(settingsModel.HiddenEvents).ToArray(),
                ShowWindows = settingsModel.ShowWindows,
                CountOfWeeksAfterCurrent = settingsModel.CountOfWeeksAfterCurrent
            };
            var queryResult = _getCalendarQuery.Execute(queryModel);

            var calendarModel = queryResult.IsSuccessful
                ? _mapper.Map<CalendarPageViewModel>(queryResult.Data)
                : new CalendarPageViewModel();

            return calendarModel;
        }
    }
}