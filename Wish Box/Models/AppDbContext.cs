using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Wish> Wishes { get; set; }
        public DbSet<TakenWish> TakenWishes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Following> Followings { get; set; }
        public DbSet<WishRating> WishRatings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Comment>()
            .HasOne(p => p.Wish)
            .WithMany()
            .HasForeignKey("WishId")
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
            .HasOne(p => p.InReply)
            .WithMany()
            .HasForeignKey("InReplyId")
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Wish>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
