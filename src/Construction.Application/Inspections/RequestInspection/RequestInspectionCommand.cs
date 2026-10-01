using Construction.Application.Common.Messaging;

namespace Construction.Application.Inspections.RequestInspection;

public sealed record RequestInspectionCommand(
    Guid ProjectId,
    Guid InspectionId) : ICommand<InspectionResponse>;
