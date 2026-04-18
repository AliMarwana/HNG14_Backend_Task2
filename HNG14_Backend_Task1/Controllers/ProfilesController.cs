using HNG14_Backend_Task1.Data;
using HNG14_Backend_Task1.DTOs;
using HNG14_Backend_Task1.Models;
using HNG14_Backend_Task1.Utils;
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

        private async Task<ResponseDto> GenderizeRequest(string name)
        {
            using var client = new HttpClient();
            string url = $"https://api.genderize.io/?name={name}";
            // Gets the response body as a string directly
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                //responseBody = JsonSerializer.Deserialize<string>(responseBody); 
                var genderDto = JsonSerializer.Deserialize<GenderDto>(responseBody);
                if (genderDto.Count == 0)
                {
                    return new ResponseDto { Code = 502, Status = "error", Message = "Genderize returned an invalid response" };
                }
                else
                {
                    return new ResponseDto { Code = 200, Data = genderDto };
                }
            }
            else
            {
                return new ResponseDto { Code = 502, Status = "error", Message = "Upstream or server failure" };
            }
        }
        private async Task<ResponseDto> AgeRequest(string name)
        {
            using var client = new HttpClient();
            string url = $"https://api.agify.io/?name={name}";
            // Gets the response body as a string directly
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var ageDto = JsonSerializer.Deserialize<AgeDto>(responseBody);
                if (ageDto.Count == 0)
                {
                    return new ResponseDto { Code = 502, Status = "error", Message = "Agify returned an invalid response" };
                }
                else
                {
                    return new ResponseDto { Code = 200, Data = ageDto };
                }
            }
            else
            {
                return new ResponseDto { Code = 502, Status = "error", Message = "Upstream or server failure" };
            }
        }
        private async Task<ResponseDto> NationalizeRequest(string name)
        {
            using var client = new HttpClient();
            string url = $"https://api.nationalize.io?name={name}";
            // Gets the response body as a string directly
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                NationalizeDto nationalizeDto = JsonSerializer.Deserialize<NationalizeDto>(responseBody);

                if (nationalizeDto.Country == null || nationalizeDto.Country.Count == 0)
                {
                    return new ResponseDto { Code = 502, Status = "error", Message = " returned an invalid response" };
                }
                else
                {
                    return new ResponseDto { Code = 200, Data = nationalizeDto };
                }
            }
            else
            {
                return new ResponseDto { Code = 502, Status = "error", Message = "Upstream or server failure" };
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddProfile([FromBody] NameRequest nameRequest)
        {
            string name = nameRequest.Name;
            name = name.ToLower();
            if (String.IsNullOrEmpty(name))
            {
                return BadRequest(new ErrorDto { Status = "error", Message = "Missing or empty name parameter" });
            }
            else if (name.GetType() != typeof(string))
            {
                return UnprocessableEntity(new ErrorDto
                {
                    Status = "error",
                    Message = "name is not a string",


                });
            }
            else
            {
                //name = name.Trim().ToLower();
                var profileCorresponding = await _context.Profiles.FirstOrDefaultAsync(p => p.Name == name);
                if (profileCorresponding == null)
                {
                    var profile = new Profile();
                    profile.Id = Guid.NewGuid();
                    profile.Name = name;
                    var isInvalidResponse = false;
                    var genderResponse = await GenderizeRequest(name);
                    if (genderResponse.Code == 200)
                    {
                        
                        profile.Gender = (string)ReflectionUtils.GetPropertyValue(genderResponse.Data, "Gender");
                        profile.GenderProbability = (float)ReflectionUtils.GetPropertyValue(genderResponse.Data, "Probability");
                        profile.SampleSize = (int)ReflectionUtils.GetPropertyValue(genderResponse.Data, "Count");
                        var ageResponse = await AgeRequest(name);
                        if (ageResponse.Code == 200)
                        {
                            profile.Age = (int)ReflectionUtils.GetPropertyValue(ageResponse.Data, "Age");
                            profile.AgeGroup = AgeUtils.GetAgeGroup(profile.Age);
                            var nationalizeResponse = await NationalizeRequest(name);
                            if(nationalizeResponse.Code == 200)
                            {
                                var convenientCountry = NationalityUtils.GetConvenientCountry((NationalizeDto)nationalizeResponse.Data);
                                profile.CountryId = convenientCountry.CountryId;
                                profile.CountryProbability = convenientCountry.Probability;
                                profile.CreatedAt = DateTime.UtcNow;
                                await _context.Profiles.AddAsync(profile);
                                await _context.SaveChangesAsync();
                                var response = new ResponseDto
                                {
                                    Status = "success",
                                    Data = profile
                                };
                                var responseJson = JsonSerializer.Serialize(response);
                                return CreatedAtAction("AddProfile", new { id = profile.Id }, responseJson);

                            }
                            else
                            {
                                genderResponse.Data = null;
                                genderResponse.Count = null;
                                return StatusCode(502, nationalizeResponse);
                            }

                            
                        }
                        else
                        {
                            genderResponse.Data = null;
                            genderResponse.Count = null;
                            return StatusCode(502, genderResponse);
                        }
                       
                    }
                    else
                    {
                        genderResponse.Data = null;
                        genderResponse.Count = null;
                        return StatusCode(502, genderResponse);
                    }


                }
                else
                {
                   
                    var successResponse = new ResponseDto
                    {
                        Status = "success",
                        Message = "Profile already exists",
                        Data = profileCorresponding
                    };
                    var successResponseJson = JsonSerializer.Serialize(successResponse);
                    return Ok(successResponse);
                    //return CreatedAtAction("AddProfile", new { id = profileCorresponding.Id }, successResponse);
                }
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            var profileCorresponding = await _context.Profiles.FirstOrDefaultAsync(p  => p.Id == id);
            if(profileCorresponding == null)
            {
                return NotFound(new ErrorDto { Status = "Not found", Message = "Profile not found" });
            }
            else
            {
                var successResponse = new ResponseDto
                {
                    Status = "success",
                    Data = profileCorresponding
                };
                var successResponseJson = JsonSerializer.Serialize(successResponse);
                return Ok(successResponseJson);
            }
        }

        private bool FilterCondition(Profile profile, string? gender = null, string? country_id = null,
        string? age_group = null)
        {
            var filterCondition = true;
            if (gender != null)
            {
                filterCondition = filterCondition && profile.Gender.ToLower() == gender.ToLower();
            }
            if (country_id != null)
            {
                filterCondition = filterCondition && profile.CountryId.ToLower() == country_id.ToLower();
            }
            if (age_group != null)
            {
                filterCondition = filterCondition && profile.AgeGroup.ToLower() == age_group.ToLower();
            }
            return filterCondition;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfiles([FromQuery] string? gender = null,
         [FromQuery] string? country_id = null,
         [FromQuery] string? age_group = null)
        {
            var allProfiles = await _context.Profiles.ToListAsync();
            var profilesFiltered = allProfiles.Where(p => FilterCondition(p, gender, country_id, age_group));
            if(profilesFiltered == null ||  profilesFiltered.Count() == 0)
            {
                return NotFound(new ErrorDto { Status = "Not found", Message = "Profile not found" });
            }
            else
            {
                profilesFiltered = profilesFiltered.Select(p =>
                {
                    p.GenderProbability = null;
                    p.SampleSize = null;
                    p.CountryProbability = null;
                    p.CreatedAt = null;
                    return p;
                }).ToList();    
                var successResponse = new ResponseDto { Status = "success", Count = profilesFiltered.Count(), Data = profilesFiltered };
                var successResponseJson = JsonSerializer.Serialize(successResponse);
                return Ok(successResponseJson);
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(Guid id)
        {
            var profileCorresponding = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == id);
            if (profileCorresponding == null)
            {
                return NotFound(new ErrorDto { Status = "Not found", Message = "Profile not found" });
            }
            else
            {
                _context.Profiles.Remove(profileCorresponding);
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}
