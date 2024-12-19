using HRProContracts.ViewModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HRProClientApp
{
    public class APIClient
    {
        private static readonly HttpClient _client = new();
        public static UserViewModel? User { get; set; } = null;
        public static CompanyViewModel? Company { get; set; } = null;
        public static void Connect(IConfiguration configuration)
        {
            _client.BaseAddress = new Uri(configuration["IPAddress"]);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public static T? GetRequest<T>(string requestUrl)
        {
            var response = _client.GetAsync(requestUrl);
            var result = response.Result.Content.ReadAsStringAsync().Result;
            if (response.Result.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(result);
            }
            else
            {
                throw new Exception(result);
            }
        }

        public static async Task<T?> GetRequestAsync<T>(string requestUrl)
        {
            try
            {
                // Асинхронный запрос
                var response = await _client.GetAsync(requestUrl);

                // Чтение содержимого ответа
                var result = await response.Content.ReadAsStringAsync();

                // Проверка статуса ответа
                if (response.IsSuccessStatusCode)
                {
                    // Десериализация результата
                    return JsonConvert.DeserializeObject<T>(result);
                }
                else
                {
                    throw new Exception(result);
                }
            }
            catch (Exception ex)
            {
                // Логирование или дополнительная обработка исключений, если требуется
                throw new Exception($"Ошибка при выполнении запроса к {requestUrl}: {ex.Message}", ex);
            }
        }


        public static void PostRequest<T>(string requestUrl, T model)
        {
            var json = JsonConvert.SerializeObject(model);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(requestUrl, data);
            var result = response.Result.Content.ReadAsStringAsync().Result;
            if (!response.Result.IsSuccessStatusCode)
            {
                throw new Exception(result);
            }
        }

        public static async Task<int> PostRequestAsync(string requestUrl, object model)
        {
            var json = JsonConvert.SerializeObject(model);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(requestUrl, data);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"HTTP Error {response.StatusCode}: {errorContent}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseJson);

            try
            {
                return (int)responseObject.id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not parse ID from response: {responseJson}, Error: {ex.Message}");
            }
        }

        public static async Task PostRequestAsynchron(string requestUrl, object model)
        {
            var json = JsonConvert.SerializeObject(model);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(requestUrl, data);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"HTTP Error {response.StatusCode}: {errorContent}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseJson);
        }
    }
}
