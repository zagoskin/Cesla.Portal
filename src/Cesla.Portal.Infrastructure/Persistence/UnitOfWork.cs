using Cesla.Portal.Application.Common.Abstractions;

namespace Cesla.Portal.Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly CeslaDbContext _context;

    public UnitOfWork(CeslaDbContext context)
    {
        _context = context;
    }

    private bool _isUnitOfWorkStarted = false;
    public void StartUnitOfWork()
    {
        _isUnitOfWorkStarted = true;
    }
    public bool IsStarted => _isUnitOfWorkStarted;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
