using System;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260516090000_AddNotificationsAndNotices")]
    public partial class AddNotificationsAndNotices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    NoticeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NoticeDateNp = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TargetAudience = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: true),
                    SectionId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notices_ClassRooms_ClassId",
                        column: x => x.ClassId,
                        principalTable: "ClassRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notices_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    NotificationType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NotificationDateTimeNp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserNotificationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FcmToken = table.Column<string>(type: "text", nullable: false),
                    DeviceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Browser = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Platform = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotificationTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotificationTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPushSent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PushSentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsSignalRSent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SignalRSentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeliveryStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotifications_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(name: "IX_Notices_ClassId", table: "Notices", column: "ClassId");
            migrationBuilder.CreateIndex(name: "IX_Notices_CreatedAt", table: "Notices", column: "CreatedAt");
            migrationBuilder.CreateIndex(name: "IX_Notices_IsPublished", table: "Notices", column: "IsPublished");
            migrationBuilder.CreateIndex(name: "IX_Notices_NoticeDate", table: "Notices", column: "NoticeDate");
            migrationBuilder.CreateIndex(name: "IX_Notices_SectionId", table: "Notices", column: "SectionId");
            migrationBuilder.CreateIndex(name: "IX_Notices_TargetAudience", table: "Notices", column: "TargetAudience");

            migrationBuilder.CreateIndex(name: "IX_Notifications_CreatedAt", table: "Notifications", column: "CreatedAt");
            migrationBuilder.CreateIndex(name: "IX_Notifications_NotificationType", table: "Notifications", column: "NotificationType");
            migrationBuilder.CreateIndex(name: "IX_Notifications_NotificationType_ReferenceId_StudentId", table: "Notifications", columns: new[] { "NotificationType", "ReferenceId", "StudentId" });
            migrationBuilder.CreateIndex(name: "IX_Notifications_ReferenceId", table: "Notifications", column: "ReferenceId");
            migrationBuilder.CreateIndex(name: "IX_Notifications_StudentId", table: "Notifications", column: "StudentId");

            migrationBuilder.CreateIndex(name: "IX_UserNotifications_CreatedAt", table: "UserNotifications", column: "CreatedAt");
            migrationBuilder.CreateIndex(name: "IX_UserNotifications_IsRead", table: "UserNotifications", column: "IsRead");
            migrationBuilder.CreateIndex(name: "IX_UserNotifications_NotificationId", table: "UserNotifications", column: "NotificationId");
            migrationBuilder.CreateIndex(name: "IX_UserNotifications_NotificationId_UserId", table: "UserNotifications", columns: new[] { "NotificationId", "UserId" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_UserNotifications_UserId", table: "UserNotifications", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_UserNotifications_UserId_IsRead_CreatedAt", table: "UserNotifications", columns: new[] { "UserId", "IsRead", "CreatedAt" });

            migrationBuilder.CreateIndex(name: "IX_UserNotificationTokens_FcmToken", table: "UserNotificationTokens", column: "FcmToken");
            migrationBuilder.CreateIndex(name: "IX_UserNotificationTokens_UserId", table: "UserNotificationTokens", column: "UserId");
            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationTokens_UserId_FcmToken_IsActive",
                table: "UserNotificationTokens",
                columns: new[] { "UserId", "FcmToken", "IsActive" },
                unique: true,
                filter: "\"IsActive\" = true");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Notices");
            migrationBuilder.DropTable(name: "UserNotifications");
            migrationBuilder.DropTable(name: "UserNotificationTokens");
            migrationBuilder.DropTable(name: "Notifications");
        }
    }
}
