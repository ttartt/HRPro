using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace HRProClientApp.Controllers
{
    public class ResponsibilityController : Controller
    {
        private readonly ILogger<ResponsibilityController> _logger;
        public ResponsibilityController(ILogger<ResponsibilityController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddVacancyResponsibility(int vacancyId, int[] responsibilities)
        {
            string returnUrl = HttpContext.Request.Headers.Referer.ToString();

            try
            {
                if (APIClient.User == null)
                {
                    throw new Exception("Необходима авторизация");
                }

                if (responsibilities == null)
                {
                    throw new ArgumentException("Обязанности не найдены");
                }

                foreach (int id in responsibilities)
                {
                    var model = new VacancyResponsibilityBindingModel
                    {
                        VacancyId = vacancyId,
                        ResponsibilityId = id
                    };
                    APIClient.PostRequest("api/responsibility/createVR", model);
                }   

                var vacancy = APIClient.GetRequest<VacancyViewModel>($"api/vacancy/details?id={vacancyId}");
                foreach (int id in responsibilities)
                {
                    var resp = APIClient.GetRequest<ResponsibilityViewModel>($"api/responsibility/details?id={id}");
                    vacancy.Responsibilities.Add(new ResponsibilityViewModel
                    {
                        Id = resp.Id,
                        Name = resp.Name
                    });
                }   
                return Redirect($"~/Vacancy/EditVacancy/{vacancyId}");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }
    }
}
