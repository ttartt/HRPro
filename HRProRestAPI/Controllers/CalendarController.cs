using HRProContracts.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            try
            {
                var calendarId = "c3145d64626c4d8e1abec2bd290fc205b4b49aee6eb533b052947a0adf7343ef@group.calendar.google.com";

                if (eventDto.StartDateTime >= eventDto.EndDateTime)
                {
                    return BadRequest(new { message = "Дата окончания должна быть позже даты начала" });
                }

                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events");

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", eventDto.AccessToken);

                var eventData = new
                {
                    summary = eventDto.Title,
                    description = eventDto.Description,
                    location = eventDto.Location,
                    start = new
                    {
                        dateTime = eventDto.StartDateTime.ToString("o"),
                        timeZone = "Europe/Samara"
                    },
                    end = new
                    {
                        dateTime = eventDto.EndDateTime.ToString("o"),
                        timeZone = "Europe/Samara"
                    }
                };

                var json = JsonSerializer.Serialize(eventData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Ошибка Google Calendar API: {response.StatusCode} - {responseContent}");
                    return BadRequest(new
                    {
                        message = "Ошибка при создании события",
                        error = responseContent
                    });
                }

                var googleEvent = JsonSerializer.Deserialize<GoogleCalendarResponse>(responseContent);

                return Ok(new
                {
                    success = true,
                    eventId = googleEvent.Id,
                    message = "Событие успешно добавлено"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в AddToGoogleCalendar");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Внутренняя ошибка сервера"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromGoogleCalendar(string eventId, [FromBody] CalendarEventDto eventDto)
        {
            var calendarId = "c3145d64626c4d8e1abec2bd290fc205b4b49aee6eb533b052947a0adf7343ef@group.calendar.google.com";

            var request = new HttpRequestMessage(HttpMethod.Delete,
                $"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events/{eventId}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", eventDto.AccessToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Ошибка удаления события: {error}");
                return BadRequest(new { message = "Не удалось удалить событие из Google Календаря", error });
            }

            return Ok(new { message = "Событие успешно удалено" });
        }
    }

    public class CalendarEventDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; } 
        public string Location { get; set; } = string.Empty;
    }
}
