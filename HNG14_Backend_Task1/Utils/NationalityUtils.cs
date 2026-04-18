using HNG14_Backend_Task1.DTOs;

namespace HNG14_Backend_Task1.Utils
{
    public class NationalityUtils
    {
        public static CountryDto? GetConvenientCountry(NationalizeDto nationalize)
        {
            var countriesOrdered = nationalize?.Country?.OrderByDescending(p => p.Probability).ToList();
            if(countriesOrdered != null && countriesOrdered.Count > 0)
            {
                return countriesOrdered.FirstOrDefault();
            }
            return null;
        }
    }
}
