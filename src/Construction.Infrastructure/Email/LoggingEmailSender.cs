using Construction.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Construction.Infrastructure.Email;

public sealed class LoggingEmailSender(
    ILogger<LoggingEmailSender> logger)
    : IEmailSender
{
    public Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogInformation(
            "Email delivery adapter invoked for {RecipientCount} recipient(s) with subject {Subject}.",
            message.Recipients.Count,
            message.Subject);

        return Task.CompletedTask;
    }
}
