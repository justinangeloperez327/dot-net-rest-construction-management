namespace Construction.Application.Common.Authorization;

public static class Permissions
{
    public static class Users
    {
        public const string View = "users.view";
        public const string Manage = "users.manage";
    }

    public static class Companies
    {
        public const string View = "companies.view";
        public const string Manage = "companies.manage";
    }

    public static class Projects
    {
        public const string View = "projects.view";
        public const string Create = "projects.create";
        public const string Update = "projects.update";
        public const string Archive = "projects.archive";
        public const string ManageMembers = "projects.members.manage";
    }

    public static class Activities
    {
        public const string View = "activities.view";
        public const string Manage = "activities.manage";
    }

    public static class DailyProgress
    {
        public const string View = "daily-progress.view";
        public const string Manage = "daily-progress.manage";
    }

    public static class Documents
    {
        public const string View = "documents.view";
        public const string Manage = "documents.manage";
    }

    public static class Rfis
    {
        public const string View = "rfis.view";
        public const string Manage = "rfis.manage";
    }

    public static class Submittals
    {
        public const string View = "submittals.view";
        public const string Manage = "submittals.manage";
    }

    public static class Inspections
    {
        public const string View = "inspections.view";
        public const string Manage = "inspections.manage";
    }

    public static class Issues
    {
        public const string View = "issues.view";
        public const string Manage = "issues.manage";
    }

    public static class Equipment
    {
        public const string View = "equipment.view";
        public const string Manage = "equipment.manage";
    }

    public static class Procurement
    {
        public const string View = "procurement.view";
        public const string ManageRequests = "purchase-requests.manage";
        public const string ManageOrders = "purchase-orders.manage";
        public const string IssueOrders = "purchase-orders.issue";
    }

    public static class Reports
    {
        public const string View = "reports.view";
    }

    public static class Audit
    {
        public const string View = "audit.view";
    }

    public static class Administration
    {
        public const string ManageRoles = "administration.roles.manage";
        public const string ManagePermissions = "administration.permissions.manage";
    }

    public static IReadOnlySet<string> All { get; } =
        new HashSet<string>(
        [
            Users.View,
            Users.Manage,
            Companies.View,
            Companies.Manage,
            Projects.View,
            Projects.Create,
            Projects.Update,
            Projects.Archive,
            Projects.ManageMembers,
            Activities.View,
            Activities.Manage,
            DailyProgress.View,
            DailyProgress.Manage,
            Documents.View,
            Documents.Manage,
            Rfis.View,
            Rfis.Manage,
            Submittals.View,
            Submittals.Manage,
            Inspections.View,
            Inspections.Manage,
            Issues.View,
            Issues.Manage,
            Equipment.View,
            Equipment.Manage,
            Procurement.View,
            Procurement.ManageRequests,
            Procurement.ManageOrders,
            Procurement.IssueOrders,
            Reports.View,
            Audit.View,
            Administration.ManageRoles,
            Administration.ManagePermissions
        ],
        StringComparer.Ordinal);
}
