using Cesla.Portal.Domain.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cesla.Portal.Infrastructure.Persistence.Interceptors;

internal sealed class LogicDeleteInterceptor : ISaveChangesInterceptor
{
    public InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not CeslaDbContext context)
        {
            return result;
        }                

        var deletedEntries = context.ChangeTracker
            .Entries<ILogicDeletable>()
            .Where(entry => entry.State == EntityState.Deleted);

        foreach (var deleteEntry in deletedEntries)
        {
            deleteEntry.Property<bool>("Deleted").CurrentValue = true;
            deleteEntry.Entity.MarkAsDeleted();
            deleteEntry.State = EntityState.Modified;
        }

        return result;
    }

    public ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return ValueTask.FromResult(SavingChanges(eventData, result));
    }
}
