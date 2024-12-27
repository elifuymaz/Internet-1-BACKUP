using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebUı.Models
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions options) : base (options)
        {
            
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<House> Houses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            var adminRoleId = Guid.NewGuid().ToString();
            var adminUserId = Guid.NewGuid().ToString();

            // Role seed
            builder.Entity<AppRole>().HasData(
                                new AppRole
                                {
                                    Id = adminRoleId, // Statik Role ID
                                    Name = "Admin",
                                    NormalizedName = "ADMIN"
                                });

            builder.Entity<AppRole>().HasData(
                               new AppRole
                               {
                                   Id = Guid.NewGuid().ToString(),
                                   Name = "User",
                                   NormalizedName = "USER"
                               });

         

            builder.Entity<AppUser>().HasData(
             new AppUser
             {
                 Id = adminUserId, // Statik User ID
                 UserName = "admin",
                 NormalizedUserName = "ADMIN",
                 Email = "admin@admin.com",
                 NormalizedEmail = "ADMIN@ADMIN.COM",
                 EmailConfirmed = true,
                 PasswordHash = new PasswordHasher<AppUser>().HashPassword(null, "Admin123!")
             });

            builder.Entity<IdentityUserRole<string>>().HasData(
               new IdentityUserRole<string>
               {
                   UserId = adminUserId, // Yukarıda oluşturulan sabit User ID
                   RoleId = adminRoleId  // Yukarıda oluşturulan sabit Role ID
               });

            // İl-İlçe ilişkisi
            builder.Entity<District>()
                .HasOne(d => d.City)
                .WithMany(c => c.Districts)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ev - İl ilişkisi
            builder.Entity<House>()
                .HasOne(h => h.City)
                .WithMany()
                .HasForeignKey(h => h.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ev - İlçe ilişkisi
            builder.Entity<House>()
                .HasOne(h => h.District)
                .WithMany()
                .HasForeignKey(h => h.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ev - Kullanıcı ilişkisi
            builder.Entity<House>()
                .HasOne(h => h.CreatedBy)
                .WithMany()
                .HasForeignKey(h => h.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);

        }
    }
}
