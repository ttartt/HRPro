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

        public MeetingController(ILogger<MeetingController> logger)
        {
            _logger = logger;
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
                var list = APIClient.GetRequest<List<MeetingViewModel>>($"api/meeting/listByUserId?userId={userId}");

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
            ViewBag.Candidates = APIClient.GetRequest<List<CandidateViewModel>>($"api/candidate/list");
            ViewBag.Vacancies = APIClient.GetRequest<List<VacancyViewModel>>($"api/vacancy/list?companyId={APIClient.Company?.Id}");
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
        public IActionResult MeetingEdit(MeetingBindingModel model)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                if (model.Id != 0)
                {
                    APIClient.PostRequest("api/meeting/update", model);
                }
                else
                {
                    var createdMeetingId = APIClient.PostRequestAsync("api/meeting/create", model);
                    model.Id = createdMeetingId.Result;

                    APIClient.User?.Meetings.Add(new MeetingViewModel
                    {
                        Id = model.Id,
                        TimeFrom = model.TimeFrom,
                        CandidateId = model.CandidateId,
                        Date = model.Date,
                        Place = model.Place,
                        TimeTo = model.TimeTo,
                        Topic = model.Topic,
                        VacancyId = model.VacancyId,
                        Comment = model.Comment
                    });
                }

                return Redirect($"~/Meeting/Meetings/{APIClient.User?.Id}");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }


        [HttpPost]
        public IActionResult InviteParticipants(int meetingId, int[] userIds)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();

            try
            {
                if (meetingId == 0)
                {
                    throw new ArgumentException("Сначала создайте встречу.");
                }

                foreach (int id in userIds)
                {
                    APIClient.PostRequest("api/meeting/createParticipant", new MeetingParticipantBindingModel
                    {
                        MeetingId = meetingId,
                        UserId = id
                    });

                    var meeting = APIClient.GetRequest<MeetingViewModel>($"api/meeting/details?id={meetingId}");
                    meeting.Participants.Add(APIClient.GetRequest<UserViewModel>($"api/user/profile?id={id}"));
                }

                return Redirect($"~/Meeting/Meetings/{APIClient.User.Id}");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string errorMessage, string returnUrl)
        {
            ViewBag.ErrorMessage = errorMessage ?? "Произошла непредвиденная ошибка.";
            ViewBag.ReturnUrl = returnUrl;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
