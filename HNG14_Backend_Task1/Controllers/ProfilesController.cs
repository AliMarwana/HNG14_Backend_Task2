using HNG14_Backend_Task1.Data;
using HNG14_Backend_Task1.DTOs;
using HNG14_Backend_Task1.Models;
using HNG14_Backend_Task1.Utils;
using HNG14_Backend_Task2.DTOs;
using HNG14_Backend_Task2.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic.Core; 
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;


namespace HNG14_Backend_Task1.Controllers
{
    [Route("api/profiles")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }



        private bool FilterCondition(Profile profile, ProfilesParamsDto profilesParamsDto)
        {
            var filterCondition = true;
            if (profilesParamsDto.Gender != null)
                filterCondition = filterCondition && profile.Gender == profilesParamsDto.Gender;
            if (profilesParamsDto.AgeGroup != null)
                filterCondition = filterCondition && profile.AgeGroup == profilesParamsDto.AgeGroup;
            if (profilesParamsDto.CountryId != null)
                filterCondition = filterCondition && profile.CountryId == profilesParamsDto.CountryId;
            if (profilesParamsDto.MinAge != null)
                filterCondition = filterCondition && profile.Age >= profilesParamsDto.MinAge;
            if (profilesParamsDto.MaxAge != null)
                filterCondition = filterCondition && profile.Age <= profilesParamsDto.MaxAge;
            if (profilesParamsDto.MinGenderProbability != null)
                filterCondition = filterCondition && profile.GenderProbability > profilesParamsDto.MinGenderProbability;
            if (profilesParamsDto.MinCountryProbability != null)
                filterCondition = filterCondition && profile.CountryProbability > profilesParamsDto.MinCountryProbability;
            return filterCondition;
        }
        private object? SortingFunction(Profile profile, ProfilesParamsDto profilesParamsDto)
        {
            if (profilesParamsDto.SortBy != null)
            {
                if (profilesParamsDto.SortBy == "age")
                    return profile.Age;
                if (profilesParamsDto.SortBy == "create_at")
                    return profile.CreatedAt;
                if (profilesParamsDto.SortBy == "gender_probability")
                    return profile.GenderProbability;
                else
                    return null;
            }
            return null;
        }


        [HttpGet]
        public async Task<IActionResult> GetProfilesFilteredSorted(ProfilesParamsDto profilesParamsDto)
        {
            var allProfiles = await _context.Profiles.ToListAsync();

            var profilesFiltered = allProfiles.Where(p => FilterCondition(p, profilesParamsDto)).OrderBy(p => p.Age);
            var profilesSorted = profilesFiltered;
            var profilesForPage = new List<Profile>();
            profilesParamsDto.SortBy = profilesParamsDto.SortBy?.ToLower();
            if (profilesParamsDto.SortBy != null)
            {
                if (profilesParamsDto.SortBy != "age"
                && profilesParamsDto.SortBy != "created_at" && profilesParamsDto.SortBy != "gender_probability")
                {
                    return UnprocessableEntity(new ErrorDto
                    {
                        Status = "error",
                        Message = "Can not process such sorting",


                    });
                }
                else
                {
                    if (profilesParamsDto.Order == null || profilesParamsDto.Order == "asc")
                        profilesSorted = profilesFiltered.OrderBy(p => SortingFunction(p, profilesParamsDto));
                    else
                        profilesSorted = profilesFiltered.OrderByDescending(p => SortingFunction(p, profilesParamsDto));
                }
            }
            if (profilesParamsDto.Limit > 50)
            {
                return UnprocessableEntity(new ErrorDto
                {
                    Status = "error",
                    Message = "limit exceeded 50"
                });
            }
            else
            {
                //if (profilesSorted == null || profilesSorted.Count() == 0)
                //{
                //    return NotFound(new ErrorDto { Status = "Not found", Message = "Profile not found" });
                //}
                //else
                //{
                    var profilesNumber = profilesSorted.Count();
                    var firstIndex = (profilesParamsDto.Page - 1) * profilesParamsDto.Limit;
                    var lastIndex = firstIndex + profilesParamsDto.Limit - 1;
                    for (int i = firstIndex; i <= lastIndex; i++)
                    {
                        var profileIndexed = profilesSorted.ElementAtOrDefault(i);
                        if (profileIndexed != null)
                            profilesForPage.Add(profileIndexed);
                    }
                    //if(profilesForPage == null ||  profilesForPage.Count() == 0)
                    //{
                    //    return NotFound(new ErrorDto { Status = "Not found", Message = "Profile not found" });
                    //}
                    //else
                    //{
                    var responseDto = new ResponseDto
                    {
                        Status = "success",
                        Page = profilesParamsDto.Page,
                        Limit = profilesParamsDto.Limit,
                        Total = allProfiles.Count(),
                        Data = profilesForPage
                    };
                    var responseJson = JsonSerializer.Serialize(responseDto);
                    return Ok(responseJson);
                    //}
                //}


            }

            return Ok();
        }

        private List<FilterTerm> AnalyzeQuery(string[] words)
        {
            List<FilterTerm> terms = new List<FilterTerm>();
            foreach (var item in words)
            {
                var leftValue = FilterUtils.GetCorrespondingLeftExpression(item);
                var comparatorValue = FilterUtils.GetCorrespondingComparator(item);
                var logicalOperatorValue = FilterUtils.GetCorrespondingLogicalOperator(item);
                var rightValue = FilterUtils.GetCorrespondingRightExpression(item);
                if (leftValue != "")
                    terms.Add(new FilterTerm { TermType = "LeftExpression", TermValue = leftValue });
                if (!comparatorValue.Contains(""))
                {
                    if(comparatorValue.Count > 1)
                    {
                        terms.Add(new FilterTerm { TermType = "LeftExpression", TermValue = leftValue });
                    }
                    foreach (var comparator in comparatorValue)
                    {
                        //var newComparatorValue = new ComparatorDto { Comparator = comparator };
                        terms.Add(new FilterTerm { TermType = "Comparator", TermValue = comparator });
                    
                    }
                }
                //if (logicalOperatorValue != "")
                //    terms.Add(new FilterTerm { TermType = "LogicalOperator", TermValue = logicalOperatorValue });
                if (!rightValue.Contains(""))
                {
                    foreach (var value in rightValue)
                    {
                        //var newRightValue = new RightExpressionDto { RightExpression = value};
                        terms.Add(new FilterTerm { TermType = "RightExpression", TermValue = value });
                    }
                }
            }
            var termsGrouped = terms.GroupBy(p => p.TermType);
            var indexGroup = 0;


            foreach (var group in termsGrouped)
            {
           
                foreach (var item in group)
                {
                    item.Index = indexGroup;
                    indexGroup++;
                }
                indexGroup = 0;
               
            }
            return terms;
        }
        private List<CriteriaDto> CreateCriteria(List<FilterTerm> filterTerms)
        {
            var criteria = new List<CriteriaDto>();
            var termsGrouped = filterTerms.GroupBy(p => p.Index);
            foreach (var group in termsGrouped)
            {
                var criterion = new CriteriaDto();
                foreach (var item in group)
                {
                    var propertyConcerned = item.TermType;
                    PropertyInfo propertyInfo = criterion.GetType().GetProperty(propertyConcerned);
                    propertyInfo.SetValue(criterion, item.TermValue);
                }
                criteria.Add(criterion);

            }
            //var indexFilterTerms = ;
            //foreach (var filterTerm in filterTerms)
            //{
            //    var latestCriterion = criteria.ElementAtOrDefault(criteria.Count - 1);
            //    if (latestCriterion == null)
            //    {
            //        latestCriterion = new CriteriaDto();
            //        criteria.Add(latestCriterion);
            //    }
            //    if (latestCriterion != null)
            //    {
            //        latestCriterion = criteria.ElementAtOrDefault(criteria.Count - 1);
            //        var criterionConcerned = filterTerm.TermType;
            //        PropertyInfo propertyInfo = latestCriterion.GetType().GetProperty(criterionConcerned);
            //        if (propertyInfo.GetValue(latestCriterion) == null)
            //        {
            //            propertyInfo.SetValue(latestCriterion, filterTerm.TermValue);
            //        }
            //    }

            //}
            return criteria;
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetSearch([FromQuery] string? q = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            try
            {
                string[] words = q.Split(' ');
                var filterTerms = AnalyzeQuery(words);
                var criteria = CreateCriteria(filterTerms);
                var genderCriteriaIndexes = criteria.Select((value, index) => new { value, index })
                .Where(x => x.value.LeftExpression == "Gender")
                .Select(x => x.index)
                .ToList();
                if (genderCriteriaIndexes.Count > 1)
                {
                    foreach (var genderIndex in genderCriteriaIndexes.OrderByDescending(i => i))
                    {
                        criteria.RemoveAt(genderIndex);
                    }

                }
                string query = "";
                int index = 0;
                foreach (var criterion in criteria)
                {
                    var queryCriterion = "";
                    if (index > 0)
                        queryCriterion += " && ";
                    queryCriterion += criterion.LeftExpression + " " + criterion.Comparator +
                    criterion.RightExpression + " " + criterion.LogicalOperator;
                    query += queryCriterion;
                    index++;
                }
                var allProfiles = await _context.Profiles.ToListAsync();
                var filteredProfiles = allProfiles.AsQueryable()
                    .Where(query).ToList();
                var profilesForPage = new List<Profile>();

                var profilesNumber = filteredProfiles.Count();
                var firstIndex = (page - 1) * limit;
                var lastIndex = firstIndex + limit - 1;
                for (int i = firstIndex; i <= lastIndex; i++)
                {
                    var profileIndexed = filteredProfiles.ElementAtOrDefault(i);
                    if (profileIndexed != null)
                        profilesForPage.Add(profileIndexed);
                }
                //if(profilesForPage == null ||  profilesForPage.Count() == 0)
                //{
                //    return NotFound(new ErrorDto { Status = "Not found", Message = "Profile not found" });
                //}
                //else
                //{
                var responseDto = new ResponseDto
                {
                    Status = "success",
                    Page = page,
                    Limit = limit,
                    Total = allProfiles.Count(),
                    Data = profilesForPage
                };
                var responseJson = JsonSerializer.Serialize(responseDto);
                return Ok(responseJson);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(new ErrorDto
                {
                    Status = "error",
                    Message = "Unable to interpret query"
                });
            }

            return Ok();
        }
    }
}
