namespace BeneficiosPlataforma.Domain.Interfaces;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(int batchSize, CancellationToken cancellationToken = default);
    Task MarkAsSentAsync(Guid id, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(Guid id, string error, CancellationToken cancellationToken = default);
    Task IncrementRetryCountAsync(Guid id, string errorMessage, CancellationToken cancellationToken = default);
}

public class OutboxMessage
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string EventType { get; init; } = null!;
    public string Payload { get; init; } = null!;
    public string Status { get; init; } = "PENDING";
    public DateTime CreatedAt { get; init; }
    public DateTime? SentAt { get; set; }
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
}
