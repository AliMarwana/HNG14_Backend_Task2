using HNG14_Backend_Task1.Data;
using HNG14_Backend_Task1.Models;
using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HNG14_Backend_Task2.Utils
{
    public class SeedUtils
    {

            public static void SeedData(DbContext context)
        {
            var data = GetProfilesForSeed();
            context.Set<Profile>().AddRange(data);

        }

        
        public static List<Profile> GetProfilesForSeed()
            {
            //var baseDirectory = GetSourceDirectory();
                var seedProfilesPath = Path.Combine("", "SeedProfiles.json");
                var jsonString = string.Empty;
                var jsonProfilesString = string.Empty;
                using (var fs = new FileStream(seedProfilesPath,
                                FileMode.Open,
                                FileAccess.Read,
                                FileShare.ReadWrite))
                {
                    // Read the file content
                    using (var reader = new StreamReader(fs))
                    {
                        jsonString = reader.ReadToEnd();
                        
                        using (JsonDocument doc = JsonDocument.Parse(jsonString))
                        {
                            // On récupère l'élément "profiles"
                            JsonElement profilesElement = doc.RootElement.GetProperty("profiles");
    
                            // On le convertit en chaîne JSON
                            jsonProfilesString = profilesElement.GetRawText();
                        }
                    }
                }
                var settings = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.Preserve,
                };
                var profilesData = JsonSerializer.Deserialize<List<Profile>>(jsonProfilesString, settings);
                return profilesData;
        }
    }
}
