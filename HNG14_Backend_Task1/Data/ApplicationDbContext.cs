using HNG14_Backend_Task1.Models;
using Microsoft.EntityFrameworkCore;

namespace HNG14_Backend_Task1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Profile> Profiles { get; set; }
    }
}
