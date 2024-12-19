using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRProClientApp.Controllers
{
    public class RequirementController : Controller
    {
        private readonly ILogger<RequirementController> _logger;

        public RequirementController(ILogger<RequirementController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddVacancyRequirement(int vacancyId, int[] requirements)
        {
            string returnUrl = HttpContext.Request.Headers.Referer.ToString();

            try
            {
                if (APIClient.User == null)
                {
                    throw new Exception("Необходима авторизация");
                }

                if (requirements == null)
                {
                    throw new ArgumentException("Обязанности не найдены");
                }

                foreach (int id in requirements)
                {
                    var model = new VacancyRequirementBindingModel
                    {
                        VacancyId = vacancyId,
                        RequirementId = id
                    };
                    APIClient.PostRequest("api/requirement/createVR", model);
                }

                var vacancy = APIClient.GetRequest<VacancyViewModel>($"api/vacancy/details?id={vacancyId}");
                foreach (int id in requirements)
                {
                    var resp = APIClient.GetRequest<RequirementViewModel>($"api/requirement/details?id={id}");
                    vacancy.Requirements.Add(new RequirementViewModel
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
