using System.Threading.Tasks;

namespace WebUi.Interfaces
{
	/// <summary>
	/// 	Предоставляет методы для сокращения ссылок.
	/// </summary>
	public interface ILinkShortener
	{
		/// <summary>
		/// 	Получить сокращенную ссылку.
		/// </summary>
		/// <returns>Сокращенная ссылка.</returns>
		public Task<string> GetShortenedLink(string srcLink);
	}
}