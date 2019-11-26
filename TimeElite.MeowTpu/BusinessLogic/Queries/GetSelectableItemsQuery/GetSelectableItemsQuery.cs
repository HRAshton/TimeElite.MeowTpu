using System.Net.Http;
using BusinessLogic.Queries.GetSelectableItemsQuery.InternalModels;
using Newtonsoft.Json;

namespace BusinessLogic.Queries.GetSelectableItemsQuery
{
    /// <summary>
    ///     Запрос для получения календаря.
    /// </summary>
    public class GetSelectableItemsQuery : QueryBase<string[], SelectableItemListEntity>
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        ///     Конструктор.
        /// </summary>
        /// <param name="httpClient">Http-клиент.</param>
        public GetSelectableItemsQuery(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        ///     Выполнить.
        /// </summary>
        /// <returns></returns>
        public override QueryResult<SelectableItemListEntity> Execute(string[] calendarLinks)
        {
            var t = _httpClient.GetStringAsync("https://rasp.tpu.ru/select/search/main.html?page_limit=900000").Result;
            var tt = JsonConvert.DeserializeObject<SelectableItemListEntity>(t);

            return GetSuccessfulResult(tt);
        }
    }
}