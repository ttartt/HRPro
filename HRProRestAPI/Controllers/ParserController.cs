using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HRproDatabaseImplement.Models;
using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;

namespace HRProRestApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ParserController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly IResumeLogic _logic;

        public ParserController(IHttpClientFactory httpClientFactory, IResumeLogic logic, ILogger<ParserController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _logic = logic;
        }

        [HttpGet]
        public async Task<IActionResult> Parse(string? cityName)
        {
            string url = cityName != null ? $"https://www.avito.ru/{cityName}/rezume" : "https://www.avito.ru/ulyanovsk/rezume";
            var response = await _httpClient.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            var resumes = new List<ResumeBindingModel>();

            var resumeNodes = doc.DocumentNode.SelectNodes("/html/body/div[1]/div/buyer-location/div/div/div/div[2]/div/div[2]/div[3]/div[3]/div[4]/div[2]");

            if (resumeNodes == null || !resumeNodes.Any())
            {
                _logger.LogWarning("Резюме не найдены на странице: {Url}", url);
                return BadRequest(new { success = false, message = "Резюме не найдены" }); // ⬅ Теперь отдаем ошибку
            }

            foreach (var node in resumeNodes)
            {
                var resume = new ResumeBindingModel
                {
                    Title = node.SelectSingleNode("./div[5]/div/div/div[2]/div[2]/div[1]/div/a/h3")?.InnerText.Trim(),
                    Experience = node.SelectSingleNode("./div[5]/div/div/div[2]/div[4]/div[2]/p")?.InnerText.Trim(),
                    Description = node.SelectSingleNode("./div[1]/div/div/div[2]/div[4]/div[1]/p")?.InnerText.Trim(),
                    Salary = node.SelectSingleNode("./div[5]/div/div/div[2]/div[2]/div[2]/span/div/div/p/strong/span")?.InnerText.Trim(),
                    CandidateInfo = node.SelectSingleNode("./div[5]/div/div/div[2]/div[4]/div[1]/p/span/span")?.InnerText.Trim()
                };

                try
                {
                    _logic.Create(resume);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка создания резюме");
                    return StatusCode(500, new { success = false, message = "Ошибка сервера" }); // ⬅ Теперь корректно отдаем ошибку
                }

                resumes.Add(resume);
            }

            return Ok(new { success = true, resumes });
        }

    }
}
