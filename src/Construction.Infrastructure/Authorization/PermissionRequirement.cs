using Microsoft.AspNetCore.Authorization;

namespace Construction.Infrastructure.Authorization;

public sealed record PermissionRequirement(string Permission)
    : IAuthorizationRequirement;
