using HNG14_Backend_Task1.Models;
using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HNG14_Backend_Task1.Data
{
    public class ApplicationDbContext : DbContext
    {
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, 
            IWebHostEnvironment env,
            IConfiguration configuration)
            : base(options) 
        {
            _configuration = configuration;
            _env = env;

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>()
                .HasIndex(u => u.Name)
                .IsUnique();
        }
        private static string FindProjectFilePath(string startPath)
        {
            DirectoryInfo current = new DirectoryInfo(startPath);

            // Walk up the directory tree (max 10 levels to prevent infinite loop)
   
            for (int i = 0; i < 10; i++)
            {
                //if (current == null) break;

                //// Look for any .csproj file in the current directory
                //var jsonFiles = current.GetFiles("*.json");
                //if (jsonFiles.Any())
                //{
                //    foreach (var jsonFile in jsonFiles)
                //    {
                //            if(jsonFile.FullName.EndsWith("seed_profiles.json"))
                //            return Path.GetDirectoryName(jsonFile.FullName);
                //    }

                //}

                // Move up one directory
                if (!current.FullName.EndsWith("HNG14_Backend_Task1"))
                    current = current.Parent;
                else
                    return current.FullName;
            }

            return null;
        }
        private static string GetSourceDirectory([CallerFilePath] string sourceFilePath = "")
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            var baseDirectory = FindProjectFilePath(Path.GetDirectoryName(assemblyPath));
            // Path.GetDirectoryName removes the filename, returns just the folder path
            return baseDirectory;

        }
        private List<Profile> GetProfilesForSeed()
        {
            //var baseDirectory = GetSourceDirectory();
            var baseDirectory =  _env.ContentRootPath;
            //baseDirectory = Path.GetDirectoryName(baseDirectory);
            var seedProfilesPath = Path.Combine(baseDirectory, "seed_profiles.json");
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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var profiles = GetProfilesForSeed();
            optionsBuilder
                .UseNpgsql(_configuration.GetConnectionString("DefaultConnection"))
                .UseSeeding((context, _) =>
                {
                    // Logique synchrone (utilisée par 'dotnet ef database update')
                    if (!context.Set<Profile>().Any())
                    {
                        context.Set<Profile>().AddRange(profiles);
                        context.SaveChanges();
                    }
                    
                })
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    // Logique asynchrone (utilisée au démarrage de l'app via MigrateAsync)
                    if (!context.Set<Profile>().Any())
                    {
                        context.Set<Profile>().AddRange(profiles);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                    
                });
        }
        public DbSet<Profile> Profiles { get; set; }
    }
}
