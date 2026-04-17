START TRANSACTION;

ALTER TABLE "AcademicYears" DROP COLUMN "PrincipalName";

ALTER TABLE "AcademicYears" DROP COLUMN "SchoolName";

ALTER TABLE "Students" RENAME COLUMN "isActive" TO "IsActive";

ALTER TABLE "Teachers" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "TeacherClassSection" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "SubjectMarks" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "studentTransferCertificateLogs" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "StudentFees" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "StudentEnrollments" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "studentCharacterCertificateLogs" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Payments" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "FeeTypes" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "FeeStructures" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "FeeAdjustments" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "ExamResults" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Courses" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "ClassRooms" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "ClassCourses" ADD "IsActive" boolean NOT NULL DEFAULT FALSE;

CREATE TABLE "Schools" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "EstablishedYear" date NOT NULL,
    "PrincipalName" text NOT NULL,
    "Province" text NOT NULL,
    "Address" text NOT NULL,
    "City" text NOT NULL,
    "District" text NOT NULL,
    "WardNo" integer NOT NULL,
    "PhoneNumber" text NOT NULL,
    "Email" text NOT NULL,
    "Website" text NOT NULL,
    "LogoUrl" text NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedDate" timestamp with time zone NOT NULL,
    "ModifiedDate" timestamp with time zone NOT NULL,
    "CreatedBy" uuid NOT NULL,
    "ModifiedBy" uuid NOT NULL,
    CONSTRAINT "PK_Schools" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260411071616_new_update', '8.0.15');

COMMIT;

START TRANSACTION;

ALTER TABLE "ClassCourses" ALTER COLUMN "TheoryFullMarks" DROP NOT NULL;

ALTER TABLE "ClassCourses" ALTER COLUMN "TheoryCreditHour" DROP NOT NULL;

ALTER TABLE "ClassCourses" ALTER COLUMN "PracticalFullMarks" DROP NOT NULL;

ALTER TABLE "ClassCourses" ALTER COLUMN "PracticalCreditHour" DROP NOT NULL;

ALTER TABLE "ClassCourses" ADD "IsPracticalRequired" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "ClassCourses" ADD "IsTheoryRequired" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260416171923_add_classcourse_requirement_flags', '8.0.15');

COMMIT;

