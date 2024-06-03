namespace Cesla.Portal.Application.Common.Abstractions;
public interface IUnitOfWork
{
    void StartUnitOfWork();
    bool IsStarted { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
