using System.Collections.Generic;

namespace BusinessLogic.Queries
{
    /// <summary>
    ///     Результат запроса.
    /// </summary>
    /// <typeparam name="TResult">Тип данных возвращаемых запросом.</typeparam>
    public class QueryResult<TResult>
    {
        /// <summary>
        ///     Конструктор успешного результата.
        /// </summary>
        /// <param name="data">Данные для возврата.</param>
        public QueryResult(TResult data)
        {
            IsSuccessful = true;
            Data = data;
        }

        /// <summary>
        ///     Конструктор неуспешного результата.
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке.</param>
        public QueryResult(string errorMessage = "")
        {
            IsSuccessful = false;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        ///     Результат запроса.
        /// </summary>
        public TResult Data { get; }

        /// <summary>
        ///     Сообщение об ошибке.
        /// </summary>
        public string ErrorMessage { get; } = string.Empty;

        /// <summary>
        ///     Успешен ли запрос.
        /// </summary>
        public bool IsSuccessful { get; }
    }
}