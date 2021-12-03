using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Messages.Domain.Entities;
using Messages.Domain.Ids;
using Messages.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Messages.Infrastructure.Persistence.Database
{
    public class ApplicationDbContext : AbstractDbContextBase
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<RecipientInformation> RecipientInformation { get; set; }
        public virtual DbSet<Relationship> Relationships { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.UseValueConverter(
                new MessageIdEntityFrameworkValueConverter(new ConverterMappingHints(MessageId.MAX_LENGTH)));
            builder.UseValueConverter(
                new FileIdEntityFrameworkValueConverter(new ConverterMappingHints(FileId.MAX_LENGTH)));
            builder.UseValueConverter(
                new RelationshipIdEntityFrameworkValueConverter(new ConverterMappingHints(RelationshipId.MAX_LENGTH)));

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
