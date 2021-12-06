using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Messages.Domain.Entities;
using Messages.Domain.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messages.Infrastructure.Persistence.Database.EntityConfigurations
{
    public class RelationshipEntityTypeConfiguration : IEntityTypeConfiguration<Relationship>
    {
        public void Configure(EntityTypeBuilder<Relationship> builder)
        {
            builder.HasKey(r => r.Id);

            builder.ToTable(nameof(Relationship) + "s", "Relationships");

            builder.Property(x => x.Id).HasColumnType($"char({RelationshipId.MAX_LENGTH})");
            builder.Property(x => x.From).HasColumnType($"char({IdentityAddress.MAX_LENGTH})");
            builder.Property(x => x.To).HasColumnType($"char({IdentityAddress.MAX_LENGTH})");
        }
    }
}
