using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        }
    }
}