namespace HNG14_Backend_Task1.Models
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public float GenderProbability { get; set; }
        public int SampleSize{ get; set; }
        public int Age { get; set; }
        public string AgeGroup { get; set; }
        public string CountryId { get; set; }
        public float CountryProbability { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
