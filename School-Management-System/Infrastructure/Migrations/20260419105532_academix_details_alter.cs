using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class academix_details_alter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "AcademicYears",
                newName: "StartDateEn");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "AcademicYears",
                newName: "EndDateEn");

            migrationBuilder.AddColumn<string>(
                name: "EndDateNp",
                table: "AcademicYears",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartDateNp",
                table: "AcademicYears",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDateNp",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "StartDateNp",
                table: "AcademicYears");

            migrationBuilder.RenameColumn(
                name: "StartDateEn",
                table: "AcademicYears",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "EndDateEn",
                table: "AcademicYears",
                newName: "EndDate");
        }
    }
}
