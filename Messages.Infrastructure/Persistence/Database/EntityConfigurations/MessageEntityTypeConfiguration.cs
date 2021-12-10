using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Messages.Domain.Entities;
using Messages.Domain.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messages.Infrastructure.Persistence.Database.EntityConfigurations;

public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasIndex(m => m.CreatedBy);
        builder.HasIndex(m => m.DoNotSendBefore);
        builder.HasIndex(m => m.CreatedAt);

        builder
            .HasKey(m => m.Id);

        builder.Property(x => x.Id).HasColumnType($"char({MessageId.MAX_LENGTH})");
        builder.Property(x => x.CreatedBy).HasColumnType($"char({IdentityAddress.MAX_LENGTH})");
        builder.Property(x => x.CreatedByDevice).HasColumnType($"char({DeviceId.MAX_LENGTH})");

        builder.Ignore(a => a.Body);
    }
}