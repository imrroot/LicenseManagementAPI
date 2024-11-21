using LicenseManagementAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LicenseManagementAPI.Infrastructure.Data
{

    public class LicenseDbContext : DbContext
    {
        public LicenseDbContext(DbContextOptions<LicenseDbContext> options) : base(options) { }

        // DbSet properties for all entities
        public DbSet<User> Users { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired();
            });

            // App entity configuration
            modelBuilder.Entity<App>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Secret).IsRequired();
                entity.Property(a => a.AppKey).IsRequired();
                entity.HasOne(a => a.User)
                    .WithMany(u => u.Apps)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(a => a.Subscriptions)
                    .WithOne(s => s.App)
                    .HasForeignKey(s => s.AppId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // License entity configuration
            modelBuilder.Entity<License>(entity =>
            {
                entity.HasKey(l => l.LicenseId);
                entity.Property(l => l.Pattern).IsRequired().HasMaxLength(50);
                entity.Property(l => l.Status).IsRequired();
                entity.Property(l => l.CreationDate).IsRequired();
                entity.Property(l => l.ExpiryDate).IsRequired();
                entity.HasOne(l => l.App)
                    .WithMany(a => a.Licenses)
                    .HasForeignKey(l => l.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Subscription entity configuration
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(50);
                entity.Property(s => s.Level).IsRequired();
                entity.HasOne(s => s.App)
                    .WithMany(a => a.Subscriptions)
                    .HasForeignKey(s => s.AppId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(s => new { s.Name, s.AppId }).IsUnique();
            });
        }
    }

}
