using Construction.Application.Common.Messaging;

namespace Construction.Application.Rfis.AddRfiComment;

public sealed record AddRfiCommentCommand(
    Guid ProjectId,
    Guid RfiId,
    string Body) : ICommand<RfiCommentResponse>;
