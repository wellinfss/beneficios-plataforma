namespace BeneficiosPlataforma.Application.Messaging;

using Domain.Events;

public interface IDomainEventPublisher
{
    Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}
