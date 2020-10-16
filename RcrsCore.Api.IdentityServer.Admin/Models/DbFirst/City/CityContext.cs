using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RcrsCore.Api.IdentityServer.Admin.Helper;
using RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.City.Entity;

namespace RcrsCore.Api.IdentityServer.Admin.Models.DbFirst.City
{
    public partial class CityContext : DbContext
    {
        public CityContext()
        {
        }

        public CityContext(DbContextOptions<CityContext> options)
            : base(options)
        {
        }

        public virtual DbSet<M_市区町村> M_市区町村s { get; set; }
        public virtual DbSet<M_都道府県> M_都道府県s { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(AppSettingsHelper.App(new string[] { "ConnectionStrings", "DefaultConnection" }));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<M_市区町村>(entity =>
            {
                entity.ToTable("M_市区町村");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.市区町村cd).HasColumnName("市区町村CD");

                entity.Property(e => e.都道府県cd).HasColumnName("都道府県CD");
            });

            modelBuilder.Entity<M_都道府県>(entity =>
            {
                entity.ToTable("M_都道府県");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.都道府県cd).HasColumnName("都道府県CD");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}