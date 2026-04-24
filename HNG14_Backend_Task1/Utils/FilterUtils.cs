using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.Eventing.Reader;

namespace HNG14_Backend_Task2.Utils
{
    public class FilterUtils
    {
        public static List<string> LeftExpressions { get; set; } = new List<string>
        {
            "Id", "Name", "Gender", "GenderProbability", "Age", "AgeGroup", "CountryId", "CountryName",
            "CountryProbability", "CreatedAt"
        };

        public static string GetCorrespondingLeftExpression(string sentenceWord)
        {
            sentenceWord = sentenceWord.ToLower().Trim();
            if(sentenceWord.Contains("young"))
            {
                return "Age";
            }
            if(sentenceWord.Contains("teenager"))
            {
                return "Age";
            }
            if(sentenceWord.Contains("child"))
            {
                return "Age";
            }
            if(sentenceWord.Contains("adult"))
            {
                return "Age";
            }
            if(sentenceWord.Contains("senior"))
            {
                return "Age";
            }
            if(sentenceWord.Contains("male"))
            {
                return "Gender";
            }
            if(sentenceWord.Contains("female"))
            {
                return "Gender";
            }
            if (sentenceWord.Contains("above") || sentenceWord.Contains("below") || sentenceWord.Contains("equal"))
            {
                return "Age";
            }
            if(sentenceWord.Contains("from"))
            {
                return "CountryName";
            }
            return "";
        }

        public static List<string> GetCorrespondingComparator(string sentenceWord)
        {
            sentenceWord = sentenceWord.ToLower().Trim();
            if (sentenceWord.Contains("young"))
            {
                return new List<string> { ">=", "<=" };
            }
            if (sentenceWord.Contains("teenager"))
            {
                return new List<string> { ">=", "<="};
            }
            if (sentenceWord.Contains("child"))
            {
                return new List<string> { ">=", "<=" };
            }
            if (sentenceWord.Contains("adult"))
            {
                return new List<string> { ">=", "<=" };
            }
            if (sentenceWord.Contains("senior"))
            {
                return new List<string> { ">="};
            }
            if (sentenceWord.Contains("male"))
            {
                return new List<string> { "==" };
            }
            if (sentenceWord.Contains("female"))
            {
                return new List<string> { "==" };
            }
            if (sentenceWord.Contains("above"))
            {
                return new List<string> { ">" };
            }
            if(sentenceWord.Contains("below"))
            {
                return new List<string> { "<" };
            }
            if (sentenceWord.Contains("equal"))
            {
                return new List<string> { "==" };
            }
            if (sentenceWord.Contains("from"))
            {
                return new List<string> { "==" };
            }
            return new List<string> { "" };
        }
        public static string GetCorrespondingLogicalOperator(string sentenceWord)
        {
            sentenceWord = sentenceWord.ToLower().Trim();
            if(sentenceWord == "and")
            {
                return "||";
            }
            if(sentenceWord == "or")
            {
                return "||";
            }
            return "";
        }
        public static List<string> GetCorrespondingRightExpression(string sentenceWord)
        {
            sentenceWord = sentenceWord.ToLower().Trim();
            //var leftResult = GetCorrespondingLeftExpression(sentenceWord);
            //var comparatorResult = new List<string>();
            //var logicalOperatorResult = "";
            var isRightExpression = false;
            if (sentenceWord != "people" && sentenceWord != "above" && sentenceWord != "below"
                && sentenceWord != "from" && sentenceWord != "and")
            {
                isRightExpression = true;
                //comparatorResult = GetCorrespondingComparator(sentenceWord);
                //if(comparatorResult.Contains(""))
                //{
                //    logicalOperatorResult = GetCorrespondingLogicalOperator(sentenceWord);
                //    if(logicalOperatorResult == "")
                //    {
                //        isRightExpression = true;
                //    }

                //}
            }
            if(isRightExpression)
            {
                if (sentenceWord.Contains("young"))
                {
                    return new List<string> { "16", "24" };
                }
                if (sentenceWord.Contains("teenager"))
                {
                    return new List<string> { "13", "19" };
                }
                if (sentenceWord.Contains("child"))
                {
                    return new List<string> { "0", "12" };
                }
                if (sentenceWord.Contains("adult"))
                {
                    return new List<string> { "20", "59" };
                }
                if (sentenceWord.Contains("senior"))
                {
                    return new List<string> { "60" };
                }
                if ((sentenceWord.StartsWith("male") || sentenceWord.StartsWith("female")))
                {
                    if (sentenceWord.EndsWith("s"))
                        sentenceWord = sentenceWord.Substring(0, sentenceWord.Length - 1);
                }
                else
                    sentenceWord = sentenceWord[0].ToString().ToUpper() + sentenceWord.Substring(1);
                if (int.TryParse(sentenceWord, out int result))
                {
                    // It is a number
                    //Console.WriteLine($"Succès : {result}");
                }
                else
                {
                    // It is not a number
                    sentenceWord = $"\"{sentenceWord}\"";
                  
                }
                
                return new List<string> { sentenceWord };
            }
            else
            {
                return new List<string> { "" };
            }
        }
        
    }
}
