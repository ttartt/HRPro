using DocumentFormat.OpenXml.EMMA;
using HRProClientApp.Models;
using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO.Compression;
using System.Xml;

namespace HRProClientApp.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ILogger<TemplateController> _logger;

        public TemplateController(ILogger<TemplateController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Templates()
        {
            if (APIClient.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            var templates = APIClient.GetRequest<List<TemplateViewModel>?>($"api/template/list");
            return View(templates);
        }

        [HttpPost]
        public async Task<IActionResult> UploadTemplate(IFormFile file, HRProDataModels.Enums.TemplateTypeEnum type)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран.");

            var templatesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates");
            if (!Directory.Exists(templatesDir))
                Directory.CreateDirectory(templatesDir);

            var filePath = Path.Combine(templatesDir, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream);
            } 

            var model = new TemplateBindingModel()
            {
                Name = file.FileName,
                FilePath = filePath,
                Type = type
            };

            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();

            try
            {
                if (APIClient.User == null)
                {
                    throw new Exception("Доступно только авторизованным пользователям");
                }

                var templateId = await APIClient.PostRequestAsync("api/template/create", model);
                DebugXmlStructure(filePath);
                var tags = ExtractTags(filePath);
                var templateViewModel = APIClient.GetRequest<TemplateViewModel?>($"api/template/details?id={templateId}");
                foreach (var tag in tags)
                {
                    var tagModel = new TagBindingModel
                    {
                        TemplateId = templateId,
                        TagName = tag
                    };
                    var tagId = await APIClient.PostRequestAsync("api/tag/create", tagModel);
                    templateViewModel.Tags.Add(new TagViewModel
                    {
                        Id = tagId,
                        TagName = tagModel.TagName,
                        TemplateId = tagModel.TemplateId
                    });
                }

                return RedirectToAction("Templates");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }

        private static void DebugXmlStructure(string filePath)
        {
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


        private static List<string> ExtractTags(string filePath)
        {
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
            return tags;
        }



        public IActionResult Delete(int id)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                if (APIClient.Company == null)
                {
                    throw new Exception("Компания не определена");
                }

                APIClient.PostRequest($"api/template/delete", new TemplateBindingModel { Id = id });
                APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");

                return Redirect($"~/Template/Templates");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
        }

        public IActionResult SearchTemplates(string? tags)
        {
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            try
            {
                if (APIClient.User == null)
                {
                    throw new Exception("Доступно только авторизованным пользователям");
                }

                if (string.IsNullOrEmpty(tags))
                {
                    ViewBag.Message = "Пожалуйста, введите поисковый запрос.";
                    return View(new List<TemplateViewModel?>());
                }

                var results = APIClient.GetRequest<List<TemplateViewModel?>>($"api/template/search?tags={tags}");
                return View(results);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { errorMessage = $"{ex.Message}", returnUrl });
            }
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
