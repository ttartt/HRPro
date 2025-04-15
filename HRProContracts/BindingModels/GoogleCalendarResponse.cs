using System.Text.Json.Serialization;

namespace HRProContracts.BindingModels
{
    public class GoogleCalendarResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("htmlLink")]
        public string HtmlLink { get; set; }
    }
}
