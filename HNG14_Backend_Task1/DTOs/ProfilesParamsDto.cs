using Microsoft.AspNetCore.Mvc;

namespace HNG14_Backend_Task2.DTOs
{
    public class ProfilesParamsDto
    {
        [FromQuery(Name = "gender")]
        public string? Gender { get; set; } = null;
        [FromQuery(Name = "age_group")]
        public string? AgeGroup { get; set; } = null;

        [FromQuery(Name = "country_id")] // Permet de mapper country_id vers cette propriété
        public string? CountryId { get; set; } = null;

        [FromQuery(Name = "min_age")]
        public int? MinAge { get; set; } = null;
        [FromQuery(Name = "max_age")]
        public int? MaxAge { get; set; } = null;
        [FromQuery(Name = "min_gender_probability")]
        public float? MinGenderProbability { get; set; } = null;
        [FromQuery(Name = "min_country_probability")]
        public float? MinCountryProbability { get; set; } = null;

        [FromQuery(Name = "sort_by")]
        public string? SortBy { get; set; } = null;
        [FromQuery(Name = "order")]
        public string? Order { get; set; } = null;
        [FromQuery(Name = "page")]
        public int Page { get; set; } = 1; // Valeur par défaut
        [FromQuery(Name = "limit")]
        public int Limit { get; set; } = 10;
    }
}
