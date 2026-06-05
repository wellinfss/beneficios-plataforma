namespace BeneficiosPlataforma.Infrastructure.Messaging;

using Microsoft.Extensions.Logging;
using BeneficiosPlataforma.Application.Messaging;

public class EmailNotificationService(ILogger<EmailNotificationService> logger) : INotificationService
{
    public async Task SendPasswordResetEmailAsync(string email, string userName, string resetUrl, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Sending password reset email to {Email}. Reset URL: {ResetUrl}",
            email, resetUrl);

        await Task.Delay(100, cancellationToken);
    }
}
