using System;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260506080000_StudentEnrollmentStatus")]
    public partial class StudentEnrollmentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnrollmentStatus",
                table: "StudentEnrollments",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusDate",
                table: "StudentEnrollments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusRemarks",
                table: "StudentEnrollments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "StudentEnrollments"
                SET "IsActive" = TRUE,
                    "EnrollmentStatus" = 1
                WHERE "IsDeleted" = FALSE;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_AcademicYearId_ClassSectionId_IsActive_IsDeleted",
                table: "StudentEnrollments",
                columns: new[] { "AcademicYearId", "ClassSectionId", "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_StudentId_AcademicYearId",
                table: "StudentEnrollments",
                columns: new[] { "StudentId", "AcademicYearId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentEnrollments_AcademicYearId_ClassSectionId_IsActive_IsDeleted",
                table: "StudentEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_StudentEnrollments_StudentId_AcademicYearId",
                table: "StudentEnrollments");

            migrationBuilder.DropColumn(
                name: "EnrollmentStatus",
                table: "StudentEnrollments");

            migrationBuilder.DropColumn(
                name: "StatusDate",
                table: "StudentEnrollments");

            migrationBuilder.DropColumn(
                name: "StatusRemarks",
                table: "StudentEnrollments");
        }
    }
}
