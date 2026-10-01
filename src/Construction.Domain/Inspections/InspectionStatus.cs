namespace Construction.Domain.Inspections;

public enum InspectionStatus
{
    Draft = 0,
    Requested = 1,
    InProgress = 2,
    Passed = 3,
    Failed = 4,
    Cancelled = 5
}
