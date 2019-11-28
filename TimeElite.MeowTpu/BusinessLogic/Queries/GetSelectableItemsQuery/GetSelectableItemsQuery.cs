using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AutoMapper;
using BusinessLogic.Queries.GetSelectableItemsQuery.InternalResultModels;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace BusinessLogic.Queries.GetSelectableItemsQuery
{
    /// <summary>
    ///     Запрос для получения календаря.
    /// </summary>
    public class GetSelectableItemsQuery : QueryBase<GetSelectableItemsModel, List<ItemResultModel>>
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        private readonly Dictionary<char, char> _russianEnglishDict;

        /// <summary>
        ///     Конструктор.
        /// </summary>
        /// <param name="httpClient">Http-клиент.</param>
        /// <param name="memoryCache"></param>
        /// <param name="mapper"></param>
        public GetSelectableItemsQuery(HttpClient httpClient, IMemoryCache memoryCache, IMapper mapper)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _mapper = mapper;

            var russianEnglishDict = new Dictionary<char, char>()
            {
                {'q', 'й'}, {'w', 'ц'}, {'e', 'у'}, {'r', 'к'}, {'t', 'е'}, {'y', 'н'},
                {'u', 'г'}, {'i', 'ш'}, {'o', 'щ'}, {'p', 'з'}, {'[', 'х'}, {']', 'ъ'},
                {'a', 'ф'}, {'s', 'ы'}, {'d', 'в'}, {'f', 'а'}, {'g', 'п'}, {'h', 'р'},
                {'j', 'о'}, {'k', 'л'}, {'l', 'д'}, {';', 'ж'}, {'\'', 'э'}, {'z', 'я'},
                {'x', 'ч'}, {'c', 'с'}, {'v', 'м'}, {'b', 'и'}, {'n', 'т'}, {'m', 'ь'},
                {',', 'б'}, {'.', 'ю'}, {'/', '.'}
            };
            _russianEnglishDict = new Dictionary<char, char>(russianEnglishDict);
        }

        /// <summary>
        ///     Выполнить.
        /// </summary>
        /// <returns></returns>
        public override QueryResult<List<ItemResultModel>> Execute(GetSelectableItemsModel model)
        {
            if (model.Query == null || !model.Query.Any())
            {
                return GetSuccessfulResult(new List<ItemResultModel>());
            }

            var allItems = _memoryCache.GetOrCreate("SearchItems", entry =>
            {
                var json = _httpClient.GetStringAsync("https://rasp.tpu.ru/select/search/main.html?page_limit=900000")
                    .Result;
                var rawItemEntities = JsonConvert.DeserializeObject<SelectableItemListEntity>(json);
                var items = _mapper.Map<List<ItemResultModel>>(rawItemEntities.Result);

                return items;
            });

            var layoutedQuery = new string(model.Query
                .ToLowerInvariant()
                .Select(c => _russianEnglishDict.ContainsKey(c) ? _russianEnglishDict[c] : c)
                .ToArray());
            var result = allItems
                .Where(item => item.Html.Contains(layoutedQuery, StringComparison.CurrentCultureIgnoreCase))
                //.Skip(ItemsPerPage * model.Page)
                //.Take(ItemsPerPage)
                .ToList();

            return GetSuccessfulResult(result);
        }
    }
}