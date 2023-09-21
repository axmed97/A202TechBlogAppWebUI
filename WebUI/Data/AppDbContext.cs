using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebUI.Models;

namespace WebUI.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleComment> ArticleComments { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            builder.Entity<User>().ToTable("Users"); // AspNetUser
            builder.Entity<IdentityRole>().ToTable("Roles"); // AspNetRole

            builder.Entity<ArticleComment>()
                    .HasOne(ac => ac.User)
                    .WithMany(u => u.ArticleComments)
                    .HasForeignKey(ac => ac.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tag>()
                .HasData(
                new Tag()
                {
                    Id = 1,
                    TagName = "Life"
                },
                new Tag()
                {
                    Id = 2,
                    TagName = "War"
                });
        }
    }
}
