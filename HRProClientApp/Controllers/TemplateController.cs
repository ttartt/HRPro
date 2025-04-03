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
            var templates = APIClient.GetRequest<List<TemplateViewModel>?>($"api/template/list?companyId={APIClient.Company?.Id}");
            return View(templates);
        }

        [HttpPost]
        public async Task<IActionResult> UploadTemplate(IFormFile file, HRProDataModels.Enums.TemplateTypeEnum type, string name)
        {
            string redirectUrl = $"/Template/Templates";
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Нет названия шаблона");
                }
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Файл не выбран");

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
                    Name = name,
                    FilePath = filePath,
                    Type = type,
                    CompanyId = APIClient.Company?.Id
                };

            
                if (APIClient.User == null)
                {
                    throw new Exception("Доступно только авторизованным пользователям");
                }
                if (model.Id == 0)
                {
                    var existingTemplates = APIClient.GetRequest<List<TemplateViewModel>>($"api/template/list?companyId={APIClient.Company.Id}");
                    var duplicate = existingTemplates?.FirstOrDefault(v =>
                        v.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase) &&
                        v.FilePath == model.FilePath);

                    if (duplicate != null)
                    {
                        throw new InvalidOperationException("Такой шаблон докуента уже существует");
                    }
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

                return Json(new { success = true, redirectUrl });
            }            
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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
            if (APIClient.Company == null)
            {
                throw new Exception("Компания не определена");
            }

            APIClient.PostRequest($"api/template/delete", new TemplateBindingModel { Id = id });
            APIClient.Company = APIClient.GetRequest<CompanyViewModel?>($"api/company/profile?id={APIClient.User?.CompanyId}");

            return Redirect($"~/Template/Templates");
        }

    }
}
