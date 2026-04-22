using System.Text.Json.Serialization;


namespace HNG14_Backend_Task1.Models
{
    public class Profile
    {

        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]

        public string Name { get; set; }
        [JsonPropertyName("gender")]
        public string Gender { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("gender_probability")]
        public float? GenderProbability { get; set; }

        [JsonPropertyName("age")]
        public int Age { get; set; }
        [JsonPropertyName("age_group")]
        public string AgeGroup { get; set; }
        [JsonPropertyName("country_id")]
        public string CountryId { get; set; }
        [JsonPropertyName("country_name")]
        public string CountryName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("country_probability")]
        public double? CountryProbability { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
