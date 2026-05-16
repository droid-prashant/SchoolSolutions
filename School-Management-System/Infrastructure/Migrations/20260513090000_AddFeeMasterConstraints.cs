using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260513090000_AddFeeMasterConstraints")]
    public partial class AddFeeMasterConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "FeeTypes"
                SET "Name" = LEFT(BTRIM(COALESCE("Name", '')), 120)
                WHERE "Name" IS NULL OR "Name" <> BTRIM("Name") OR LENGTH("Name") > 120;

                UPDATE "FeeTypes"
                SET "Name" = 'Fee Type ' || "Id"::text
                WHERE BTRIM("Name") = '';

                WITH duplicate_fee_types AS (
                    SELECT
                        "Id",
                        ROW_NUMBER() OVER (PARTITION BY "Name" ORDER BY "CreatedDate", "Id") AS row_number
                    FROM "FeeTypes"
                )
                UPDATE "FeeTypes" fee_type
                SET "Name" = LEFT(fee_type."Name", 112) || ' (' || duplicate_fee_types.row_number || ')'
                FROM duplicate_fee_types
                WHERE fee_type."Id" = duplicate_fee_types."Id"
                  AND duplicate_fee_types.row_number > 1;

                UPDATE "FeeTypes"
                SET "Frequency" = LEFT(COALESCE("Frequency", ''), 40)
                WHERE "Frequency" IS NULL OR LENGTH("Frequency") > 40;

                UPDATE "FeeStructures"
                SET "Description" = LEFT(COALESCE("Description", ''), 300)
                WHERE "Description" IS NULL OR LENGTH("Description") > 300;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FeeTypes",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Frequency",
                table: "FeeTypes",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FeeStructures",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "FeeStructures",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateIndex(
                name: "IX_FeeTypes_Name",
                table: "FeeTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructures_AcademicYearId_ClassId_FeeTypeId",
                table: "FeeStructures",
                columns: new[] { "AcademicYearId", "ClassId", "FeeTypeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FeeTypes_Name",
                table: "FeeTypes");

            migrationBuilder.DropIndex(
                name: "IX_FeeStructures_AcademicYearId_ClassId_FeeTypeId",
                table: "FeeStructures");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FeeTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Frequency",
                table: "FeeTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FeeStructures",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "FeeStructures",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);
        }
    }
}
