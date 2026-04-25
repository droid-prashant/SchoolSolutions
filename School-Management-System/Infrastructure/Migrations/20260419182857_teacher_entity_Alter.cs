using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class teacher_entity_Alter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "TeacherExperiences");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TeacherExperiences");

            migrationBuilder.AddColumn<string>(
                name: "EndDateEn",
                table: "TeacherExperiences",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EndDateNp",
                table: "TeacherExperiences",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartDateEn",
                table: "TeacherExperiences",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartDateNp",
                table: "TeacherExperiences",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDateEn",
                table: "TeacherExperiences");

            migrationBuilder.DropColumn(
                name: "EndDateNp",
                table: "TeacherExperiences");

            migrationBuilder.DropColumn(
                name: "StartDateEn",
                table: "TeacherExperiences");

            migrationBuilder.DropColumn(
                name: "StartDateNp",
                table: "TeacherExperiences");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Teachers",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "TeacherExperiences",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "TeacherExperiences",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
