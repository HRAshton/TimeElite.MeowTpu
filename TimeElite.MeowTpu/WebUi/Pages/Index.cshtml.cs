using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BusinessLogic.Queries.GetCalendarQuery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUi.Models.Calendar;

namespace WebUi.Pages
{
    public class IndexModel : PageModel
    {
        /// <summary>
        ///     Календарь.
        /// </summary>
        public CalendarModel CalendarModel { get; set; }

        /// <summary>
        ///     Календарь.
        /// </summary>
        public string[] Groups { get; set; } = new string[0];

        [BindProperty(SupportsGet = true)] public string EncodedModel { get; set; } = string.Empty;

        public IndexModel(IMapper mapper, GetCalendarQuery getCalendarQuery)
        {
            CalendarModel = new CalendarModel
            {
                Legend = new List<CalendarLegendItemModel>(),
                Matrix = new CalendarDayModel[2, 6]
            };

            _mapper = mapper;
            _getCalendarQuery = getCalendarQuery;
        }

        private readonly IMapper _mapper;
        private readonly GetCalendarQuery _getCalendarQuery;

        // ReSharper disable once UnusedMember.Global
        public void OnGet()
        {
            DecodeReceivedModel(); //todo: shadow legend toggler

            CalendarModel = GetSchedule(Groups);
        }

        private void DecodeReceivedModel()
        {
            var groupHashes = EncodedModel.Split(",", StringSplitOptions.RemoveEmptyEntries);
            Groups = groupHashes;
        }


        private CalendarModel GetSchedule(string[] groupHashes)
        {
            var queryResult = _getCalendarQuery.Execute(groupHashes);

            var calendarModel = queryResult.IsSuccessful
                ? _mapper.Map<CalendarModel>(queryResult.Data)
                : new CalendarModel();

            var newMatrix = new CalendarDayModel[2, 6];
            for (var i = 0; i < 2; i++)
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

            calendarModel.Matrix = newMatrix;

            return calendarModel;
        }
    }
}