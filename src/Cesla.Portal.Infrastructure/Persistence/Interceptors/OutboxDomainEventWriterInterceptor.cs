using System.Text.Json;
using Cesla.Portal.Domain.Common.Abstractions;
using Cesla.Portal.Infrastructure.Persistence.OutboxDomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cesla.Portal.Infrastructure.Persistence.Interceptors;

internal sealed class OutboxDomainEventWriterInterceptor : ISaveChangesInterceptor
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        DispatchDomainEventsAsync(eventData.Context).GetAwaiter().GetResult();

        return result;

    }

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(eventData.Context, cancellationToken);

        return result;
    }

    public static async Task DispatchDomainEventsAsync(DbContext? context, CancellationToken token = default)
    {
        if (context is null || context is not CeslaDbContext ceslaDbContext)
            return;

        var relevantEntities = ceslaDbContext.ChangeTracker.Entries<IHaveDomainEvents>()
           .Where(entry => entry.Entity.DomainEvents.Any())
           .Select(entry => entry.Entity)
           .ToList();

        var domainEvents = relevantEntities
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        foreach (var entity in relevantEntities)
        {
            entity.ClearDomainEvents();
        }


        var outboxDomainEvents = domainEvents.ConvertAll(domainEvent =>
            new OutboxDomainEvent(
                domainEvent.GetType().Name,
                JsonSerializer.Serialize(
                    domainEvent,
                    domainEvent.GetType(),
                    _jsonSerializerOptions)));

        if (outboxDomainEvents.Count is not 0)
        {
            await ceslaDbContext.OutboxDomainEvents.AddRangeAsync(outboxDomainEvents, token);
        }
    }
}
