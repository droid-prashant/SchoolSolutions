using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class attendance_management : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentEnrollments_AcademicYearId",
                table: "StudentEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_StudentEnrollments_StudentId",
                table: "StudentEnrollments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_AcademicYearId",
                table: "StudentEnrollments",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_StudentId",
                table: "StudentEnrollments",
                column: "StudentId");
        }
    }
}
