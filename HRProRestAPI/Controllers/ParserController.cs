using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.ViewModels;

namespace HRProRestApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ParserController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly IResumeLogic _logic;

        public ParserController(IHttpClientFactory httpClientFactory,
                              IResumeLogic logic,
                              ILogger<ParserController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            _logger = logger;
            _logic = logic;
        }

        [HttpGet]
        public async Task<IActionResult> Parse([FromQuery] string? cityName = "ulyanovsk")
        {
            try
            {
                string url = $"https://www.avito.ru/{cityName}/rezume";

                var response = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(response);

                var savedCount = 0;
                var resumes = new List<ResumeBindingModel>();
                var resumeNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'iva-item-content')]");

                if (resumeNodes == null || !resumeNodes.Any())
                {
                    return Ok(new ApiResponse<List<ResumeBindingModel>>
                    {
                        Success = true,
                        Message = "Резюме не найдены на странице",
                        Data = new List<ResumeBindingModel>()
                    });
                }

                foreach (var node in resumeNodes)
                {
                    try
                    {
                        var resume = ParseResumeNode(node, cityName ?? "Неизвестен");

                        if (resume != null && !string.IsNullOrEmpty(resume.Title))
                        {
                            savedCount++;
                            resumes.Add(resume);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка обработки резюме");
                    }
                }

                return Ok(new ApiResponse<List<ResumeBindingModel>>
                {
                    Success = true,
                    Message = savedCount > 0
                        ? $"Успешно сохранено {savedCount} резюме"
                        : "Новые резюме не найдены",
                    Data = resumes.Take(20).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка сбора резюме");
                return StatusCode(500, new ApiResponse<List<ResumeBindingModel>>
                {
                    Success = false,
                    Message = $"Ошибка сервера: {ex.Message}",
                    Data = new List<ResumeBindingModel>()
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ParseForVacancy([FromQuery] string? cityName, string? tags)
        {
            try
            {
                string url = cityName != null
                    ? $"https://www.avito.ru/{cityName}/rezume?q={tags}"
                    : $"https://www.avito.ru/ulyanovsk/rezume?q={tags}";

                var response = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(response);

                var savedCount = 0;
                var resumes = new List<ResumeBindingModel>();
                var resumeNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'iva-item-content')]");

                if (resumeNodes == null || !resumeNodes.Any())
                {
                    return Ok(new ApiResponse<List<ResumeBindingModel>>
                    {
                        Success = true,
                        Message = "Резюме не найдены на странице",
                        Data = new List<ResumeBindingModel>()
                    });
                }

                foreach (var node in resumeNodes)
                {
                    try
                    {
                        var resume = ParseResumeNode(node, cityName ?? "Неизвестен");

                        if (resume != null && !string.IsNullOrEmpty(resume.Title))
                        {
                            savedCount++;
                            resumes.Add(resume);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка обработки резюме");
                    }
                }

                return Ok(new ApiResponse<List<ResumeBindingModel>>
                {
                    Success = true,
                    Message = savedCount > 0
                        ? $"Успешно сохранено {savedCount} резюме"
                        : "Новые резюме не найдены",
                    Data = resumes.Take(20).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка сбора резюме");
                return StatusCode(500, new ApiResponse<List<ResumeBindingModel>>
                {
                    Success = false,
                    Message = $"Ошибка сервера: {ex.Message}",
                    Data = new List<ResumeBindingModel>()
                });
            }
        }      

        private ResumeBindingModel? ParseResumeNode(HtmlNode node, string city)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Parsing node: " + node.OuterHtml);

                return new ResumeBindingModel
                {
                    Title = node.SelectSingleNode("/html/body/div[5]/div/div[1]/div[3]/div[1]/div[1]/div/div[3]/div/div/div[1]/div/div/main/div[1]/div/div[1]/div/div/div/div/div[1]/div[2]/h3/a/div/span/span")?.InnerText?.Trim()
                           ?? node.SelectSingleNode(".//a[@data-marker='item-title']")?.InnerText?.Trim(),

                    City = city,

                    CandidateInfo = node.SelectSingleNode(".//*[@data-marker='item-line']")?.InnerText?.Trim()
                                  ?? node.SelectSingleNode(".//div[contains(@class, 'data')]//span")?.InnerText?.Trim(),

                    Salary = node.SelectSingleNode("/html/body/div[5]/div/div[1]/div[3]/div[1]/div[1]/div/div[3]/div/div/div[1]/div/div/main/div[1]/div/div[1]/div/div/div/div/div[1]/div[2]/div[3]")?.InnerText?.Trim()
                             ?? node.SelectSingleNode(".//*[@data-marker='item-price']")?.InnerText?.Trim()
                             ?? node.SelectSingleNode(".//meta[@itemprop='price']")?.GetAttributeValue("content", ""),  

                    Url = "https://www.avito.ru" + (
                              node.SelectSingleNode("/html/body/div[5]/div/div[1]/div[3]/div[1]/div[1]/div/div[3]/div/div/div[1]/div/div/main/div[1]/div/div[1]/div/div/div/div/div[1]/div[2]/h3/a")?.GetAttributeValue("href", "")
                              ?? node.SelectSingleNode(".//a[contains(@href, '/rezume/')]")?.GetAttributeValue("href", "")
                              ?? string.Empty)
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing resume: {ex.Message}");
                return null;
            }
        }
    }
}