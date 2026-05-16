using System;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260516103000_AddNoticeStudents")]
    public partial class AddNoticeStudents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NoticeStudents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NoticeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoticeStudents_Notices_NoticeId",
                        column: x => x.NoticeId,
                        principalTable: "Notices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoticeStudents_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NoticeStudents_NoticeId",
                table: "NoticeStudents",
                column: "NoticeId");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeStudents_NoticeId_StudentId",
                table: "NoticeStudents",
                columns: new[] { "NoticeId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NoticeStudents_StudentId",
                table: "NoticeStudents",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "NoticeStudents");
        }
    }
}
