using System.Text.Json.Serialization;

namespace HNG14_Backend_Task1.DTOs
{
    public class NationalizeDto
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("country")]
        public List<CountryDto>? Country { get; set; }
    }
}
