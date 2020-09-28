using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WebUi.Interfaces;

namespace WebUi.Helpers
{
	public class BrandlyLinkShortener : ILinkShortener
	{
		/// <summary>
		/// 	Конструктор.
		/// </summary>
		/// <param name="httpClient">HttpClient.</param>
		/// <param name="configuration">Конфигурация.</param>
		public BrandlyLinkShortener(HttpClient httpClient, IConfiguration configuration)
		{
			HttpClient = httpClient;
			httpClient.DefaultRequestHeaders.Add("apikey", configuration["RebrandlyApiKey"]);
		}
		
		
		private HttpClient HttpClient { get; }
		
		
		public async Task<string> GetShortenedLink(string srcLink)
		{
#if DEBUG
			srcLink = srcLink.Replace("localhost", "localhost.ru");
#endif
			
			var payload = new
			{
				destination = srcLink,
			};

			var body = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

			using var response = await HttpClient.PostAsync("https://api.rebrandly.com/v1/links", body);
			response.EnsureSuccessStatusCode();

			var model = JsonConvert.DeserializeObject<RebrandlyResponse>(await response.Content.ReadAsStringAsync());

			return $"https://{model.ShortUrl}";
		}

		private class RebrandlyResponse
		{
			[JsonProperty("shortUrl")]
			public string ShortUrl { get; set; } = string.Empty;
		}
	}
}