using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class student_table_alter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StudentId1",
                table: "studentTransferCertificateLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId1",
                table: "studentCharacterCertificateLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_studentTransferCertificateLogs_StudentId1",
                table: "studentTransferCertificateLogs",
                column: "StudentId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_studentCharacterCertificateLogs_StudentId1",
                table: "studentCharacterCertificateLogs",
                column: "StudentId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_studentCharacterCertificateLogs_Students_StudentId1",
                table: "studentCharacterCertificateLogs",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_studentTransferCertificateLogs_Students_StudentId1",
                table: "studentTransferCertificateLogs",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_studentCharacterCertificateLogs_Students_StudentId1",
                table: "studentCharacterCertificateLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_studentTransferCertificateLogs_Students_StudentId1",
                table: "studentTransferCertificateLogs");

            migrationBuilder.DropIndex(
                name: "IX_studentTransferCertificateLogs_StudentId1",
                table: "studentTransferCertificateLogs");

            migrationBuilder.DropIndex(
                name: "IX_studentCharacterCertificateLogs_StudentId1",
                table: "studentCharacterCertificateLogs");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "studentTransferCertificateLogs");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "studentCharacterCertificateLogs");
        }
    }
}
