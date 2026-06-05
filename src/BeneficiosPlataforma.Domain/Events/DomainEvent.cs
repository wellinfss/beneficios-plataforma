namespace BeneficiosPlataforma.Domain.Events;

public abstract record DomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid TenantId { get; init; }
    public string EventType { get; init; } = null!;
}
