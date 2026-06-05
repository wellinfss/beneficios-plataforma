namespace BeneficiosPlataforma.Infrastructure.Messaging;

using Application.Messaging;
using Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

public class OutboxDispatcherWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxDispatcherWorker> _logger;
    private readonly int _intervalSeconds = 10;
    private readonly int _batchSize = 50;
    private readonly int _maxRetries = 5;

    public OutboxDispatcherWorker(
        IServiceProvider serviceProvider,
        ILogger<OutboxDispatcherWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxDispatcherWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
            }

            await Task.Delay(TimeSpan.FromSeconds(_intervalSeconds), stoppingToken);
        }

        _logger.LogInformation("OutboxDispatcherWorker stopped");
    }

    private async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        var pendingMessages = await outboxRepository.GetPendingAsync(_batchSize, cancellationToken);

        foreach (var message in pendingMessages)
        {
            try
            {
                var eventType = EventTypeRegistry.GetEventType(message.EventType);
                if (eventType == null)
                {
                    _logger.LogError("Unknown event type {EventType} in outbox message {MessageId}",
                        message.EventType, message.Id);
                    await outboxRepository.MarkAsFailedAsync(message.Id,
                        $"Unknown event type: {message.EventType}", cancellationToken);
                    continue;
                }

                var domainEvent = JsonSerializer.Deserialize(message.Payload, eventType);
                if (domainEvent == null)
                {
                    _logger.LogError("Failed to deserialize outbox message {MessageId} with event type {EventType}",
                        message.Id, message.EventType);
                    await outboxRepository.MarkAsFailedAsync(message.Id,
                        "Failed to deserialize event payload", cancellationToken);
                    continue;
                }

                await PublishEventAsync(eventBus, eventType, domainEvent, cancellationToken);

                await outboxRepository.MarkAsSentAsync(message.Id, cancellationToken);
                _logger.LogInformation("Outbox message {MessageId} sent successfully", message.Id);
            }
            catch (Exception ex)
            {
                var newRetryCount = message.RetryCount + 1;
                _logger.LogWarning(ex, "Error publishing outbox message {MessageId}, retry count: {RetryCount}",
                    message.Id, newRetryCount);

                if (newRetryCount >= _maxRetries)
                {
                    await outboxRepository.MarkAsFailedAsync(message.Id, ex.Message, cancellationToken);
                }
                else
                {
                    await outboxRepository.IncrementRetryCountAsync(message.Id, ex.Message, cancellationToken);
                }
            }
        }
    }

    private async Task PublishEventAsync(IEventBus eventBus, Type eventType, object domainEvent, CancellationToken cancellationToken)
    {
        var method = typeof(IEventBus).GetMethod("PublishAsync")
            ?.MakeGenericMethod(eventType);

        if (method == null)
        {
            throw new InvalidOperationException($"Could not find PublishAsync method for type {eventType.Name}");
        }

        var task = (Task?)method.Invoke(eventBus, new[] { domainEvent, cancellationToken });
        if (task != null)
        {
            await task;
        }
    }
}
