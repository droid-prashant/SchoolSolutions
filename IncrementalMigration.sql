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


