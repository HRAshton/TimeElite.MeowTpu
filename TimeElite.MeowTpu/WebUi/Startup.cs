using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogic.Models;
using BusinessLogic.Queries.GetCalendarQuery;
using BusinessLogic.Queries.GetSelectableItemsQuery;
using BusinessLogic.Queries.GetSelectableItemsQuery.InternalResultModels;
using Ical.Net.CalendarComponents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebUi.Helpers;
using WebUi.Interfaces;
using WebUi.Models.Calendar;

namespace WebUi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        private IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAutomapper(services);

            var httpClient = new HttpClient();
            services.AddSingleton(httpClient);

            services.AddRazorPages();
            services.AddServerSideBlazor();
            
            services.AddSingleton<GetCalendarQuery>();
            services.AddSingleton<GetSelectableItemsQuery>();
            services.AddSingleton<ILinkShortener, BrandlyLinkShortener>();
            services.AddSingleton(Configuration);
            services.AddApplicationInsightsTelemetry();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Error");

            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub();
            });
        }

        private static void ConfigureAutomapper(IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CalendarEntity, CalendarModel>();
                cfg.CreateMap<CalendarDayEntity, CalendarDayModel>();
                cfg.CreateMap<CalendarEventEntity, CalendarEventModel>();
                cfg.CreateMap<CalendarLegendItemEntity, CalendarLegendItemModel>();

                cfg.CreateMap<SelectableItemEntity, ItemResultModel>()
                    .ForMember(x => x.Title, x => x.MapFrom(y => y.Text))
                    .ForMember(x => x.Hash, x => x.MapFrom(
                        y => y.Url.Replace("/redirect/kalendar.html?hash=", string.Empty)));

                cfg.CreateMap<CalendarEvent, CalendarEventEntity>()
                    .ForMember(x => x.Date, x => x.MapFrom(y => y.DtStart.AsSystemLocal))
                    .ForMember(x => x.Name, x => x.MapFrom(y => y.Name))
                    .ForMember(x => x.Place, x => x.MapFrom(y => y.Location))
                    .ForMember(x => x.Type, x => x.MapFrom(y => y.Categories.SingleOrDefault() ?? string.Empty))
                    .ForMember(x => x.Teacher, x => x.MapFrom(y => y.Contacts.FirstOrDefault() ?? string.Empty));

                cfg.CreateMap<HidableEventModel, HidableEventEntity>();
                cfg.CreateMap<HidableEventEntity, HidableEventModel>();
            });
            services.AddSingleton<IMapper>(new Mapper(config));
        }
    }
}
