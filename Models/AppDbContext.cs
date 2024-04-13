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
            modelBuilder.Entity<UsersRole>(options=>{
                options.HasKey(u=>new{u.UsersId, u.RoleId});
            });
            modelBuilder.Entity<UsersCourse>(options=>{
                options.HasKey(u=>new{u.UsersId, u.CourseId});
            });
        }
        public DbSet<Course> courses{set;get;}
        public DbSet<Lesson>lessons{set;get;}
        public DbSet<Chapter> chapters{set;get;}

        public DbSet<Users> users{set;get;}
        public DbSet<UsersRole>usersRoles{set;get;}
        public DbSet<Role> roles{set;get;}
        
        public DbSet<UsersCourse> usersCourses{set;get;}
        public DbSet<Category> categories{set;get;}

        public DbSet<Test> tests{set;get;}
        public DbSet<Question> questions{set;get;}
        public DbSet<Answer> answers{set;get;}
        public DbSet<Order> orders{set;get;}

       
    }
}