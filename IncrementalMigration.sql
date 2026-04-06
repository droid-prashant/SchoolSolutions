START TRANSACTION;

CREATE TABLE "studentCharacterCertificateLogs" (
    "Id" uuid NOT NULL,
    "StudentId" uuid NOT NULL,
    "CreatedDate" timestamp with time zone NOT NULL,
    "ModifiedDate" timestamp with time zone NOT NULL,
    "CreatedBy" uuid NOT NULL,
    "ModifiedBy" uuid NOT NULL,
    CONSTRAINT "PK_studentCharacterCertificateLogs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_studentCharacterCertificateLogs_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Students" ("Id") ON DELETE CASCADE
);

CREATE TABLE "studentTransferCertificateLogs" (
    "Id" uuid NOT NULL,
    "StudentId" uuid NOT NULL,
    "CreatedDate" timestamp with time zone NOT NULL,
    "ModifiedDate" timestamp with time zone NOT NULL,
    "CreatedBy" uuid NOT NULL,
    "ModifiedBy" uuid NOT NULL,
    CONSTRAINT "PK_studentTransferCertificateLogs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_studentTransferCertificateLogs_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Students" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_studentCharacterCertificateLogs_StudentId" ON "studentCharacterCertificateLogs" ("StudentId");

CREATE INDEX "IX_studentTransferCertificateLogs_StudentId" ON "studentTransferCertificateLogs" ("StudentId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251105044920_student_certificate_log', '8.0.15');

COMMIT;




START TRANSACTION;

ALTER TABLE "studentTransferCertificateLogs" ADD "CertificateNumber" integer NOT NULL DEFAULT 0;

ALTER TABLE "studentCharacterCertificateLogs" ADD "CertificateNumber" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251105053518_Alter_Student_Certificate_Log', '8.0.15');

COMMIT;



START TRANSACTION;

ALTER TABLE "studentTransferCertificateLogs" ADD "StudentId1" uuid;

ALTER TABLE "studentCharacterCertificateLogs" ADD "StudentId1" uuid;

CREATE UNIQUE INDEX "IX_studentTransferCertificateLogs_StudentId1" ON "studentTransferCertificateLogs" ("StudentId1");

CREATE UNIQUE INDEX "IX_studentCharacterCertificateLogs_StudentId1" ON "studentCharacterCertificateLogs" ("StudentId1");

ALTER TABLE "studentCharacterCertificateLogs" ADD CONSTRAINT "FK_studentCharacterCertificateLogs_Students_StudentId1" FOREIGN KEY ("StudentId1") REFERENCES "Students" ("Id");

ALTER TABLE "studentTransferCertificateLogs" ADD CONSTRAINT "FK_studentTransferCertificateLogs_Students_StudentId1" FOREIGN KEY ("StudentId1") REFERENCES "Students" ("Id");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251105061234_student_table_alter', '8.0.15');

COMMIT;

START TRANSACTION;

ALTER TABLE "Students" ADD "isActive" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251106165410_Student_Is_Active_column', '8.0.15');

COMMIT;



START TRANSACTION;

ALTER TABLE "Students" DROP COLUMN "DateOfBirth";

ALTER TABLE "Students" ADD "DateOfBirthEn" text NOT NULL DEFAULT '';

ALTER TABLE "Students" ADD "DateOfBirthNp" text NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251113172439_Student_Dob_alter', '8.0.15');

COMMIT;

START TRANSACTION;

ALTER TABLE "ExamResults" ADD "Attendance" integer NOT NULL DEFAULT 0;

ALTER TABLE "ExamResults" ADD "TotalSchoolDays" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251115083012_ExamResult__column_added', '8.0.15');

COMMIT;


START TRANSACTION;

ALTER TABLE public."SubjectMarks" ALTER COLUMN "ObtainedTheoryMarks" TYPE numeric;

ALTER TABLE "SubjectMarks" ALTER COLUMN "ObtainedPracticalMarks" TYPE numeric;

ALTER TABLE "SubjectMarks" ALTER COLUMN "GradePointTheory" TYPE numeric;

ALTER TABLE "SubjectMarks" ALTER COLUMN "GradePointPractical" TYPE numeric;

ALTER TABLE "SubjectMarks" ALTER COLUMN "FullTheoryMarks" TYPE numeric;

ALTER TABLE "SubjectMarks" ALTER COLUMN "FullPracticalMarks" TYPE numeric;

ALTER TABLE "SubjectMarks" ALTER COLUMN "FinalGradePoint" TYPE numeric;

ALTER TABLE "ExamResults" ALTER COLUMN "TotalCredit" TYPE numeric;

ALTER TABLE "ExamResults" ALTER COLUMN "GPA" TYPE numeric;

ALTER TABLE "ExamResults" ALTER COLUMN "ExamType" TYPE integer USING "ExamType"::integer;;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260405123003_dataType_updated_to_decimal', '8.0.15');

COMMIT;


START TRANSACTION;

ALTER TABLE "Students" DROP COLUMN "Address";

ALTER TABLE "Students" ALTER COLUMN "ParentEmail" DROP NOT NULL;

ALTER TABLE "Students" ALTER COLUMN "Age" DROP NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260405144152_student_entity_alter', '8.0.15');

COMMIT;





