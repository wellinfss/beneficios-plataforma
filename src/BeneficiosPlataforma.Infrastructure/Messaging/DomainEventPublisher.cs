namespace BeneficiosPlataforma.Infrastructure.Messaging;

using Application.Messaging;
using Domain.Events;
using Domain.Interfaces;
using System.Text.Json;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IOutboxRepository _outboxRepository;

    public DomainEventPublisher(IOutboxRepository outboxRepository)
    {
        _outboxRepository = outboxRepository;
    }

    public async Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            TenantId = domainEvent.TenantId,
            EventType = domainEvent.EventType,
            Payload = JsonSerializer.Serialize(domainEvent),
            Status = "PENDING",
            CreatedAt = DateTime.UtcNow,
            RetryCount = 0
        };

        await _outboxRepository.AddAsync(outboxMessage, cancellationToken);
    }
}
