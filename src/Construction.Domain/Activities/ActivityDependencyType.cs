namespace Construction.Domain.Activities;

public enum ActivityDependencyType
{
    FinishToStart = 0,
    StartToStart = 1,
    FinishToFinish = 2,
    StartToFinish = 3
}
