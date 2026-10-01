using Construction.Application.Common.Messaging;

namespace Construction.Application.Inspections.GetInspection;

public sealed record GetInspectionQuery(
    Guid ProjectId,
    Guid InspectionId) : IQuery<InspectionResponse>;
