
using HNG14_Backend_Task1.Data;
using HNG14_Backend_Task2.Utils;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HNG14_Backend_Task1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var port = environment.getenvironmentvariable("port") ?? "8080";
            builder.webhost.useurls($"http://*:{port}");
            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
   );
                ;

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

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("AllowAll");   // <-- Apply the CORS policy

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
