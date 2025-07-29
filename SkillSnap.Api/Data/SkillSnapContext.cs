using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Data
{
    public class SkillSnapContext : IdentityDbContext<ApplicationUser>
    {
        public SkillSnapContext(DbContextOptions<SkillSnapContext> options)
            : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<PortfolioUser> PortfolioUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ PortfolioUser config
            builder.Entity<PortfolioUser>(entity =>
            {
                entity.Property(u => u.Name)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(u => u.Bio)
                      .HasMaxLength(500);
            });

            // ✅ Project config
            builder.Entity<Project>(entity =>
            {
                entity.Property(p => p.Title)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(p => p.Description)
                      .HasMaxLength(300);

                entity.HasOne(p => p.PortfolioUser)  // Навигация
                      .WithMany(u => u.Projects)
                      .HasForeignKey(p => p.PortfolioUserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ✅ Skill config
            builder.Entity<Skill>(entity =>
            {
                entity.Property(s => s.Name)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasOne(s => s.PortfolioUser)
                      .WithMany(u => u.Skills)
                      .HasForeignKey(s => s.PortfolioUserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

