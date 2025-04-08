using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HRProRestAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public CalendarController(IHttpClientFactory httpClientFactory, ILogger<CalendarController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddToGoogleCalendar([FromBody] CalendarEventDto eventDto)
        {
            var calendarId = "c3145d64626c4d8e1abec2bd290fc205b4b49aee6eb533b052947a0adf7343ef@group.calendar.google.com";

            var request = new HttpRequestMessage(HttpMethod.Post,
                $"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", eventDto.AccessToken);

            var eventData = new
            {
                summary = eventDto.Title,
                description = eventDto.Description,
                location = eventDto.Location,
                start = new
                {
                    dateTime = eventDto.StartDateTime.ToString(),
                    timeZone = "Europe/Samara" 
                },
                end = new
                {
                    dateTime = eventDto.EndDateTime.ToString(),
                    timeZone = "Europe/Samara"
                }
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(eventData),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Ошибка добавления события: {error}");

                return BadRequest(new { message = "Не удалось добавить событие в Google Календарь", error });
            }

            return Ok(new { message = "Событие успешно добавлено" });
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
