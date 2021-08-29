using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data
{
    // public class DataContext : DbContext
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>> // Identity buoc 1 ,ke thua DBcontext voi Identity
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
       // public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; } // se sinh 1 bang ten Like"s"
        public DbSet<Message> Messages { get; set; }
        public DbSet<Connection> Connections {get; set;}
        public DbSet<Group> Groups { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
              base.OnModelCreating(builder);
               
               
               //====== Identity Xay dung moi quan he AppUser và AppRole voi AppUserRole  ===================
               builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

               builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
               //===================================
            
            builder.Entity<UserLike>()
                   .HasKey(k => new { k.SourceUserId, k.LikedUserId });
            builder.Entity<UserLike>()
                   .HasOne(s => s.SourceUser) // 1 nguoi co the like cho nhieu 
                   .WithMany(l => l.LikedUsers) // day la 1 collection
                   .HasForeignKey(s => s.SourceUserId) // nguoi da login va like
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
                  .HasOne(s => s.LikedUser) // 1 user co the duco like nhieu lan
                  .WithMany(l => l.LikedByUsers) // day la 1 collection
                  .HasForeignKey(s => s.LikedUserId) // nguoi da login va like
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                   .HasOne(r => r.Recipient)                   
                   .WithMany(m => m.MessageReceived)
                   .OnDelete(DeleteBehavior.Restrict); // khi Recient xoa thi Sender van ko xoa
            builder.Entity<Message>()
                   .HasOne(s => s.Sender)
                   .WithMany(m => m.MessageSent)
                   .OnDelete(DeleteBehavior.Restrict);
            // cover datetime thanh datetime UTC
            builder.ApplyUtcDateTimeConverter();
        }
    }

public static class UtcDateAnnotation
{
  private const String IsUtcAnnotation = "IsUtc";
  private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
    new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

  private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter =
    new ValueConverter<DateTime?, DateTime?>(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

  public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, Boolean isUtc = true) =>
    builder.HasAnnotation(IsUtcAnnotation, isUtc);

  public static Boolean IsUtc(this IMutableProperty property) =>
    ((Boolean?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;

  /// <summary>
  /// Make sure this is called after configuring all your entities.
  /// </summary>
  public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
  {
    foreach (var entityType in builder.Model.GetEntityTypes())
    {
      foreach (var property in entityType.GetProperties())
      {
        if (!property.IsUtc())
        {
          continue;
        }

        if (property.ClrType == typeof(DateTime))
        {
          property.SetValueConverter(UtcConverter);
        }

        if (property.ClrType == typeof(DateTime?))
        {
          property.SetValueConverter(UtcNullableConverter);
        }
      }
    }
  }
}


}