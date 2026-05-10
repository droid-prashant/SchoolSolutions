using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260509110000_AddFeeApplicabilityAndEnrollmentBus")]
    public partial class AddFeeApplicabilityAndEnrollmentBus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Applicability",
                table: "FeeTypes",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsBusRequired",
                table: "StudentEnrollments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Applicability",
                table: "FeeTypes");

            migrationBuilder.DropColumn(
                name: "IsBusRequired",
                table: "StudentEnrollments");
        }
    }
}
