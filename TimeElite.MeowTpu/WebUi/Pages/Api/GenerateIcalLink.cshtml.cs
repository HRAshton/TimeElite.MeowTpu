using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUi.Interfaces;

namespace WebUi.Pages.Api
{
    public class GenerateIcalLink : PageModel
    {
        /// <summary>
        ///     Конструтор.
        /// </summary>
        /// <param name="linkShortener">Сократитель ссылок.</param>
        public GenerateIcalLink(ILinkShortener linkShortener)
        {
            LinkShortener = linkShortener;
        }

        private ILinkShortener LinkShortener { get; }

        [BindProperty(SupportsGet = true)] public string? Query { get; set; }

        // ReSharper disable once UnusedMember.Global
        public async Task<IActionResult> OnGet()
        {
            var srcLink = $"https://{Request.Host}/Api/GetIcalForGoogleCalendar{Request.QueryString.Value}";

            var result = await LinkShortener.GetShortenedLink(srcLink);

            return Content(result, "text/plain", Encoding.UTF8);
        }
    }
}