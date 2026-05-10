using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260509130000_AddEducationTaxToFeeAdjustment")]
    public partial class AddEducationTaxToFeeAdjustment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EducationTaxAmount",
                table: "FeeAdjustments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EducationTaxPercentage",
                table: "FeeAdjustments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationTaxAmount",
                table: "FeeAdjustments");

            migrationBuilder.DropColumn(
                name: "EducationTaxPercentage",
                table: "FeeAdjustments");
        }
    }
}
