using Construction.Application.Common.Messaging;

namespace Construction.Application.Projects.GetProject;

public sealed record GetProjectQuery(Guid ProjectId)
    : IQuery<ProjectResponse>;
