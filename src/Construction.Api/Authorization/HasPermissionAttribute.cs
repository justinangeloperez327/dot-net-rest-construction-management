using Construction.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Construction.Api.Authorization;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = true,
    Inherited = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);
        Policy = PermissionPolicy.Build(permission);
    }
}
