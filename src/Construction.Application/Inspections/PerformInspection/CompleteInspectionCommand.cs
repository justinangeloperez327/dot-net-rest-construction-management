using Construction.Application.Common.Messaging;

namespace Construction.Application.Inspections.PerformInspection;

public sealed record CompleteInspectionCommand(
    Guid ProjectId,
    Guid InspectionId,
    bool Passed,
    string ResultNotes) : ICommand<InspectionResponse>;
