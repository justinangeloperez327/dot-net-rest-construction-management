namespace Construction.Domain.Inspections;

public enum InspectionHistoryAction
{
    Created = 0,
    Updated = 1,
    Requested = 2,
    Started = 3,
    Passed = 4,
    Failed = 5,
    Cancelled = 6
}
