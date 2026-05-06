using System;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260506090000_AddAttendanceModule")]
    public partial class AddAttendanceModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentEnrollmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassSectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttendanceDateEn = table.Column<DateOnly>(type: "date", nullable: false),
                    AttendanceDateNp = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RecordedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAttendances_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAttendances_ClassSections_ClassSectionId",
                        column: x => x.ClassSectionId,
                        principalTable: "ClassSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAttendances_StudentEnrollments_StudentEnrollmentId",
                        column: x => x.StudentEnrollmentId,
                        principalTable: "StudentEnrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAttendances_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttendanceDateEn = table.Column<DateOnly>(type: "date", nullable: false),
                    AttendanceDateNp = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CheckInTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    CheckOutTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RecordedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherAttendances_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherAttendances_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_AcademicYearId_ClassSectionId_AttendanceDateEn",
                table: "StudentAttendances",
                columns: new[] { "AcademicYearId", "ClassSectionId", "AttendanceDateEn" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_ClassSectionId",
                table: "StudentAttendances",
                column: "ClassSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_Status_AttendanceDateEn",
                table: "StudentAttendances",
                columns: new[] { "Status", "AttendanceDateEn" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_StudentEnrollmentId_AttendanceDateEn",
                table: "StudentAttendances",
                columns: new[] { "StudentEnrollmentId", "AttendanceDateEn" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_StudentEnrollmentId_AttendanceDateEn_IsDeleted",
                table: "StudentAttendances",
                columns: new[] { "StudentEnrollmentId", "AttendanceDateEn", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_StudentId_AcademicYearId",
                table: "StudentAttendances",
                columns: new[] { "StudentId", "AcademicYearId" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAttendances_AcademicYearId_AttendanceDateEn",
                table: "TeacherAttendances",
                columns: new[] { "AcademicYearId", "AttendanceDateEn" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAttendances_Status_AttendanceDateEn",
                table: "TeacherAttendances",
                columns: new[] { "Status", "AttendanceDateEn" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAttendances_TeacherId_AcademicYearId",
                table: "TeacherAttendances",
                columns: new[] { "TeacherId", "AcademicYearId" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAttendances_TeacherId_AcademicYearId_AttendanceDateEn",
                table: "TeacherAttendances",
                columns: new[] { "TeacherId", "AcademicYearId", "AttendanceDateEn" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "StudentAttendances");
            migrationBuilder.DropTable(name: "TeacherAttendances");
        }
    }
}
