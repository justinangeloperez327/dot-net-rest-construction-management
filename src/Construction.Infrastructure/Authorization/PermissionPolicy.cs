namespace Construction.Infrastructure.Authorization;

public static class PermissionPolicy
{
    public const string Prefix = "Permission:";

    public static string Build(string permission)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);
        return Prefix + permission;
    }

    public static bool TryGetPermission(
        string policyName,
        out string permission)
    {
        if (policyName.StartsWith(Prefix, StringComparison.Ordinal))
        {
            permission = policyName[Prefix.Length..];
            return !string.IsNullOrWhiteSpace(permission);
        }

        permission = string.Empty;
        return false;
    }
}
