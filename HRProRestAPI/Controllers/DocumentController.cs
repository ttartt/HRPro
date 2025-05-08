using HRProContracts.BindingModels;
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
    public class DocumentController : Controller
    {
        private readonly ILogger _logger;
        private readonly IDocumentLogic _logic;
        private readonly IDocumentTagLogic _documentTagLogic;
        public DocumentController(IDocumentLogic logic, IDocumentTagLogic documentTagLogic, ILogger<DocumentController> logger)
        {
            _logger = logger;
            _logic = logic;
            _documentTagLogic = documentTagLogic;
        }

        [HttpGet]
        public DocumentViewModel? Details(int id)
        {
            try
            {
                return _logic.ReadElement(new DocumentSearchModel
                {
                    Id = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения документа");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] int documentId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран");

            var filePath = Path.Combine("Uploads/Documents", $"{documentId}_{file.FileName}");

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { message = "Файл загружен", path = filePath });
        }


        [HttpGet]
        public List<DocumentViewModel>? Search(string? tags)
        {
            try
            {
                if (!string.IsNullOrEmpty(tags))
                {
                    return _logic.ReadList(new DocumentSearchModel
                    {
                        Name = tags
                    });
                }
                return new List<DocumentViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения документов");
                throw;
            }
        }

        [HttpGet]
        public List<DocumentViewModel>? List(int? userId, int? companyId)
        {
            try
            {
                if (userId.HasValue)
                {
                    return _logic.ReadList(new DocumentSearchModel
                    {
                        CreatorId = userId
                    });
                }
                else if (companyId.HasValue)
                {
                    return _logic.ReadList(new DocumentSearchModel
                    {
                        CompanyId = companyId
                    });
                }
                else if (userId.HasValue && companyId.HasValue)
                {
                    return _logic.ReadList(new DocumentSearchModel
                    {
                        CompanyId = companyId,
                        CreatorId = userId
                    });
                }
                else return _logic.ReadList(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения документов");
                throw;
            }
        }

        [HttpPost]
        public IActionResult Create(DocumentBindingModel model)
        {
            try
            {
                int? id = _logic.Create(model);
                return Ok(new DocumentBindingModel { Id = (int)id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания документа");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveDocument([FromForm] DocumentUploadModel model)
        {
            try
            {
                if (model.File == null || model.File.Length == 0)
                    return BadRequest("Файл не предоставлен");

                var documentsFolder = Path.Combine("Uploads", "Documents");
                Directory.CreateDirectory(documentsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.File.FileName)}";
                var filePath = Path.Combine(documentsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                return Ok(new
                {
                    success = true,
                    filePath = filePath,
                    fileName = fileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Download(int id)
        {
            try
            {
                var document = _logic.ReadElement(new DocumentSearchModel { Id = id });
                if (document == null)
                {
                    return NotFound("Документ не найден");
                }

                var filePath = document.FilePath;

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Файл документа не найден");
                }

                var fileStream = System.IO.File.OpenRead(filePath);

                var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                return File(fileStream, contentType, Path.GetFileName(document.Name));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при скачивании файла: {ex.Message}");
            }
        }

        public class DocumentUploadModel
        {
            public IFormFile File { get; set; }
            public int DocumentId { get; set; }
            public int TemplateId { get; set; }
        }

        [HttpPost]
        public void CreateTag(DocumentTagBindingModel model)
        {
            try
            {
                _documentTagLogic.Create(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания тега документа");
                throw;
            }
        }

        [HttpPost]
        public void Update(DocumentBindingModel model)
        {
            try
            {
                _logic.Update(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления документа");
                throw;
            }
        }

        [HttpPost]
        public IActionResult Delete(DocumentBindingModel model)
        {
            try
            {
                var document = _logic.ReadElement(new DocumentSearchModel { Id = model.Id });
                if (document == null)
                {
                    return NotFound("Документ не найден");
                }

                _logic.Delete(model);

                var filePath = document.FilePath;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation($"Файл документа удален: {filePath}");
                }
                else
                {
                    _logger.LogWarning($"Файл документа не найден: {filePath}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления документа");
                return StatusCode(500, $"Ошибка удаления документа: {ex.Message}");
            }
        }
    }
}