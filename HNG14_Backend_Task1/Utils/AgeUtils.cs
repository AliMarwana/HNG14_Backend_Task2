namespace HNG14_Backend_Task1.Utils
{
    public class AgeUtils
    {

        public static string GetAgeGroup(int age)
        {
            if (age >= 0 && age <= 12)
                return "Child";
            else if (age >= 13 && age <= 19)
                return "Teenager";
            else if (age >= 20 && age <= 59)
                return "Adult";
            else if (age > 55)
                return "Senior";
            else
                return "Unknown";
        }
    }
}
