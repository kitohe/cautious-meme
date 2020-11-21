using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<IdentityUser>().ToTable("Users", "dbo");
            builder.Entity<IdentityRole>().ToTable("Roles", "dbo");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "dbo");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "dbo");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "dbo");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "dbo");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "dbo");
        }
    }
}
