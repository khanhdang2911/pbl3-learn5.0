using Microsoft.EntityFrameworkCore;

namespace PBL3_Course.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
                base.OnConfiguring(builder);
        } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Course> courses{set;get;}
        public DbSet<Lesson>lessons{set;get;}

       
    }
}