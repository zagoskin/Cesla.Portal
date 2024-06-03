using Cesla.Portal.Application.Common.Abstractions;

namespace Cesla.Portal.Infrastructure.Persistence;

internal abstract class BaseRepository
{
    protected readonly CeslaDbContext _context;
    protected readonly IUnitOfWork _unitOfWork;
    protected BaseRepository(CeslaDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    protected async Task SaveChangesIfNotUnitOfWorkStartedAsync(CancellationToken token = default)
    {
        if (_unitOfWork.IsStarted)
        {
            return;
        }
        await _unitOfWork.SaveChangesAsync(token);
    }
}
