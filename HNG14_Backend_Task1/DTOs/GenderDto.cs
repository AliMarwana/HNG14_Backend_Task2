using System.Text.Json.Serialization;

namespace HNG14_Backend_Task1.DTOs
{
    public class GenderDto
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("gender")]
        public string? Gender { get; set; }
        [JsonPropertyName("probability")]
        public float Probability { get; set; }
    }
}
