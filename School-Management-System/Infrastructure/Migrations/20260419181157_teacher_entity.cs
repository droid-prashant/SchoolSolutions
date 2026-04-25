using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class teacher_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClassSection_ClassSections_ClassSectionId",
                table: "TeacherClassSection");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClassSection_Teachers_TeacherId",
                table: "TeacherClassSection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherClassSection",
                table: "TeacherClassSection");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassSection_TeacherId",
                table: "TeacherClassSection");

            migrationBuilder.RenameTable(
                name: "TeacherClassSection",
                newName: "TeacherClassSections");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherClassSection_ClassSectionId",
                table: "TeacherClassSections",
                newName: "IX_TeacherClassSections_ClassSectionId");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Teachers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Teachers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Teachers",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ContactNumber",
                table: "Teachers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "Teachers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Teachers",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "AlternateContactNumber",
                table: "Teachers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DateOfBirthEn",
                table: "Teachers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DateOfBirthNp",
                table: "Teachers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Teachers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Teachers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Teachers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "Teachers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "Teachers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "InactiveDate",
                table: "Teachers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InactiveReason",
                table: "Teachers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Teachers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JoiningDateEn",
                table: "Teachers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JoiningDateNp",
                table: "Teachers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Teachers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MunicipalityId",
                table: "Teachers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceId",
                table: "Teachers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Teachers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardNo",
                table: "Teachers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "SubjectMarks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "SubjectMarks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SubjectMarks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "studentTransferCertificateLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "studentTransferCertificateLogs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "studentTransferCertificateLogs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Students",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Students",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Students",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "StudentFees",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "StudentFees",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StudentFees",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "StudentEnrollments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "StudentEnrollments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StudentEnrollments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "studentCharacterCertificateLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "studentCharacterCertificateLogs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "studentCharacterCertificateLogs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Schools",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Schools",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Schools",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Payments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Payments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "FeeTypes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "FeeTypes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FeeTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "FeeStructures",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "FeeStructures",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FeeStructures",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "FeeAdjustments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "FeeAdjustments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FeeAdjustments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "ExamResults",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ExamResults",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ExamResults",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Courses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Courses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Courses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "ClassRooms",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ClassRooms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ClassRooms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "ClassCourses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ClassCourses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ClassCourses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "AcademicYears",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "AcademicYears",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AcademicYears",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "AcademicYearId",
                table: "TeacherClassSections",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "TeacherClassSections",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "TeacherClassSections",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "TeacherClassSections",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                table: "TeacherClassSections",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveTo",
                table: "TeacherClassSections",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClassTeacher",
                table: "TeacherClassSections",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TeacherClassSections",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "TeacherClassSections",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherClassSections",
                table: "TeacherClassSections",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TeacherDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    DocumentTitle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_TeacherDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherDocuments_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Designation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SubjectOrDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
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
                    table.PrimaryKey("PK_TeacherExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherExperiences_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherQualifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    DegreeName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    InstitutionName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BoardOrUniversity = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PassedYear = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    GradeOrPercentage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MajorSubject = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Remarks = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
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
                    table.PrimaryKey("PK_TeacherQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherQualifications_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_Email",
                table: "Teachers",
                column: "Email",
                unique: true,
                filter: "\"Email\" IS NOT NULL AND \"Email\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_EmployeeCode",
                table: "Teachers",
                column: "EmployeeCode",
                unique: true,
                filter: "\"EmployeeCode\" IS NOT NULL AND \"EmployeeCode\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassSections_AcademicYearId_ClassSectionId_CourseId~",
                table: "TeacherClassSections",
                columns: new[] { "AcademicYearId", "ClassSectionId", "CourseId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassSections_AcademicYearId_ClassSectionId_IsClassT~",
                table: "TeacherClassSections",
                columns: new[] { "AcademicYearId", "ClassSectionId", "IsClassTeacher", "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassSections_CourseId",
                table: "TeacherClassSections",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassSections_TeacherId_AcademicYearId_ClassSectionI~",
                table: "TeacherClassSections",
                columns: new[] { "TeacherId", "AcademicYearId", "ClassSectionId", "CourseId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherDocuments_TeacherId_DocumentType_IsDeleted",
                table: "TeacherDocuments",
                columns: new[] { "TeacherId", "DocumentType", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherExperiences_TeacherId",
                table: "TeacherExperiences",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherQualifications_TeacherId_DegreeName_InstitutionName_~",
                table: "TeacherQualifications",
                columns: new[] { "TeacherId", "DegreeName", "InstitutionName", "PassedYear", "IsDeleted" });

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClassSections_AcademicYears_AcademicYearId",
                table: "TeacherClassSections",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClassSections_ClassSections_ClassSectionId",
                table: "TeacherClassSections",
                column: "ClassSectionId",
                principalTable: "ClassSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClassSections_Courses_CourseId",
                table: "TeacherClassSections",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClassSections_Teachers_TeacherId",
                table: "TeacherClassSections",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClassSections_AcademicYears_AcademicYearId",
                table: "TeacherClassSections");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClassSections_ClassSections_ClassSectionId",
                table: "TeacherClassSections");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClassSections_Courses_CourseId",
                table: "TeacherClassSections");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClassSections_Teachers_TeacherId",
                table: "TeacherClassSections");

            migrationBuilder.DropTable(
                name: "TeacherDocuments");

            migrationBuilder.DropTable(
                name: "TeacherExperiences");

            migrationBuilder.DropTable(
                name: "TeacherQualifications");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_Email",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_EmployeeCode",
                table: "Teachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherClassSections",
                table: "TeacherClassSections");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassSections_AcademicYearId_ClassSectionId_CourseId~",
                table: "TeacherClassSections");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassSections_AcademicYearId_ClassSectionId_IsClassT~",
                table: "TeacherClassSections");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassSections_CourseId",
                table: "TeacherClassSections");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassSections_TeacherId_AcademicYearId_ClassSectionI~",
                table: "TeacherClassSections");

            migrationBuilder.DropColumn(
                name: "AlternateContactNumber",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "DateOfBirthEn",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "DateOfBirthNp",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "InactiveDate",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "InactiveReason",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "JoiningDateEn",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "JoiningDateNp",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "MunicipalityId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "WardNo",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SubjectMarks");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "SubjectMarks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SubjectMarks");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "studentTransferCertificateLogs");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "studentTransferCertificateLogs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "studentTransferCertificateLogs");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StudentFees");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "StudentFees");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StudentFees");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StudentEnrollments");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "StudentEnrollments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StudentEnrollments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "studentCharacterCertificateLogs");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "studentCharacterCertificateLogs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "studentCharacterCertificateLogs");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "FeeTypes");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "FeeTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FeeTypes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "FeeStructures");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "FeeStructures");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FeeStructures");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "FeeAdjustments");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "FeeAdjustments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FeeAdjustments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ClassRooms");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ClassRooms");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ClassRooms");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ClassCourses");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ClassCourses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ClassCourses");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "TeacherClassSections");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "TeacherClassSections");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TeacherClassSections");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "TeacherClassSections");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "TeacherClassSections");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "TeacherClassSections");

            migrationBuilder.DropColumn(
                name: "IsClassTeacher",
                table: "TeacherClassSections");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TeacherClassSections");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "TeacherClassSections");

            migrationBuilder.RenameTable(
                name: "TeacherClassSections",
                newName: "TeacherClassSection");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherClassSections_ClassSectionId",
                table: "TeacherClassSection",
                newName: "IX_TeacherClassSection_ClassSectionId");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "ContactNumber",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "Teachers",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherClassSection",
                table: "TeacherClassSection",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassSection_TeacherId",
                table: "TeacherClassSection",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClassSection_ClassSections_ClassSectionId",
                table: "TeacherClassSection",
                column: "ClassSectionId",
                principalTable: "ClassSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClassSection_Teachers_TeacherId",
                table: "TeacherClassSection",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
