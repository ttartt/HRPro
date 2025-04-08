using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRProRestApi.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly IReportLogic _reportLogic;
        public ReportController(IReportLogic reportLogic)
        {
            _reportLogic = reportLogic;
        }

        [HttpPost]
        public void Resume(ReportBindingModel model)
        {
            try
            {
                _reportLogic.SaveResumeToPdf(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public void Statistics(ReportBindingModel model)
        {
            try
            {
                _reportLogic.SaveResumesStatisticsToPdf(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
