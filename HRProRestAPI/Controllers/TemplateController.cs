using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Xml;

namespace HRProRestAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TemplateController : Controller
    {
        private readonly ILogger _logger;
        private readonly ITemplateLogic _logic;
        private readonly ITagLogic _tagLogic;
        public TemplateController(ITemplateLogic logic, ITagLogic tagLogic, ILogger<TemplateController> logger)
        {
            _logger = logger;
            _logic = logic;
            _tagLogic = tagLogic;
        }

        [HttpGet]
        public TemplateViewModel? Details(int id)
        {
            try
            {
                return _logic.ReadElement(new TemplateSearchModel
                {
                    Id = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения шаблона");
                throw;
            }
        }

        [HttpGet]
        public IActionResult File(int id)
        {
            var template = _logic.ReadElement(new TemplateSearchModel { Id = id });
            if (template == null) return NotFound();

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), template.FilePath);
            if (!System.IO.File.Exists(fullPath)) return NotFound();

            return PhysicalFile(fullPath,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                Path.GetFileName(template.FilePath));
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран");

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension) || !extension.Equals(".docx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Только файлы .docx поддерживаются");
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine("Uploads\\Templates", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new
            {
                message = "Файл загружен",
                fileName = fileName,
                relativePath = $"Uploads\\Templates\\{fileName}"
            });
        }

        private static void DebugXmlStructure(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");
            using (var archive = ZipFile.OpenRead(filePath))
            {
                var entry = archive.GetEntry("word/document.xml");
                if (entry != null)
                {
                    using var stream = entry.Open();
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(stream);

                    var allNodes = xmlDoc.SelectNodes("//*");
                    foreach (XmlNode node in allNodes)
                    {
                        if (node.Name.StartsWith("w:"))
                        {
                            Console.WriteLine($"Найден узел: {node.Name} | Значение: {node.InnerText}");
                        }
                    }
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeDocx([FromBody] AnalyzeRequest request)
        {
            try
            {
                var filePath = Path.Combine("Uploads\\Templates", request.FileName);

                if (!System.IO.File.Exists(filePath))
                    return NotFound("Файл не найден");
                DebugXmlStructure(filePath);

                var tags = new List<string>();

                using (var archive = ZipFile.OpenRead(filePath))
                {
                    var entry = archive.GetEntry("word/document.xml");
                    if (entry != null)
                    {
                        using var stream = entry.Open();
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load(stream);

                        var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
                        namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

                        var bookmarkNodes = xmlDoc.SelectNodes("//w:bookmarkStart", namespaceManager);
                        if (bookmarkNodes != null)
                        {
                            foreach (XmlNode node in bookmarkNodes)
                            {
                                var nameAttr = node.Attributes?["w:name"];
                                if (nameAttr != null && !nameAttr.Value.StartsWith("_"))
                                {
                                    tags.Add(nameAttr.Value);
                                }
                            }
                        }

                        var sdtNodes = xmlDoc.SelectNodes("//w:sdt/w:sdtPr/w:tag", namespaceManager);
                        if (sdtNodes != null)
                        {
                            foreach (XmlNode node in sdtNodes)
                            {
                                var nameAttr = node.Attributes?["w:val"];
                                if (nameAttr != null && !nameAttr.Value.StartsWith("_"))
                                {
                                    tags.Add(nameAttr.Value);
                                }
                            }
                        }

                        var instrTextNodes = xmlDoc.SelectNodes("//w:instrText", namespaceManager);
                        if (instrTextNodes != null)
                        {
                            foreach (XmlNode node in instrTextNodes)
                            {
                                if (!string.IsNullOrWhiteSpace(node.InnerText) && node.InnerText.Contains("MERGEFIELD"))
                                {
                                    var parts = node.InnerText.Split(' ');
                                    if (parts.Length > 1)
                                    {
                                        var tagName = parts[1].Trim();
                                        if (!tagName.StartsWith("_"))
                                        {
                                            tags.Add(tagName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Console.WriteLine($"Найдено {tags.Count} тегов: {string.Join(", ", tags)}");
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Download(int id)
        {
            try
            {
                var template = _logic.ReadElement(new TemplateSearchModel { Id = id });
                if (template == null)
                {
                    return NotFound("Шаблон не найден");
                }

                var filePath = template.FilePath;

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Файл шаблона не найден");
                }

                var fileStream = System.IO.File.OpenRead(filePath);

                var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                return File(fileStream, contentType, Path.GetFileName(template.Name));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при скачивании файла: {ex.Message}");
            }
        }

        public class AnalyzeRequest
        {
            public string FileName { get; set; }
        }


        [HttpGet]
        public List<TemplateViewModel>? Search(string? tags)
        {
            try
            {
                if (!string.IsNullOrEmpty(tags))
                {
                    return _logic.ReadList(new TemplateSearchModel
                    {
                        Name = tags
                    });
                }
                return new List<TemplateViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения шаблонов");
                throw;
            }
        }

        [HttpGet]
        public List<TagViewModel>? Tags(int? templateId)
        {
            try
            {
                var list = _tagLogic.ReadList(new TagSearchModel { TemplateId = templateId });
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения тэгов шаблона");
                throw;
            }
        }

        [HttpGet]
        public List<TemplateViewModel>? List(int? companyId)
        {
            try
            {
                if (companyId.HasValue)
                {
                    return _logic.ReadList(new TemplateSearchModel
                    {
                        CompanyId = companyId
                    });
                }
                else return _logic.ReadList(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения шаблонов");
                throw;
            }
        }

        [HttpPost]
        public IActionResult Create(TemplateBindingModel model)
        {
            try
            {
                int? id = _logic.Create(model);
                return Ok(new TemplateBindingModel { Id = (int)id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания шаблона");
                throw;
            }
        }

        [HttpPost]
        public void Update(TemplateBindingModel model)
        {
            try
            {
                _logic.Update(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления шаблона");
                throw;
            }
        }

        [HttpPost]
        public IActionResult Delete(TemplateBindingModel model)
        {
            try
            {
                var template = _logic.ReadElement(new TemplateSearchModel { Id = model.Id });
                if (template == null)
                {
                    return NotFound("Шаблон не найден");
                }

                _logic.Delete(model);

                var filePath = template.FilePath;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation($"Файл шаблона удален: {filePath}");
                }
                else
                {
                    _logger.LogWarning($"Файл шаблона не найден: {filePath}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления шаблона");
                return StatusCode(500, $"Ошибка удаления шаблона: {ex.Message}");
            }
        }
    }
}
