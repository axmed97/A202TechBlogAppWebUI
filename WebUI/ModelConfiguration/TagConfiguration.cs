using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebUI.Data;
using WebUI.Models;

namespace WebUI.ModelConfiguration
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        private readonly AppDbContext _context;

        public TagConfiguration(AppDbContext context)
        {
            _context = context;
        }

        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            //builder.Property(x => x.TagName).IsRequired();
            builder.Property(x => x.TagName).HasMaxLength(128);
            Tag tag1 = new()
            {
                Id = 1,
                TagName = "Life"
            };
            Tag tag2 = new()
            {
                Id = 2,
                TagName = "War"
            };
            Tag tag3 = new()
            {
                Id = 3,
                TagName = "Tech"
            };

            

        }
    }
}
