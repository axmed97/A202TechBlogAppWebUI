using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebUI.Data;
using WebUI.Models;

namespace WebUI.ModelConfiguration
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {

        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            //builder.Property(x => x.TagName).IsRequired();
            builder.Property(x => x.TagName).IsRequired()
            .HasMaxLength(50);

        }
    }
}
