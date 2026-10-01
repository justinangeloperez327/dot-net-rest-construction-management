using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Construction.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOptimisticConcurrencyVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "work_packages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "suppliers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "submittals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "submittal_revisions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "submittal_comments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "rfis",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "rfi_comments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "purchase_requests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "purchase_request_items",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "purchase_orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "purchase_order_items",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "projects",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "project_members",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "project_locations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "notifications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "notification_preferences",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "issues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "inspections",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "equipment_maintenance",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "equipment_assignments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "equipment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "documents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "document_revisions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "daily_progress_reports",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "daily_progress_manpower",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "daily_progress_equipment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "daily_progress_activities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "corrective_actions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "companies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "attachments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "activity_dependencies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "activity_assignments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "activities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "work_packages");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "submittals");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "submittal_revisions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "submittal_comments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "rfis");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "rfi_comments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "purchase_requests");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "purchase_request_items");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "purchase_orders");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "purchase_order_items");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "project_members");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "project_locations");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "notification_preferences");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "issues");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "inspections");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "equipment_maintenance");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "equipment_assignments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "equipment");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "document_revisions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "daily_progress_reports");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "daily_progress_manpower");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "daily_progress_equipment");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "daily_progress_activities");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "corrective_actions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "attachments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "activity_dependencies");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "activity_assignments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "activities");
        }
    }
}
