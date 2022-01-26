﻿using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Messages.Domain.Entities;
using Messages.Domain.Ids;
using Messages.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Messages.Infrastructure.Persistence.Database;

public class ApplicationDbContext : AbstractDbContextBase
{
    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public virtual DbSet<Message> Messages { get; set; }
    public virtual DbSet<RecipientInformation> RecipientInformation { get; set; }
    public virtual DbSet<Relationship> Relationships { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<FileId>().AreUnicode(false).AreFixedLength().HaveMaxLength(FileId.MAX_LENGTH).HaveConversion<FileIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<MessageId>().AreUnicode(false).AreFixedLength().HaveMaxLength(MessageId.MAX_LENGTH).HaveConversion<MessageIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<RelationshipId>().AreUnicode(false).AreFixedLength().HaveMaxLength(RelationshipId.MAX_LENGTH).HaveConversion<RelationshipIdEntityFrameworkValueConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
