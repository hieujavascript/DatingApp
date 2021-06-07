using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppUser> User { get; set; }
         public DbSet<UserLike> Likes { get; set; } // se sinh 1 bang ten Like"s"
        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder.Entity<UserLike>()
                   .HasKey(k => new {k.SourceUserId , k.LikedUserId});
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
        }
    }
}