using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Construction.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQualitySiteControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "inspections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedForDate = table.Column<DateOnly>(type: "date", nullable: true),
                    InspectorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ResultNotes = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    InspectedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    InspectedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_inspections_activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inspections_auth_users_InspectedByUserId",
                        column: x => x.InspectedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inspections_auth_users_InspectorUserId",
                        column: x => x.InspectorUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inspections_auth_users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inspections_project_locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "project_locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inspections_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "issues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResponsibleUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ResolutionSummary = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    VerifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    VerifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_issues_activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_issues_auth_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_issues_auth_users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_issues_auth_users_VerifiedByUserId",
                        column: x => x.VerifiedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_issues_project_locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "project_locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_issues_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inspection_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InspectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inspection_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_inspection_history_auth_users_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inspection_history_inspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "inspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "corrective_actions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    ResponsibleUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CompletionNotes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CompletedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_corrective_actions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_corrective_actions_auth_users_CompletedByUserId",
                        column: x => x.CompletedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_corrective_actions_auth_users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_corrective_actions_issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "issue_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issue_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_issue_history_auth_users_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_issue_history_issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_corrective_actions_CompletedByUserId",
                table: "corrective_actions",
                column: "CompletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_corrective_actions_IssueId_Status_DueDate",
                table: "corrective_actions",
                columns: new[] { "IssueId", "Status", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_corrective_actions_ResponsibleUserId",
                table: "corrective_actions",
                column: "ResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_inspection_history_ActorUserId",
                table: "inspection_history",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_inspection_history_InspectionId_OccurredAtUtc",
                table: "inspection_history",
                columns: new[] { "InspectionId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_inspections_ActivityId",
                table: "inspections",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_inspections_InspectedByUserId",
                table: "inspections",
                column: "InspectedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_inspections_InspectorUserId",
                table: "inspections",
                column: "InspectorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_inspections_LocationId",
                table: "inspections",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_inspections_ProjectId_NormalizedNumber",
                table: "inspections",
                columns: new[] { "ProjectId", "NormalizedNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inspections_ProjectId_Status_RequestedForDate",
                table: "inspections",
                columns: new[] { "ProjectId", "Status", "RequestedForDate" });

            migrationBuilder.CreateIndex(
                name: "IX_inspections_RequestedByUserId",
                table: "inspections",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_issue_history_ActorUserId",
                table: "issue_history",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_issue_history_IssueId_OccurredAtUtc",
                table: "issue_history",
                columns: new[] { "IssueId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_issues_ActivityId",
                table: "issues",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_issues_CreatedByUserId",
                table: "issues",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_issues_LocationId",
                table: "issues",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_issues_ProjectId_NormalizedNumber",
                table: "issues",
                columns: new[] { "ProjectId", "NormalizedNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_issues_ProjectId_Status_Severity_DueDate",
                table: "issues",
                columns: new[] { "ProjectId", "Status", "Severity", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_issues_ResponsibleUserId",
                table: "issues",
                column: "ResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_issues_VerifiedByUserId",
                table: "issues",
                column: "VerifiedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "corrective_actions");

            migrationBuilder.DropTable(
                name: "inspection_history");

            migrationBuilder.DropTable(
                name: "issue_history");

            migrationBuilder.DropTable(
                name: "inspections");

            migrationBuilder.DropTable(
                name: "issues");
        }
    }
}
