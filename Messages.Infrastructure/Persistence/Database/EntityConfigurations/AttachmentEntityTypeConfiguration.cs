using Messages.Domain.Entities;
using Messages.Domain.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class AttachmentEntityTypeConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable($"{nameof(Attachment)}s");

        builder
            .HasKey(m => new {m.Id, m.MessageId});

        builder.Property(x => x.Id).HasColumnType($"char({FileId.MAX_LENGTH})");
        builder.Property(x => x.MessageId).HasColumnType($"char({MessageId.MAX_LENGTH})");
    }
}
