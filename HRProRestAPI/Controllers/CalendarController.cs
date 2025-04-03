using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HRProRestAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]    
    public class CalendarController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public CalendarController(IHttpClientFactory httpClientFactory, ILogger<CalendarController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddToYandexCalendar([FromBody] CalendarEventDto eventDto)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://calendar.yandex.ru/api/v2/events");

            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", eventDto.AccessToken);

            var eventData = new
            {
                summary = eventDto.Title,
                description = eventDto.Description,
                start = new { dateTime = eventDto.StartDateTime },
                end = new { dateTime = eventDto.EndDateTime },
                location = eventDto.Location
            };

            request.Content = new StringContent(JsonSerializer.Serialize(eventData),
                Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(await response.Content.ReadAsStringAsync());
            }

            return Ok();
        }
    }

    public class CalendarEventDto
    {
        public string AccessToken { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string Location { get; set; }
    }
}
