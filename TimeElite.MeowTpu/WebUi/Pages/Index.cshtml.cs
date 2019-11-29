using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using BusinessLogic.Queries.GetCalendarQuery;
using Core.Enums;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUi.Enums;
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
        public string[] Items { get; set; } = new string[0];

        public HiddenEventModel[] HiddenEvents { get; set; } = new HiddenEventModel[0];
        public ViewType ViewType { get; set; } = ViewType.Tabled;
        public bool ShowWindows { get; set; }


        [BindProperty(SupportsGet = true)]
        public string EncodedModel { get; set; } = string.Empty;


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
            if (Request.Path.Value.Length < 3 && Request.Cookies.TryGetValue("link", out var link))
            {
                EncodedModel = new Uri(link).LocalPath.Substring(1);
                Response.Redirect(link);
            }

            DecodeReceivedModel(); //todo: shadow legend toggler

            CalendarModel = GetSchedule(Items, HiddenEvents, ShowWindows);

            if (Items.Any())
            {
                Response.Cookies.Append("link", Request.GetDisplayUrl());
            }
            else
            {
                Response.Cookies.Delete("link");
            }
        }

        private void DecodeReceivedModel()
        {
            var blocks = EncodedModel.Split(";");

            if (blocks.Length != 4)
            {
                return;
            }

            var viewType = (ViewType)Convert.ToByte(blocks[0]);

            var feelWindows = Convert.ToByte(blocks[1]) == 1;
            var itemHashes = blocks[2].Split(",", StringSplitOptions.RemoveEmptyEntries);

            var hiddenEvents = blocks[3].Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(":"))
                .Where(x => x.Length == 5)
                .Select(x => new HiddenEventModel
                {
                    ParentItemHash = x[0],
                    WeekType = (WeekType)Convert.ToByte(x[1]),
                    WeekDay = (DayOfWeek)Convert.ToByte(x[2]),
                    EventIndex = Convert.ToByte(x[3]),
                    Place = HttpUtility.UrlDecode(x[4])
                })
                .ToArray();

            Items = itemHashes;
            HiddenEvents = hiddenEvents;
            ViewType = viewType;
            ShowWindows = feelWindows;
        }


        private CalendarModel GetSchedule(string[] groupHashes, HiddenEventModel[] hiddenEvents, bool feelWindows)
        {
            var queryModel = new GetCalendarQueryModel
            {
                ItemHashes = groupHashes,
                HiddenEvents = hiddenEvents,
                FeelWindows = feelWindows
            };
            var queryResult = _getCalendarQuery.Execute(queryModel);

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