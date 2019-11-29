using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogic.Queries.GetCalendarQuery;
using BusinessLogic.Queries.GetCalendarQuery.InternalModels;
using BusinessLogic.Queries.GetSelectableItemsQuery;
using BusinessLogic.Queries.GetSelectableItemsQuery.InternalResultModels;
using Ical.Net.CalendarComponents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebUi.Models;
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

            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy(new Uri("http://10.0.25.3:8080")) { UseDefaultCredentials = true },
                DefaultProxyCredentials = CredentialCache.DefaultCredentials
            };
            var httpClient = new HttpClient(handler);
            services.AddSingleton(httpClient);

            services.AddRazorPages();
            services.AddServerSideBlazor();
            
            services.AddSingleton<GetCalendarQuery>();
            services.AddSingleton<GetSelectableItemsQuery>();
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
            });
            services.AddSingleton<IMapper>(new Mapper(config));
        }
    }
}
