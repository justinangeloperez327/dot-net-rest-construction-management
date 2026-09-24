using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.Projects.GetProjects;

public sealed record GetProjectsQuery(PageRequest Page)
    : IQuery<PagedResult<ProjectResponse>>;
