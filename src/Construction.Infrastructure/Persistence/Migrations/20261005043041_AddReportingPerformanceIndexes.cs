using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Construction.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReportingPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_submittal_revisions_SubmittalId_IsCurrent",
                table: "submittal_revisions");

            migrationBuilder.CreateIndex(
                name: "IX_submittal_revisions_SubmittalId_IsCurrent_ReviewDueDate",
                table: "submittal_revisions",
                columns: new[] { "SubmittalId", "IsCurrent", "ReviewDueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_issues_ProjectId_Status_DueDate",
                table: "issues",
                columns: new[] { "ProjectId", "Status", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_activities_ProjectId_Status_PlannedEndDate",
                table: "activities",
                columns: new[] { "ProjectId", "Status", "PlannedEndDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_submittal_revisions_SubmittalId_IsCurrent_ReviewDueDate",
                table: "submittal_revisions");

            migrationBuilder.DropIndex(
                name: "IX_issues_ProjectId_Status_DueDate",
                table: "issues");

            migrationBuilder.DropIndex(
                name: "IX_activities_ProjectId_Status_PlannedEndDate",
                table: "activities");

            migrationBuilder.CreateIndex(
                name: "IX_submittal_revisions_SubmittalId_IsCurrent",
                table: "submittal_revisions",
                columns: new[] { "SubmittalId", "IsCurrent" });
        }
    }
}
