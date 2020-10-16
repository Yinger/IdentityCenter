using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RcrsCore.IdentityServer.Dto.DomainModel.Application;

namespace RcrsCore.IdentityServer.Models
{
    //---------------------------------------------------------------
    /// <summary>
    /// Entity Framework database context used for identity
    /// </summary>
    //---------------------------------------------------------------
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        //---------------------------------------------------------------
        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="options"></param>
        //---------------------------------------------------------------
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //---------------------------------------------------------------
        /// <summary>
        /// テーブル作成時に自動実行します。
        /// </summary>
        /// <param name="builder"></param>
        //---------------------------------------------------------------
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // foreign key を設定します。
            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            //// rename the ASP.NET Identity table names
            //builder.Entity<ApplicationRole>()
            //    .ToTable("M_Role");

            //builder.Entity<ApplicationUser>()
            //    .ToTable("M_User");

            //builder.Entity<ApplicationUserRole>()
            //    .ToTable("T_UserRole");
        }
    }
}