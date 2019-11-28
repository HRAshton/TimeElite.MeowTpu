using System;
using System.Collections.Generic;
using AutoMapper;
using BusinessLogic.Queries.GetSelectableItemsQuery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUi.Models;

namespace WebUi.Pages.Api
{
    public class SearchModel : PageModel
    {
        private readonly IMapper _mapper;

        public SearchModel(GetSelectableItemsQuery getSelectableItemsQuery, IMapper mapper)
        {
            _getSelectableItemsQuery = getSelectableItemsQuery;
            _mapper = mapper;
            Query = string.Empty;
        }

        [BindProperty(SupportsGet = true)] public string Query { get; set; }

        private GetSelectableItemsQuery _getSelectableItemsQuery { get; set; }

        // ReSharper disable once UnusedMember.Global
        public JsonResult OnGet()
        {
            var items = _getSelectableItemsQuery.Execute(new GetSelectableItemsModel
            {
                Query = Query
            });

            var models = _mapper.Map<List<ItemResultModel>>(items.Data);

            return new JsonResult(models);
        }
    }
}