namespace Construction.Api.Contracts.DailyProgress;

public sealed record SetDailyProgressManpowerRequest(
    IReadOnlyCollection<DailyProgressManpowerRequestItem> Manpower);

public sealed record DailyProgressManpowerRequestItem(
    Guid? CompanyId,
    string Trade,
    int Headcount,
    decimal TotalHours);
