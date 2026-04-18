using System.Text.Json.Serialization;

namespace HNG14_Backend_Task1.DTOs
{
    public class AgeDto
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("age")]
        public int Age { get; set; }
    }
}
