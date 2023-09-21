using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebUI.Models;

namespace WebUI.ModelConfiguration
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property(x => x.TagName).IsRequired();
            builder.Property(x => x.TagName).HasMaxLength(128);
            Tag tag1 = new()
            {
                TagName = "Life"
            };
            Tag tag2 = new()
            {
                TagName = "War"
            };
            Tag tag3 = new()
            {
                TagName = "Tech"
            };

            builder.HasData(tag1);
            builder.HasData(tag2);
            builder.HasData(tag3);

        }
    }
}
