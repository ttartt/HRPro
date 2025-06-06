﻿using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRProRestAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TagController : Controller
    {
        private readonly ILogger _logger;
        private readonly ITagLogic _logic;
        public TagController(ITagLogic logic, ILogger<TagController> logger)
        {
            _logger = logger;
            _logic = logic;
        }

        [HttpGet]
        public TagViewModel? Details(int id)
        {
            try
            {
                return _logic.ReadElement(new TagSearchModel
                {
                    Id = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения тэга");
                throw;
            }
        }

        [HttpPost]
        public IActionResult Create(TagBindingModel model)
        {
            try
            {
                int? id = _logic.Create(model);
                return Ok(new TagBindingModel { Id = (int)id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания тэга");
                throw;
            }
        }

        [HttpPost]
        public void Update(TagBindingModel model)
        {
            try
            {
                _logic.Update(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления тэга");
                throw;
            }
        }

        [HttpPost]
        public void Delete(TagBindingModel model)
        {
            try
            {
                _logic.Delete(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления тэга");
                throw;
            }
        }
    }
}
