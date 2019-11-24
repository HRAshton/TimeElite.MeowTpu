namespace BusinessLogic.Queries
{
    /// <summary>
    ///     Базовый класс запроса.
    /// </summary>
    /// <typeparam name="TQueryModel">Модель для запроса.</typeparam>
    /// <typeparam name="TResult">Результат запроса.</typeparam>
    public abstract class QueryBase<TQueryModel, TResult> : IQuery<TQueryModel, TResult>
    {
        /// <summary>
        ///     Выполнить запрос.
        /// </summary>
        /// <param name="model">Модель для запроса.</param>
        /// <returns>Результат запроса.</returns>
        public abstract QueryResult<TResult> Execute(TQueryModel model);

        /// <summary>
        ///     Привести полученные данные к результатам запроса.
        /// </summary>
        /// <param name="data">Данные полученные в запросе.</param>
        /// <returns>Результат запроса.</returns>
        protected QueryResult<TResult> GetSuccessfulResult(TResult data)
        {
            return new QueryResult<TResult>(data);
        }

        /// <summary>
        ///     Получить неуспешный результат запроса из сообщения об ошибке.
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке.</param>
        /// <returns>Результат запроса.</returns>
        protected QueryResult<TResult> GetFailedResult(string errorMessage)
        {
            return new QueryResult<TResult>(errorMessage);
        }
    }
}