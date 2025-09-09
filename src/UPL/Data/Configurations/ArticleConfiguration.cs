using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UPL.Domain.Entities;

namespace UPL.Data.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => new { x.CategoryId, x.PublishedAt, x.AuthorId });
        builder.Property(x => x.Title).UseCollation("Vietnamese_100_CI_AI");
        builder.Property(x => x.Slug).UseCollation("Vietnamese_100_CI_AI");
        builder.HasOne(x => x.Category).WithMany(c => c.Articles).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Author).WithMany(u => u.Articles).HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Restrict);
    }
}

