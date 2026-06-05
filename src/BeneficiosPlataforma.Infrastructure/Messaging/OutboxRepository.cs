namespace BeneficiosPlataforma.Infrastructure.Messaging;

using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

public class OutboxRepository : IOutboxRepository
{
    private readonly AppDbContext _context;

    public OutboxRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _context.OutboxMessages.Add(message);
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages
            .IgnoreQueryFilters()
            .Where(m => m.Status == "PENDING")
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsSentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (message != null)
        {
            message.Status = "SENT";
            message.SentAt = DateTime.UtcNow;
            _context.OutboxMessages.Update(message);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsFailedAsync(Guid id, string error, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (message != null)
        {
            message.Status = "FAILED";
            message.ErrorMessage = error;
            _context.OutboxMessages.Update(message);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task IncrementRetryCountAsync(Guid id, string errorMessage, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (message != null)
        {
            message.RetryCount++;
            message.ErrorMessage = errorMessage;
            _context.OutboxMessages.Update(message);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
