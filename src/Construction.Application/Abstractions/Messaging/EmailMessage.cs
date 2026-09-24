namespace Construction.Application.Abstractions.Messaging;

public sealed record EmailMessage(
    IReadOnlyCollection<string> Recipients,
    string Subject,
    string Body,
    bool IsHtml = false);
