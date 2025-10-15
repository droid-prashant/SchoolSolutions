using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Subject_Mark__Entity_Alter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: new Guid("5de70202-810b-40e8-b632-715df1674d47"));

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: new Guid("b782c5ef-0813-4934-a1d3-a5dfa021bf4c"));

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: new Guid("eef99712-d3e2-488d-b6ef-12335ec0ebdf"));

            migrationBuilder.AddColumn<double>(
                name: "FinalGradePoint",
                table: "SubjectMarks",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.InsertData(
                table: "Sections",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("925cf03f-0dbb-4d58-8ff4-3f87a2966f87"), "B" },
                    { new Guid("d52bdaeb-27ce-464f-adc2-454d45df0d0c"), "A" },
                    { new Guid("d5f1a6ae-d44b-48d5-9705-2246172bdb2f"), "C" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: new Guid("925cf03f-0dbb-4d58-8ff4-3f87a2966f87"));

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: new Guid("d52bdaeb-27ce-464f-adc2-454d45df0d0c"));

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: new Guid("d5f1a6ae-d44b-48d5-9705-2246172bdb2f"));

            migrationBuilder.DropColumn(
                name: "FinalGradePoint",
                table: "SubjectMarks");

            migrationBuilder.InsertData(
                table: "Sections",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("5de70202-810b-40e8-b632-715df1674d47"), "C" },
                    { new Guid("b782c5ef-0813-4934-a1d3-a5dfa021bf4c"), "B" },
                    { new Guid("eef99712-d3e2-488d-b6ef-12335ec0ebdf"), "A" }
                });
        }
    }
}
