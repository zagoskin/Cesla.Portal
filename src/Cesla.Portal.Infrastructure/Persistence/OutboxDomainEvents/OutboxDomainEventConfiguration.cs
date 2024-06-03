using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cesla.Portal.Infrastructure.Persistence.OutboxDomainEvents;
internal sealed class OutboxDomainEventConfiguration : IEntityTypeConfiguration<OutboxDomainEvent>
{
    public void Configure(EntityTypeBuilder<OutboxDomainEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Type)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Content)
            .IsRequired();

        builder.Property(e => e.CreatedOnUtc)
            .IsRequired();

        builder.Property(e => e.ProcessedOnUtc);

        builder.Property(e => e.Error);
    }
}
