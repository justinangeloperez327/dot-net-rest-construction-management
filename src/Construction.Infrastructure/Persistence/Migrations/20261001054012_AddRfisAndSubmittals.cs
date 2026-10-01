using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Construction.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRfisAndSubmittals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rfis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Question = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ResponsibleUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RaisedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Response = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    RespondedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RespondedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ClosedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClosedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rfis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rfis_auth_users_ClosedByUserId",
                        column: x => x.ClosedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rfis_auth_users_RaisedByUserId",
                        column: x => x.RaisedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rfis_auth_users_RespondedByUserId",
                        column: x => x.RespondedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rfis_auth_users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rfis_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "submittals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ResponsibleUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CurrentVersionNumber = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submittals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_submittals_auth_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_submittals_auth_users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_submittals_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rfi_comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RfiId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rfi_comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rfi_comments_auth_users_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rfi_comments_rfis_RfiId",
                        column: x => x.RfiId,
                        principalTable: "rfis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rfi_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RfiId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rfi_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rfi_history_auth_users_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rfi_history_rfis_RfiId",
                        column: x => x.RfiId,
                        principalTable: "rfis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "submittal_comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmittalId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submittal_comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_submittal_comments_auth_users_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_submittal_comments_submittals_SubmittalId",
                        column: x => x.SubmittalId,
                        principalTable: "submittals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "submittal_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmittalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submittal_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_submittal_history_auth_users_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_submittal_history_submittals_SubmittalId",
                        column: x => x.SubmittalId,
                        principalTable: "submittals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "submittal_revisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmittalId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    RevisionCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    SubmittedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubmittedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReviewDueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReviewRemarks = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submittal_revisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_submittal_revisions_auth_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_submittal_revisions_auth_users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_submittal_revisions_auth_users_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "auth_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_submittal_revisions_submittals_SubmittalId",
                        column: x => x.SubmittalId,
                        principalTable: "submittals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rfi_comments_AuthorUserId",
                table: "rfi_comments",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_rfi_comments_RfiId_CreatedAtUtc",
                table: "rfi_comments",
                columns: new[] { "RfiId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_rfi_history_ActorUserId",
                table: "rfi_history",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_rfi_history_RfiId_OccurredAtUtc",
                table: "rfi_history",
                columns: new[] { "RfiId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_rfis_ClosedByUserId",
                table: "rfis",
                column: "ClosedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_rfis_ProjectId_NormalizedNumber",
                table: "rfis",
                columns: new[] { "ProjectId", "NormalizedNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rfis_ProjectId_Status_DueDate",
                table: "rfis",
                columns: new[] { "ProjectId", "Status", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_rfis_RaisedByUserId",
                table: "rfis",
                column: "RaisedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_rfis_RespondedByUserId",
                table: "rfis",
                column: "RespondedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_rfis_ResponsibleUserId",
                table: "rfis",
                column: "ResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_submittal_comments_AuthorUserId",
                table: "submittal_comments",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_submittal_comments_SubmittalId_CreatedAtUtc",
                table: "submittal_comments",
                columns: new[] { "SubmittalId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_submittal_history_ActorUserId",
                table: "submittal_history",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_submittal_history_SubmittalId_OccurredAtUtc",
                table: "submittal_history",
                columns: new[] { "SubmittalId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_submittal_revisions_CreatedByUserId",
                table: "submittal_revisions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_submittal_revisions_ReviewedByUserId",
                table: "submittal_revisions",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_submittal_revisions_SubmittalId_IsCurrent",
                table: "submittal_revisions",
                columns: new[] { "SubmittalId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_submittal_revisions_SubmittalId_RevisionCode",
                table: "submittal_revisions",
                columns: new[] { "SubmittalId", "RevisionCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_submittal_revisions_SubmittalId_VersionNumber",
                table: "submittal_revisions",
                columns: new[] { "SubmittalId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_submittal_revisions_SubmittedByUserId",
                table: "submittal_revisions",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_submittals_CreatedByUserId",
                table: "submittals",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_submittals_ProjectId_NormalizedNumber",
                table: "submittals",
                columns: new[] { "ProjectId", "NormalizedNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_submittals_ProjectId_Status_Type",
                table: "submittals",
                columns: new[] { "ProjectId", "Status", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_submittals_ResponsibleUserId",
                table: "submittals",
                column: "ResponsibleUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rfi_comments");

            migrationBuilder.DropTable(
                name: "rfi_history");

            migrationBuilder.DropTable(
                name: "submittal_comments");

            migrationBuilder.DropTable(
                name: "submittal_history");

            migrationBuilder.DropTable(
                name: "submittal_revisions");

            migrationBuilder.DropTable(
                name: "rfis");

            migrationBuilder.DropTable(
                name: "submittals");
        }
    }
}
