namespace BusinessLogic.Queries
{
    /// <summary>
    ///     Интерфейс запроса.
    /// </summary>
    /// <typeparam name="TQueryModel">Модель для запроса.</typeparam>
    /// <typeparam name="TResult">Результат запроса.</typeparam>
    public interface IQuery<in TQueryModel, TResult>
    {
        /// <summary>
        ///     Выполнить запрос.
        /// </summary>
        /// <param name="model">Модель для запроса.</param>
        /// <returns>Результат запроса.</returns>
        QueryResult<TResult> Execute(TQueryModel model);
    }
}