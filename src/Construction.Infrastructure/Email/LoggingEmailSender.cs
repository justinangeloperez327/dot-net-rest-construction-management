using Construction.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Construction.Infrastructure.Email;

public sealed class LoggingEmailSender(
    ILogger<LoggingEmailSender> logger)
    : IEmailSender
{
    private static readonly Action<ILogger, int, string, Exception?>
        EmailDeliveryInvoked = LoggerMessage.Define<int, string>(
            LogLevel.Information,
            new EventId(1, nameof(EmailDeliveryInvoked)),
            "Email delivery adapter invoked for {RecipientCount} recipient(s) with subject {Subject}.");

    public Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        cancellationToken.ThrowIfCancellationRequested();

        EmailDeliveryInvoked(
            logger,
            message.Recipients.Count,
            message.Subject,
            null);

        return Task.CompletedTask;
    }
}
