using CVProject.Models.Repository.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CVProject.Models.Repository
{
    public class CvContext : IdentityDbContext<Profile>
    {
        public CvContext(DbContextOptions<CvContext> options) : base(options) { }
		public DbSet<Message> Messages { get; set; }

        public DbSet<GuestMessage> GuestMessages { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<ProfileProject> ProfileProject { get; set; }

        public DbSet<CV_Project> CVProject { get; set; }
        public DbSet<Images> Images { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Skapar en gemensam primärnyckel för CV_Project tabellen
            modelBuilder.Entity<ProfileProject>()
                .HasKey(pp => new { pp.Profileid, pp.Projectid });
            

            modelBuilder.Entity<CV_Project>()
                .HasKey(pp => new { pp.Cvid, pp.Projectid });

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }
    }
}
