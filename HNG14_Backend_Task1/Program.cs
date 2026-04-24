
using HNG14_Backend_Task1.Data;
using HNG14_Backend_Task2.Utils;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HNG14_Backend_Task1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://*:{port}");
            //// Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
   );
                ;
            builder.Services.AddHttpLogging(logging =>
            {
                // Choisissez les champs à afficher (RequestPath contient l'URL)
                logging.LoggingFields = HttpLoggingFields.RequestMethod |
                                       HttpLoggingFields.RequestPath |
                                       HttpLoggingFields.ResponseStatusCode;
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
            });
            
            
            builder.Services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                    }); ;
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    // THIS IS WHERE YOU SET Access-Control-Allow-Origin: *
                    policy.AllowAnyOrigin()   // <-- Sets the header to "*"
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
            app.UseForwardedHeaders(options);

            // 3. ACTIVATION DES LOGS HTTP (À placer ici pour tout capturer)
            app.UseHttpLogging();
            app.UseForwardedHeaders(options);
            //app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("AllowAll");   // <-- Apply the CORS policy
            app.UseHttpLogging();
            app.MapControllers();
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Cette commande déclenche UseAsyncSeeding (ou UseSeeding)
                await context.Database.MigrateAsync();

                // OU si vous n'utilisez pas les migrations (ex: tests/démo) :
                // await context.Database.EnsureCreatedAsync();
            }
            app.Run();
        }
    }
}
