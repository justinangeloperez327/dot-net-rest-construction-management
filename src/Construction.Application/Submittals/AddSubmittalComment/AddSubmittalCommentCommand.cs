using Construction.Application.Common.Messaging;

namespace Construction.Application.Submittals.AddSubmittalComment;

public sealed record AddSubmittalCommentCommand(
    Guid ProjectId,
    Guid SubmittalId,
    string Body) : ICommand<SubmittalCommentResponse>;
