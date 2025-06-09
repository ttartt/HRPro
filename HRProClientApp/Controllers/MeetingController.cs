using DocumentFormat.OpenXml.Office2010.Excel;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json;

namespace HRProClientApp.Controllers
{
    public class MeetingController : Controller
    {
        private readonly ILogger<MeetingController> _logger;
        private readonly IConfiguration _configuration;

        public MeetingController(ILogger<MeetingController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult MeetingDetails(int? id)
        {
            var model = APIClient.GetRequest<MeetingViewModel>($"api/meeting/details?id={id}");

            if (model == null)
            {
                return Redirect("/Home/Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Meetings(int? userId)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                var list = APIClient.GetRequest<List<MeetingViewModel>>($"api/meeting/list?userId={userId}&companyId={APIClient.Company?.Id}");
                ViewData["GoogleClientId"] = _configuration["Google:ClientId"];
                foreach (var meeting in list)
                {
                    meeting.Date = meeting.Date.ToLocalTime();
                    meeting.TimeFrom = meeting.TimeFrom.ToLocalTime();
                    meeting.TimeTo = meeting.TimeTo.ToLocalTime();
                }

                return View(list.OrderByDescending(x => x.Date).ToList());
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }

        [HttpGet]
        public IActionResult MeetingEdit(int? id)
        {
            if (APIClient.User == null)
            {
                return Redirect("/Home/Enter");
            }

            if (APIClient.Company == null)
            {
                return Redirect("/Home/Index");
            }

            ViewBag.Users = APIClient.GetRequest<List<UserViewModel>>($"api/user/list?companyId={APIClient.Company?.Id}");
            ViewBag.Vacancies = APIClient.GetRequest<List<VacancyViewModel>>($"api/vacancy/list?companyId={APIClient.Company?.Id}");
            ViewBag.Resumes = APIClient.GetRequest<List<ResumeViewModel>>($"api/resume/list?companyId={APIClient.Company?.Id}");
            var employees = APIClient.GetRequest<List<UserViewModel>>($"api/user/list?companyId={APIClient.Company?.Id}");
            ViewBag.Employees = employees.Where(x => x.Id != APIClient.User.Id);

            if (!id.HasValue)
            {
                ViewBag.InvitedEmployeeIds = new List<int>();
                return View();
            }
            else
            {
                var meeting = APIClient.GetRequest<MeetingViewModel?>($"api/meeting/details?id={id}");
                ViewBag.InvitedEmployeeIds = APIClient.GetRequest<List<MeetingParticipantViewModel>?>($"api/meeting/Participants?meetingId={id}")?
                    .Select(p => p.UserId)  
                    .ToList() ?? new List<int>();
                meeting.Date = meeting.Date.ToLocalTime();
                meeting.TimeFrom = meeting.TimeFrom.ToLocalTime();
                meeting.TimeTo = meeting.TimeTo.ToLocalTime();
                return View(meeting);
            }
        }

        [HttpPost]
        public async Task<IActionResult> MeetingEdit(MeetingBindingModel model, string redirectUrl)
        {
            try
            {
                ValidateMeetingModel(model);

                if (model.Id == 0)
                {
                    CheckForDuplicateMeetings(model);
                }
                var timeFrom = model.TimeFrom;
                var timeTo = model.TimeTo;
                var date = model.Date;
                model.Date = model.Date.ToUniversalTime();
                model.TimeFrom = model.TimeFrom.ToUniversalTime();
                model.TimeTo = model.TimeTo.ToUniversalTime();
                model.CompanyId = APIClient.Company?.Id;
                timeFrom = date.Date.Add(timeFrom.TimeOfDay);
                timeTo = date.Date.Add(timeTo.TimeOfDay);

                string googleEventId = null;

                if (!string.IsNullOrEmpty(APIClient.User?.GoogleToken) && string.IsNullOrEmpty(model.GoogleEventId))
                {
                    googleEventId = await CreateGoogleCalendarEvent(model, timeFrom, timeTo);                    
                }

                model.GoogleEventId = googleEventId;

                if (model.Id == 0)
                {
                    var meetingId = await APIClient.PostRequestAsync("api/meeting/create", model);
                    model.SelectedParticipantIds.Add(APIClient.User.Id);

                    if (model.SelectedParticipantIds != null && model.SelectedParticipantIds.Count > 0)
                    {                        
                        foreach (var id in model.SelectedParticipantIds)
                        {
                            var participantModel = new { MeetingId = meetingId, UserId = id };
                            APIClient.PostRequest("api/meeting/createParticipant", participantModel);

                            var participant = APIClient.GetRequest<UserViewModel?>($"api/user/profile?id={id}");
                            if (participant != null)
                            {
                                SendEmail(participant.Email,
                                        date.ToShortDateString(),
                                        timeFrom.ToShortTimeString(),
                                        timeTo.ToShortTimeString(),
                                        model.Topic);
                            }
                        }
                    }

                    APIClient.User?.Meetings.Add(new MeetingViewModel
                    {
                        Id = meetingId,
                        TimeFrom = timeFrom,
                        ResumeId = model.ResumeId,
                        Date = date,
                        Place = model.Place,
                        TimeTo = timeTo,
                        Topic = model.Topic,
                        VacancyId = model.VacancyId,
                        Comment = model.Comment,
                        GoogleEventId = model.GoogleEventId,
                        CompanyId = model.CompanyId
                    });
                }
                else
                {
                    var currentParticipants = APIClient.GetRequest<List<MeetingParticipantViewModel>>($"api/meeting/participants?meetingId={model.Id}");
                    var currentParticipantIds = currentParticipants.Select(p => p.UserId).ToList();

                    var newParticipantIds = model.SelectedParticipantIds?
                        .Except(currentParticipantIds)
                        .ToList() ?? new List<int>();

                    model.SelectedParticipantIds.Add(APIClient.User.Id);

                    var participantsToRemove = currentParticipantIds
                        .Except(model.SelectedParticipantIds ?? new List<int>())
                        .ToList();

                    APIClient.PostRequest("api/meeting/update", model);

                    foreach (var id in participantsToRemove)
                    {
                        APIClient.PostRequest($"api/meeting/DeleteParticipant", new MeetingParticipantBindingModel { Id = id });
                    }

                    foreach (var id in newParticipantIds)
                    {
                        var participantModel = new { MeetingId = model.Id, UserId = id };
                        var existingParticipant = APIClient.GetRequest<MeetingParticipantViewModel>($"api/meeting/participant?meetingId={model.Id}&userId={id}");
                        if (existingParticipant == null) 
                            APIClient.PostRequest("api/meeting/createParticipant", participantModel);

                        var participant = APIClient.GetRequest<UserViewModel?>($"api/user/profile?id={id}");
                        if (participant != null)
                        {
                            SendEmail(
                                participant.Email,
                                date.ToShortDateString(),
                                timeFrom.ToShortTimeString(),
                                timeTo.ToShortTimeString(),
                                model.Topic
                            );
                        }
                    }
                }

                return Json(new { success = true, redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private void ValidateMeetingModel(MeetingBindingModel model)
        {
            if (string.IsNullOrEmpty(model.Topic))
                throw new ArgumentException("Тема встречи не может быть пустой");

            if (string.IsNullOrEmpty(model.Comment))
                throw new ArgumentException("Комментарий не может быть пустым");

            if (string.IsNullOrEmpty(model.Place))
                throw new ArgumentException("Место встречи не может быть пустым");

            if (model.TimeFrom >= model.TimeTo)
                throw new ArgumentException("Время начала должно быть раньше времени окончания");
        }

        private void CheckForDuplicateMeetings(MeetingBindingModel model)
        {
            var existingMeetings = APIClient.GetRequest<List<MeetingViewModel>>(
                $"api/meeting/list?userId={APIClient.User?.Id}&companyId={APIClient.Company?.Id}");

            var duplicate = existingMeetings?.FirstOrDefault(v =>
                v.Date == model.Date &&
                v.TimeFrom == model.TimeFrom &&
                v.TimeTo == model.TimeTo);

            if (duplicate != null)
            {
                throw new InvalidOperationException("Такая встреча уже существует");
            }
        }

        private async Task<string> CreateGoogleCalendarEvent(MeetingBindingModel model, DateTime localStart, DateTime localEnd)
        {
            var calendarEvent = new GoogleCalendarEventModel
            {
                Title = model.Topic,
                Description = model.Comment,
                StartDateTime = localStart,
                EndDateTime = localEnd,
                Location = model.Place
            };

            var response = await AddToGoogleCalendar(calendarEvent);

            if (response is JsonResult jsonResult)
            {
                var responseValue = jsonResult.Value;

                dynamic responseObj = responseValue;

                if (responseObj.success == true)
                {
                    return responseObj.eventId.ToString();
                }
                else
                {
                    throw new Exception(responseObj.message?.ToString() ?? "Не удалось создать событие в Google Calendar");
                }
            }

            throw new Exception("Неверный формат ответа от сервера");
        }

        [HttpPost]
        public async Task<IActionResult> AddToGoogleCalendar([FromBody] GoogleCalendarEventModel model)
        {
            try
            {
                if (model.StartDateTime >= model.EndDateTime)
                {
                    return Json(new { success = false, message = "Дата окончания должна быть позже даты начала" });
                }

                var meeting = new
                {
                    AccessToken = APIClient.User?.GoogleToken,
                    Title = model.Title,
                    Description = model.Description,
                    StartDateTime = model.StartDateTime,
                    EndDateTime = model.EndDateTime,
                    Location = model.Location
                };

                var response = await APIClient.PostRequestWithFullResponseAsync("api/calendar/AddToGoogleCalendar", meeting);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"HTTP Error {response.StatusCode}: {responseContent}");
                }

                var responseObj = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return Json(new { success = true, eventId = (string)responseObj.eventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении в Google Календарь");
                return Json(new
                {
                    success = false,
                    message = ex.Message.StartsWith("HTTP Error") ?
                             "Ошибка при создании события в календаре" :
                             ex.Message
                });
            }
        }

        [HttpPost("DeleteMeeting")]
        public async Task<IActionResult> DeleteMeeting(int meetingId)
        {
            try
            {
                var meeting = APIClient.GetRequest<MeetingViewModel>($"api/meeting/details?id={meetingId}");
                if (meeting == null) throw new Exception("Встреча не найдена");

                if (!string.IsNullOrEmpty(meeting.GoogleEventId))
                {
                    try
                    {
                        await APIClient.PostRequestAsync(
                            "api/calendar/DeleteEvent",
                            new DeleteEventRequest
                            {
                                EventId = meeting.GoogleEventId,
                                GoogleAccessToken = APIClient.User?.GoogleToken,
                                MeetingId = meetingId
                            });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Ошибка при удалении из Google Calendar: {ex.Message}");
                    }
                }

                APIClient.PostRequest("api/meeting/delete", new MeetingBindingModel { Id = meetingId });

                return RedirectToAction("Meetings", new
                {
                    userId = APIClient.User?.Id,
                    companyId = APIClient.Company?.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении встречи");
                return BadRequest(ex.Message);
            }
        }

        private void SendEmail(string email, string date, string timeFrom, string timeTo, string meetingName)
        {
            APIClient.PostRequest("api/user/SendToMail", new MailSendInfoBindingModel
            {
                MailAddress = email,
                Subject = $"Приглашение на встречу: {meetingName}",
                Text = $"Вас пригласили на встречу, которая состоится {date} с {timeFrom} до {timeTo}"
            });
        }

        public class GoogleCalendarEventModel
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime StartDateTime { get; set; }
            public DateTime EndDateTime { get; set; }
            public string Location { get; set; }
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (APIClient.User == null)
            {
                throw new Exception("Доступно только авторизованным пользователям");
            }
            if (APIClient.Company == null)
            {
                throw new Exception("Компания не определена");
            }
            await DeleteMeeting(id);
            APIClient.PostRequest($"api/meeting/delete", new MeetingBindingModel { Id = id });
            
            return Redirect($"~/Meeting/Meetings?userId={APIClient.User?.Id}");
        }
    }
}
