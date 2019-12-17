﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BusinessLogic.Models;
using BusinessLogic.Queries.GetCalendarQuery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUi.Models;
using WebUi.Models.Calendar;

namespace WebUi.Pages
{
    public class IndexModel : PageModel
    {
        /// <summary>Календарь.</summary>
        public CalendarModel CalendarModel { get; set; }

        /// <summary>Настройки календаря.</summary>
        public CalendarSettingsModel SettingsModel { get; set; }


        /// <summary>
        ///     Конструтор.
        /// </summary>
        /// <param name="mapper">Автомаппер.</param>
        /// <param name="getCalendarQuery">Запросдля получения календаря.</param>
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


        /// <summary>Обработчик запросов пользоателя.</summary>
        // ReSharper disable once UnusedMember.Global
        public void OnGet()
        {
            var encodedModel = Request.QueryString.Value == string.Empty 
                ? string.Empty
                : Request.QueryString.Value.Substring(1);
            encodedModel = encodedModel.Replace("g=", ""); // поддержка старого формата ссылок.
            if (encodedModel.Length < 3 && Request.Cookies.TryGetValue("link", out var link))
            {
                Response.Redirect(link);
            }

            SettingsModel = CalendarSettingsModel.Deserialize(encodedModel); //todo: shadow legend toggler
            CalendarModel = GetSchedule(SettingsModel);

            if (SettingsModel.Items.Any())
            {
                Response.Cookies.Append("link", Request.GetDisplayUrl(), new CookieOptions { MaxAge = TimeSpan.FromDays(365) });
            }
            else
            {
                Response.Cookies.Delete("link");
            }
        }


        private CalendarModel GetSchedule(CalendarSettingsModel settingsModel)
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