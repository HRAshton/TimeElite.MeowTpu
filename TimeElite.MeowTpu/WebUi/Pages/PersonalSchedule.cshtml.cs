using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Xml;
using AutoMapper;
using BusinessLogic.Queries.GetCalendarQuery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using WebUi.Models.Calendar;

namespace WebUi.Pages
{
    public class ScheduleViewModel : PageModel
    {
        /// <summary>
        ///     Календарь.
        /// </summary>
        public CalendarModel CalendarModel { get; set; }

        /// <summary>
        /// Группы, для которых получается расписание.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string[] Groups { get; set; }

        public ScheduleViewModel(IMapper mapper, GetCalendarQuery getCalendarQuery, IMemoryCache memoryCache)
        {
            _mapper = mapper;
            _getCalendarQuery = getCalendarQuery;
            _memoryCache = memoryCache;
        }

        private readonly IMapper _mapper;
        private readonly GetCalendarQuery _getCalendarQuery;
        private readonly IMemoryCache _memoryCache;

        // ReSharper disable once UnusedMember.Global
        public void OnGet()
        {
            var viewModel = _memoryCache.GetOrCreate(nameof(CalendarModel), entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                var result = GetSchedule(Groups);

                return result;
            });

            CalendarModel = viewModel;
        }


        private CalendarModel GetSchedule(string[] groups)
        {
            var queryResult = _getCalendarQuery.Execute(groups);

            var calendarModel = queryResult.IsSuccessful
                ? _mapper.Map<CalendarModel>(queryResult.Data)
                : new CalendarModel();

            var newMatrix = new CalendarDayModel[2, 6];
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 6; j++)
                {
                    newMatrix[i, j] = calendarModel.Matrix[i, j];
                    newMatrix[i, j].Events = newMatrix[i, j].Events
                        .GroupBy(x => (x.Date, x.Type, x.Name, x.Color))
                        .Select(gr =>
                        {
                            var model = gr.First();
                            model.Summary = gr.Select(x => (x.Place, x.Teacher)).ToArray();

                            return model;
                        })
                        .OrderBy(x => x.Date)
                        .ThenBy(x => x.Color.ToArgb())
                        .ToList();
                }
            }

            calendarModel.Matrix = newMatrix;

            return calendarModel;
        }
    }
}