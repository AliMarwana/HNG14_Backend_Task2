using HNG14_Backend_Task1.Data;
using HNG14_Backend_Task1.DTOs;
using HNG14_Backend_Task1.Models;
using HNG14_Backend_Task1.Utils;
using HNG14_Backend_Task2.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            if(profilesParamsDto.Gender != null)
                filterCondition = filterCondition && profile.Gender == profilesParamsDto.Gender;
            if(profilesParamsDto.CountryId != null)
                filterCondition = filterCondition && profile.CountryId == profilesParamsDto.CountryId;
            if(profilesParamsDto.MinAge != null)
                filterCondition = filterCondition && profile.Age >= profilesParamsDto.MinAge;
            if(profilesParamsDto.MaxAge != null)
                filterCondition = filterCondition && profile.Age <= profilesParamsDto.MaxAge;
            if(profilesParamsDto.MinGenderProbability != null)
                filterCondition = filterCondition && profile.GenderProbability >= profilesParamsDto.MinGenderProbability;
            if(profilesParamsDto.MinCountryProbability != null)
                filterCondition = filterCondition && profile.CountryProbability >= profilesParamsDto.MinCountryProbability;
            return filterCondition;
        }
        private object? SortingFunction(Profile profile, ProfilesParamsDto profilesParamsDto)
        {
            if(profilesParamsDto.SortBy != null)
            {
                if(profilesParamsDto.SortBy == "age")
                    return profile.Age;
                if(profilesParamsDto.SortBy == "create_at")
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
                && profilesParamsDto.SortBy != "create_at" && profilesParamsDto.SortBy != "gender_probability")
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
            if(profilesParamsDto.Limit > 50)
            {
                return UnprocessableEntity(new ErrorDto
                {
                    Status = "error",
                    Message = "limit exceeded 50",


                });
            }
            else
            {
                if (profilesSorted == null || profilesSorted.Count()  == 0)
                {
                    return NotFound(new ErrorDto { Status = "Not found", Message = "Profile not found" });
                }
                else
                {
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
                            Total = profilesSorted.Count(),
                            Data = profilesForPage
                        };
                        var responseJson = JsonSerializer.Serialize(responseDto);
                        return Ok(responseJson);
                    //}
                }
                   

            }

                return Ok();
        }


    }
}
