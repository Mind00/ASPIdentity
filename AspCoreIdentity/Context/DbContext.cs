using AspCoreIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Context
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<IdentityUser>(entity => { entity.ToTable(name: "Users");});
            //builder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Roles"); });
            //builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });
            //builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });
            ////builder.Entity<IdentityUserLogin<string>>(entity => { 
            ////    entity.ToTable("UserLogins");
            ////    entity.Property(e => e.LoginProvider);                                                        
            ////});
            //builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });
            //builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRoles> ApplicationRoles { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PhotoGallery> PhotoGalleries { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
    }
}
