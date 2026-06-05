namespace BeneficiosPlataforma.Application.Messaging;

public interface INotificationService
{
    Task SendPasswordResetEmailAsync(string email, string userName, string resetUrl, CancellationToken cancellationToken);
}
