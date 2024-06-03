using System.Reflection;
using System.Text.Json;
using Cesla.Portal.Domain.Common.Abstractions;
using Cesla.Portal.Infrastructure.Persistence;
using Cesla.Portal.Infrastructure.Persistence.OutboxDomainEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

namespace Cesla.Portal.Infrastructure.BackgroundServices;
internal sealed class OutboxDomainEventPublisher : BackgroundService
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxDomainEventPublisher> _logger;
    private readonly TimeSpan _period;
    private readonly AsyncRetryPolicy _retryOnExceptionPolicy;
    public OutboxDomainEventPublisher(
        ILogger<OutboxDomainEventPublisher> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _period = TimeSpan.FromSeconds(3);
        _serviceScopeFactory = serviceScopeFactory;
        _retryOnExceptionPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(2), 3), (exception, timeSpan, retryCount, context) =>
            {
                _logger.LogWarning(exception, "Exception occurred while publishing domain event. Retrying in {TimeSpan} seconds. Retry attempt {RetryCount}", timeSpan.TotalSeconds, retryCount);
            });

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(_period);
        while (!stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await PublishDomainEventsFromDatabaseAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while publishing domain event");
            }
        }
    }

    private async Task PublishDomainEventsFromDatabaseAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Checking if any domain events.");
        List<OutboxDomainEvent> outboxDomainEvents = await ReadRequestedDomainEventsAsync(stoppingToken);
        if (outboxDomainEvents.Count == 0)
        {
            _logger.LogInformation("No domain events found.");
            return;
        }
        _logger.LogInformation("Read a total of {NumEvents} outbox domain events", outboxDomainEvents.Count);
        foreach (var outboxDomainEvent in outboxDomainEvents)
        {
            _logger.LogInformation("Processing outbox domain event with Id: {Id}", outboxDomainEvent.Id);
            await DeserializeAndPublishDomainEventAsync(outboxDomainEvent, stoppingToken);
        }
    }

    private async Task DeserializeAndPublishDomainEventAsync(OutboxDomainEvent outboxDomainEvent, CancellationToken stoppingToken)
    {
        var domainAssembly = Assembly.GetAssembly(typeof(IDomainEvent));
        var type = domainAssembly?.GetTypes().FirstOrDefault(t => t.Name == outboxDomainEvent.Type);
        if (type is null)
        {
            _logger.LogWarning("Unknown Type {Type}", outboxDomainEvent.Type);
            return;
        }

        var domainEvent = (IDomainEvent)JsonSerializer.Deserialize(
            outboxDomainEvent.Content,
            type,
            _jsonSerializerOptions)!;

        await PerformScopedPublishAndSaveResultsAsync(outboxDomainEvent, domainEvent, stoppingToken);
    }

    private async Task PerformScopedPublishAndSaveResultsAsync(OutboxDomainEvent outboxDomainEvent, IDomainEvent domainEvent, CancellationToken stoppingToken)
    {
        _logger.LogInformation("Publishing domain event of Type {Type}. Data: {DomainEvent}", outboxDomainEvent.Type, outboxDomainEvent.Content);
        using var eventScope = _serviceScopeFactory.CreateScope();
        var publisher = eventScope.ServiceProvider.GetRequiredService<IPublisher>();
        var dbContext = eventScope.ServiceProvider.GetRequiredService<CeslaDbContext>();

        // se existe algum handler para o evento, ele será executado
        // se não existir, o evento será ignorado (não da erro de execução)
        var result = await _retryOnExceptionPolicy.ExecuteAndCaptureAsync(() =>
            publisher.Publish(domainEvent, stoppingToken)
        );


        outboxDomainEvent.MarkAsProcessed(result.FinalException?.ToString());
        dbContext.OutboxDomainEvents.Update(outboxDomainEvent);        
        await dbContext.SaveChangesAsync(stoppingToken);
    }

    private async Task<List<OutboxDomainEvent>> ReadRequestedDomainEventsAsync(CancellationToken token)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CeslaDbContext>();

        return await dbContext.OutboxDomainEvents
            .AsNoTracking()
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.CreatedOnUtc)
            .ToListAsync(token);
    }
}
