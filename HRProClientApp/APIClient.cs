using HRProContracts.BindingModels;
using HRProContracts.ViewModels;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace HRProClientApp
{
    public static class APIClient
    {
        private static readonly HttpClient _client = new();
        public static UserViewModel? User { get; set; } = null;
        public static CompanyViewModel? Company { get; set; } = null;
        public static string? Token { get; set; }
        public static string? BaseUrl { get; private set; }

        public static void Connect(IConfiguration configuration)
        {
            BaseUrl = configuration["IPAddress"];
            _client.BaseAddress = new Uri(BaseUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static void ApplyAuthorizationHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                string.IsNullOrEmpty(Token) ? null : new AuthenticationHeaderValue("Bearer", Token);
        }

        public static async Task<T?> GetRequestAsync<T>(string requestUrl)
        {
            try
            {
                ApplyAuthorizationHeader();

                var response = await _client.GetAsync(requestUrl);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(result);
                }

                throw new Exception(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при GET-запросе к {requestUrl}: {ex.Message}", ex);
            }
        }

        public static T? GetRequest<T>(string requestUrl)
        {
            try
            {
                ApplyAuthorizationHeader();

                var response = _client.GetAsync(requestUrl).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(result);
                }

                throw new Exception(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при синхронном GET-запросе к {requestUrl}: {ex.Message}", ex);
            }
        }

        public static void PostRequest<T>(string requestUrl, T model)
        {
            var json = JsonConvert.SerializeObject(model);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            ApplyAuthorizationHeader();

            var response = _client.PostAsync(requestUrl, data).Result;
            var result = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(result);
            }
        }

        public static async Task<int> PostRequestAsync(string requestUrl, object model)
        {
            var json = JsonConvert.SerializeObject(model);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            ApplyAuthorizationHeader();

            var response = await _client.PostAsync(requestUrl, data);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP Error {response.StatusCode}: {responseJson}");
            }
            try
            {
                dynamic responseObject = JsonConvert.DeserializeObject(responseJson);
                return (int)(responseObject?.Id ?? responseObject?.id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при разборе ID из ответа: {responseJson}. Ошибка: {ex.Message}");
            }
        }

        public static async Task<FileUploadResponse> PostFileAsync(string requestUrl, IFormFile? file, string fileName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не выбран или пустой");

            using (var multipartFormContent = new MultipartFormDataContent())
            {
                using (var fileStream = file.OpenReadStream())
                {
                    var fileStreamContent = new StreamContent(fileStream);
                    fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                    var extension = Path.GetExtension(file.FileName);
                    var safeFileName = string.IsNullOrEmpty(extension)
                        ? fileName
                        : $"{Path.GetFileNameWithoutExtension(fileName)}{extension}";

                    multipartFormContent.Add(fileStreamContent, name: "file", fileName: safeFileName);

                    ApplyAuthorizationHeader();

                    var response = await _client.PostAsync(requestUrl, multipartFormContent);
                    var responseJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"HTTP Error {response.StatusCode}: {responseJson}");
                    }

                    return JsonConvert.DeserializeObject<FileUploadResponse>(responseJson);
                }
            }
        }

        public static async Task<TResponse?> PostRequestAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            ApplyAuthorizationHeader();

            var response = await _client.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<TResponse>(result);
            }

            throw new Exception($"Ошибка POST-запроса ({response.StatusCode}): {result}");
        }

        public static async Task<HttpResponseMessage> PostRequestWithFullResponseAsync(string requestUrl, object model)
        {
            var json = JsonConvert.SerializeObject(model);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            ApplyAuthorizationHeader();

            return await _client.PostAsync(requestUrl, data);
        }

        public static async Task<HttpResponseMessage> PostFileWithFullResponseAsync(
            string requestUrl,
            Stream fileStream,
            string fileName,
            Dictionary<string, string> formFields = null)
        {
            var formData = new MultipartFormDataContent();

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            formData.Add(fileContent, "file", fileName);

            if (formFields != null)
            {
                foreach (var field in formFields)
                {
                    formData.Add(new StringContent(field.Value), field.Key);
                }
            }

            ApplyAuthorizationHeader();
            return await _client.PostAsync(requestUrl, formData);
        }

        public static async Task<HttpResponseMessage> GetRequestWithFullResponseAsync(string requestUrl)
        {
            ApplyAuthorizationHeader();
            return await _client.GetAsync(requestUrl);
        }
    }
}
