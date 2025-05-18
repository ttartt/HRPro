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

        [HttpGet]
        public async Task<IActionResult> ParseResumeFromHH([FromQuery] string? cityName, string? tags, string? ageFrom, string? ageTo, string? gender, string? salaryFrom, string? salaryTo)
        {
            try
            {
                string url = cityName != null
                    ? $"https://{cityName}.hh.ru/search/resume?text={tags}&logic=normal&pos=full_text&exp_period=all_time&exp_company_size=any&from=suggest_post&filter_exp_period=all_time&area=98&relocation=living_or_relocation&age_from={ageFrom}&age_to={ageTo}&gender={gender}&salary_from={salaryFrom}&salary_to={salaryTo}&currency_code=RUR&order_by=relevance&search_period=0&items_on_page=50&hhtmFrom=resume_search_form"
                    : $"https://ulyanovsk.hh.ru/search/resume?text={tags}&logic=normal&pos=full_text&exp_period=all_time&exp_company_size=any&from=suggest_post&filter_exp_period=all_time&area=98&relocation=living_or_relocation&age_from={ageFrom}&age_to={ageTo}&gender={gender}&salary_from={salaryFrom}&salary_to={salaryTo}&currency_code=RUR&order_by=relevance&search_period=0&items_on_page=50&hhtmFrom=resume_search_form";

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
        public async Task<IActionResult> ParseVacancies([FromQuery] string? cityName, string? tags)
        {
            try
            {
                string url = cityName != null
                    ? $"https://www.avito.ru/{cityName}/vakansii?q={tags}"
                    : $"https://www.avito.ru/ulyanovsk/vakansii?q={tags}";

                var response = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(response);

                var savedCount = 0;
                var vacancies = new List<ExternalVacancyBindingModel>();
                var vacancyNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'iva-item-content')]");

                if (vacancyNodes == null || !vacancyNodes.Any())
                {
                    return Ok(new ApiResponse<List<ExternalVacancyBindingModel>>
                    {
                        Success = true,
                        Message = "Вакансии не найдены на странице",
                        Data = new List<ExternalVacancyBindingModel>()
                    });
                }

                foreach (var node in vacancyNodes)
                {
                    try
                    {
                        var vacancy = ParseVacancyNode(node, cityName ?? "Неизвестен");

                        if (vacancy != null && !string.IsNullOrEmpty(vacancy.Title))
                        {
                            savedCount++;
                            vacancies.Add(vacancy);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка обработки вакансий");
                    }
                }

                return Ok(new ApiResponse<List<ExternalVacancyBindingModel>>
                {
                    Success = true,
                    Message = savedCount > 0
                        ? $"Успешно сохранено {savedCount} вакансий"
                        : "Новые вакансии не найдены",
                    Data = vacancies.Take(20).ToList()
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

        private ExternalVacancyBindingModel? ParseVacancyNode(HtmlNode node, string city)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Parsing node: " + node.OuterHtml);

                return new ExternalVacancyBindingModel
                {
                    // Основной XPath + fallback на data-marker
                    Title = node.SelectSingleNode(".//p[contains(@class, 'title-root')]")?.InnerText?.Trim()
                           ?? node.SelectSingleNode("/html/body/div[1]/div/buyer-pages-mfe-location/div/div/div/div[2]/div/div[2]/div[3]/div[1]/div[6]/div[2]/div[1]/div/div/div[1]/div[2]/div[1]/div/p")?.InnerText?.Trim()
                           ?? node.SelectSingleNode(".//a[@data-marker='item-title']")?.InnerText?.Trim(),

                    City = city,

                    // Основной XPath + fallback на data-marker и itemprop
                    Salary = node.SelectSingleNode("/html/body/div[1]/div/buyer-pages-mfe-location/div/div/div/div[2]/div/div[2]/div[3]/div[1]/div[6]/div[2]/div[1]/div/div/div[1]/div[2]/div[2]/span/div/div/p/strong/span")?.InnerText?.Trim()
                            ?? node.SelectSingleNode(".//*[@data-marker='item-price']")?.InnerText?.Trim()
                            ?? node.SelectSingleNode(".//meta[@itemprop='price']")?.GetAttributeValue("content", ""),

                    // Основной XPath + fallback на contains href
                    Url = "https://www.avito.ru" + (
                             node.SelectSingleNode("/html/body/div[1]/div/buyer-pages-mfe-location/div/div/div/div[2]/div/div[2]/div[3]/div[1]/div[6]/div[2]/div[1]/div/div/div[1]/div[2]/div[1]/div/p/a")?.GetAttributeValue("href", "")
                             ?? node.SelectSingleNode(".//a[contains(@href, '/vacancy/')]")?.GetAttributeValue("href", "")
                             ?? string.Empty)
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing vacancy: {ex.Message}");
                return null;
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

        private ResumeBindingModel? ParseHHResumeNode(HtmlNode node, string city)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Parsing node: " + node.OuterHtml);

                return new ResumeBindingModel
                {
                    Title = node.SelectSingleNode(".//a[@data-marker='item-title']")?.InnerText?.Trim(),

                    City = city,                   

                    Salary = node.SelectSingleNode(".//*[@data-marker='item-price']")?.InnerText?.Trim()
                            ?? node.SelectSingleNode(".//meta[@itemprop='price']")?.GetAttributeValue("content", ""),

                    Age = node.SelectSingleNode("/html/body/div[5]/div/div[1]/div[3]/div[1]/div[1]/div/div[3]/div/div/div[1]/div/div/main/div[1]/div/div[1]/div/div/div/div/div[1]/div[2]/span[1]/span")?.InnerText?.Trim()
                          ?? node.SelectSingleNode(".//span[contains(text(), 'лет') or contains(text(), 'год')]")?.InnerText?.Trim(),

                    LastWorkPlace = node.SelectSingleNode("/html/body/div[5]/div/div[1]/div[3]/div[1]/div[1]/div/div[3]/div/div/div[1]/div/div/main/div[1]/div/div[1]/div/div/div/div/div[3]/div[2]/div[2]/div[1]/div/span")?.InnerText?.Trim()
                                   ?? node.SelectSingleNode(".//div[contains(@class, 'last-workplace')]")?.InnerText?.Trim(),

                    LastJobTitle = node.SelectSingleNode("/html/body/div[5]/div/div[1]/div[3]/div[1]/div[1]/div/div[3]/div/div/div[1]/div/div/main/div[1]/div/div[1]/div/div/div/div/div[3]/div[2]/div[2]/div[1]/text()[1]")?.InnerText?.Trim()
                                  ?? node.SelectSingleNode(".//div[contains(@class, 'last-job-title')]")?.InnerText?.Trim(),

                    Url = "https://www.avito.ru" +
                         (node.SelectSingleNode(".//a[contains(@href, '/rezume/')]")?.GetAttributeValue("href", "")
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