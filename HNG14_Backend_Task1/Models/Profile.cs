using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace HNG14_Backend_Task1.Models
{
    public class Profile
    {

        [JsonPropertyName("id")]
        [Column("id")] 
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        [Column("name")]
        public string Name { get; set; }
        [JsonPropertyName("gender")]
        [Column("gender")]
        public string Gender { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("gender_probability")]
        [Column("gender_probability")]
        public float? GenderProbability { get; set; }

        [JsonPropertyName("age")]
        [Column("age")]
        public int Age { get; set; }
        [JsonPropertyName("age_group")]
        [Column("age_group")]
        public string AgeGroup { get; set; }
        [JsonPropertyName("country_id")]
        [Column("country_id")]
        public string CountryId { get; set; }
        [JsonPropertyName("country_name")]
        [Column("country_name")]
        public string CountryName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("country_probability")]
        [Column("country_probability")]
        public double? CountryProbability { get; set; }
        [JsonPropertyName("created_at")]
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
