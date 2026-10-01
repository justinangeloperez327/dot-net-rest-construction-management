using Construction.Application.Common.Messaging;

namespace Construction.Application.Submittals.StartSubmittalReview;

public sealed record StartSubmittalReviewCommand(
    Guid ProjectId,
    Guid SubmittalId) : ICommand<SubmittalResponse>;
