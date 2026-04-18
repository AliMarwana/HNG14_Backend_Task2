using System.Text.Json.Serialization;

namespace HNG14_Backend_Task1.DTOs
{
    public class CountryDto
    {
        [JsonPropertyName("country_id")]
        public string CountryId { get; set; }
        [JsonPropertyName("probability")]
        public double Probability { get; set; }
    }
}
