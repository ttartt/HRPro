using DocumentFormat.OpenXml.EMMA;
using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

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

                return View(list);
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
            if (id.HasValue)
            {
                var invitedParticipants = APIClient.GetRequest<List<MeetingParticipantViewModel>>($"api/meeting/participants?meetingId={id}");

                var invitedUserIds = invitedParticipants.Select(p => p.UserId).ToList();

                ViewBag.InvitedUserIds = invitedUserIds;
            }
            if (!id.HasValue)
            {                
                return View();
            }
            else 
            {
                var meeting = APIClient.GetRequest<MeetingViewModel?>($"api/meeting/details?id={id}");
                return View(meeting);
            }
        }

        [HttpPost]
        public async Task<IActionResult> MeetingEdit(MeetingBindingModel model)
        {
            string redirectUrl = $"/Meeting/Meetings/{APIClient.User?.Id}";
            try
            { 
                if (model.VacancyId <= 0 || model.VacancyId == null)
                {
                    throw new ArgumentException("Вакансия не выбрана");
                }
                if (model.ResumeId <= 0 || model.ResumeId == null)
                {
                    throw new ArgumentException("Резюме не выбрано");
                }

                if (string.IsNullOrEmpty(model.Topic))
                {
                    throw new ArgumentException("Тема встречи не может быть пустой");
                }

                if (model.TimeFrom >= model.TimeTo)
                {
                    throw new ArgumentException("Время начала должно быть меньше времени окончания");
                }

                

                if (string.IsNullOrEmpty(model.Place))
                {
                    throw new ArgumentException("Место проведения встречи не может быть пустым");
                }
                if (model.Id != 0)
                {
                    APIClient.PostRequest("api/meeting/update", model);
                }
                else
                {
                    model.CompanyId = APIClient.Company?.Id;
                    APIClient.PostRequest("api/meeting/create", model);

                    var calendarEvent = new GoogleCalendarEventModel
                    {
                        Title = model.Topic,
                        Description = model.Comment,
                        StartDateTime = model.Date.Date.Add(model.TimeFrom.TimeOfDay),
                        EndDateTime = model.Date.Date.Add(model.TimeTo.TimeOfDay),
                        Location = model.Place
                    };

                    await AddToGoogleCalendar(calendarEvent);

                    APIClient.User?.Meetings.Add(new MeetingViewModel
                    {
                        Id = model.Id,
                        TimeFrom = model.TimeFrom,
                        ResumeId = model.ResumeId,
                        Date = model.Date,
                        Place = model.Place,
                        TimeTo = model.TimeTo,
                        Topic = model.Topic,
                        VacancyId = model.VacancyId,
                        Comment = model.Comment,
                        CompanyId = model.CompanyId
                    });
                }

                return Json(new { success = true, redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToGoogleCalendar([FromBody] GoogleCalendarEventModel model)
        {
            try
            {
                var googleToken = APIClient.User?.GoogleToken;

                if (string.IsNullOrEmpty(googleToken))
                {
                    return Json(new { success = false, message = "Необходима авторизация через Google" });
                }

                var meeting = new
                {
                    AccessToken = googleToken,
                    Title = model.Title,
                    Description = model.Description,
                    StartDateTime = model.StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    EndDateTime = model.EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    Location = model.Location
                };

                var response = await APIClient.PostRequestAsync("api/calendar/AddToGoogleCalendar", meeting);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении в Яндекс.Календарь");
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class GoogleCalendarEventModel
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime StartDateTime { get; set; }
            public DateTime EndDateTime { get; set; }
            public string Location { get; set; }
        }

        [HttpPost]
        public void Delete(UserBindingModel model)
        {
            if (APIClient.User == null)
            {
                throw new Exception("Доступно только авторизованным пользователям");
            }

            APIClient.PostRequest($"api/user/delete", model);
            Response.Redirect("/Home/Enter");
        }
    }
}
