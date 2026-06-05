namespace BeneficiosPlataforma.Infrastructure.Messaging;

using Application.Messaging;
using MassTransit;

public class EventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventBus(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class
    {
        await _publishEndpoint.Publish(message, cancellationToken);
    }
}
