using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Construction.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "daily_progress_reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Weather = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TemperatureCelsius = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    WorkSummary = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Remarks = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SubmittedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubmittedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RejectionReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_progress_reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_daily_progress_reports_auth_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_daily_progress_reports_auth_users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_daily_progress_reports_auth_users_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_daily_progress_reports_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "daily_progress_activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ReportedProgressPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    QuantityCompleted = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_progress_activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_daily_progress_activities_activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_daily_progress_activities_daily_progress_reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "daily_progress_reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "daily_progress_equipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    WorkingHours = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    IdleHours = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_progress_equipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_daily_progress_equipment_daily_progress_reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "daily_progress_reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "daily_progress_manpower",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    Trade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Headcount = table.Column<int>(type: "integer", nullable: false),
                    TotalHours = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_progress_manpower", x => x.Id);
                    table.ForeignKey(
                        name: "FK_daily_progress_manpower_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_daily_progress_manpower_daily_progress_reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "daily_progress_reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_daily_progress_activities_ActivityId",
                table: "daily_progress_activities",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_progress_activities_ReportId_ActivityId",
                table: "daily_progress_activities",
                columns: new[] { "ReportId", "ActivityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_daily_progress_equipment_ReportId",
                table: "daily_progress_equipment",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_progress_manpower_CompanyId",
                table: "daily_progress_manpower",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_progress_manpower_ReportId",
                table: "daily_progress_manpower",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_progress_reports_CreatedByUserId",
                table: "daily_progress_reports",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_progress_reports_ProjectId_ReportDate",
                table: "daily_progress_reports",
                columns: new[] { "ProjectId", "ReportDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_daily_progress_reports_ProjectId_Status_ReportDate",
                table: "daily_progress_reports",
                columns: new[] { "ProjectId", "Status", "ReportDate" });

            migrationBuilder.CreateIndex(
                name: "IX_daily_progress_reports_ReviewedByUserId",
                table: "daily_progress_reports",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_progress_reports_SubmittedByUserId",
                table: "daily_progress_reports",
                column: "SubmittedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_progress_activities");

            migrationBuilder.DropTable(
                name: "daily_progress_equipment");

            migrationBuilder.DropTable(
                name: "daily_progress_manpower");

            migrationBuilder.DropTable(
                name: "daily_progress_reports");
        }
    }
}
