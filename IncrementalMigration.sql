CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = '20260411071616_new_update'
    ) THEN

        ALTER TABLE "AcademicYears" DROP COLUMN IF EXISTS "PrincipalName";
        ALTER TABLE "AcademicYears" DROP COLUMN IF EXISTS "SchoolName";

        IF EXISTS (
            SELECT 1
            FROM information_schema.columns
            WHERE table_name = 'Students'
              AND column_name = 'isActive'
        ) AND NOT EXISTS (
            SELECT 1
            FROM information_schema.columns
            WHERE table_name = 'Students'
              AND column_name = 'IsActive'
        ) THEN
            ALTER TABLE "Students" RENAME COLUMN "isActive" TO "IsActive";
        END IF;

        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "TeacherClassSection" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "SubjectMarks" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "studentTransferCertificateLogs" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "StudentFees" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "StudentEnrollments" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "studentCharacterCertificateLogs" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "Payments" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "FeeTypes" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "FeeStructures" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "FeeAdjustments" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "ExamResults" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "Courses" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "ClassRooms" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "ClassCourses" ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT FALSE;

        CREATE TABLE IF NOT EXISTS "Schools" (
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

    END IF;
END $$;


DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = '20260416171923_add_classcourse_requirement_flags'
    ) THEN

        ALTER TABLE "ClassCourses" ALTER COLUMN "TheoryFullMarks" DROP NOT NULL;
        ALTER TABLE "ClassCourses" ALTER COLUMN "TheoryCreditHour" DROP NOT NULL;
        ALTER TABLE "ClassCourses" ALTER COLUMN "PracticalFullMarks" DROP NOT NULL;
        ALTER TABLE "ClassCourses" ALTER COLUMN "PracticalCreditHour" DROP NOT NULL;

        ALTER TABLE "ClassCourses" ADD COLUMN IF NOT EXISTS "IsPracticalRequired" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "ClassCourses" ADD COLUMN IF NOT EXISTS "IsTheoryRequired" boolean NOT NULL DEFAULT FALSE;

        INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
        VALUES ('20260416171923_add_classcourse_requirement_flags', '8.0.15');

    END IF;
END $$;


DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = '20260419105532_academix_details_alter'
    ) THEN

        IF EXISTS (
            SELECT 1
            FROM information_schema.columns
            WHERE table_name = 'AcademicYears'
              AND column_name = 'StartDate'
        ) AND NOT EXISTS (
            SELECT 1
            FROM information_schema.columns
            WHERE table_name = 'AcademicYears'
              AND column_name = 'StartDateEn'
        ) THEN
            ALTER TABLE "AcademicYears" RENAME COLUMN "StartDate" TO "StartDateEn";
        END IF;

        IF EXISTS (
            SELECT 1
            FROM information_schema.columns
            WHERE table_name = 'AcademicYears'
              AND column_name = 'EndDate'
        ) AND NOT EXISTS (
            SELECT 1
            FROM information_schema.columns
            WHERE table_name = 'AcademicYears'
              AND column_name = 'EndDateEn'
        ) THEN
            ALTER TABLE "AcademicYears" RENAME COLUMN "EndDate" TO "EndDateEn";
        END IF;

        ALTER TABLE "AcademicYears" ADD COLUMN IF NOT EXISTS "EndDateNp" text NOT NULL DEFAULT '';
        ALTER TABLE "AcademicYears" ADD COLUMN IF NOT EXISTS "StartDateNp" text NOT NULL DEFAULT '';

        INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
        VALUES ('20260419105532_academix_details_alter', '8.0.15');

    END IF;
END $$;


DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = '20260419111005_startDate_endDate_alter'
    ) THEN

        ALTER TABLE "AcademicYears"
        ALTER COLUMN "StartDateEn" TYPE text
        USING "StartDateEn"::text;

        ALTER TABLE "AcademicYears"
        ALTER COLUMN "EndDateEn" TYPE text
        USING "EndDateEn"::text;

        INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
        VALUES ('20260419111005_startDate_endDate_alter', '8.0.15');

    END IF;
END $$;


DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = '20260419181157_teacher_entity'
    ) THEN

        IF EXISTS (
            SELECT 1
            FROM information_schema.tables
            WHERE table_name = 'TeacherClassSection'
        ) AND NOT EXISTS (
            SELECT 1
            FROM information_schema.tables
            WHERE table_name = 'TeacherClassSections'
        ) THEN

            IF EXISTS (
                SELECT 1
                FROM information_schema.table_constraints
                WHERE table_name = 'TeacherClassSection'
                  AND constraint_name = 'FK_TeacherClassSection_ClassSections_ClassSectionId'
            ) THEN
                ALTER TABLE "TeacherClassSection" DROP CONSTRAINT "FK_TeacherClassSection_ClassSections_ClassSectionId";
            END IF;

            IF EXISTS (
                SELECT 1
                FROM information_schema.table_constraints
                WHERE table_name = 'TeacherClassSection'
                  AND constraint_name = 'FK_TeacherClassSection_Teachers_TeacherId'
            ) THEN
                ALTER TABLE "TeacherClassSection" DROP CONSTRAINT "FK_TeacherClassSection_Teachers_TeacherId";
            END IF;

            IF EXISTS (
                SELECT 1
                FROM information_schema.table_constraints
                WHERE table_name = 'TeacherClassSection'
                  AND constraint_name = 'PK_TeacherClassSection'
            ) THEN
                ALTER TABLE "TeacherClassSection" DROP CONSTRAINT "PK_TeacherClassSection";
            END IF;

            DROP INDEX IF EXISTS "IX_TeacherClassSection_TeacherId";

            ALTER TABLE "TeacherClassSection" RENAME TO "TeacherClassSections";

            IF EXISTS (
                SELECT 1
                FROM pg_indexes
                WHERE schemaname = current_schema()
                  AND indexname = 'IX_TeacherClassSection_ClassSectionId'
            ) THEN
                ALTER INDEX "IX_TeacherClassSection_ClassSectionId" RENAME TO "IX_TeacherClassSections_ClassSectionId";
            END IF;
        END IF;

        ALTER TABLE "Teachers" ALTER COLUMN "LastName" TYPE character varying(100);
        ALTER TABLE "Teachers" ALTER COLUMN "FirstName" TYPE character varying(100);
        ALTER TABLE "Teachers" ALTER COLUMN "Email" TYPE character varying(150);
        ALTER TABLE "Teachers" ALTER COLUMN "ContactNumber" TYPE character varying(20);
        ALTER TABLE "Teachers" ALTER COLUMN "Age" DROP NOT NULL;
        ALTER TABLE "Teachers" ALTER COLUMN "Address" TYPE character varying(300);

        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "AlternateContactNumber" text NOT NULL DEFAULT '';
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "DateOfBirthEn" text NOT NULL DEFAULT '';
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "DateOfBirthNp" text NOT NULL DEFAULT '';
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "Designation" character varying(100) NOT NULL DEFAULT '';
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "DistrictId" integer;
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "EmployeeCode" character varying(50) NOT NULL DEFAULT '';
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "InactiveDate" timestamp with time zone;
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "InactiveReason" text NOT NULL DEFAULT '';
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "JoiningDateEn" text NOT NULL DEFAULT '';
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "JoiningDateNp" text NOT NULL DEFAULT '';
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "MiddleName" character varying(100) NOT NULL DEFAULT '';
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "MunicipalityId" integer;
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "ProvinceId" integer;
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "UserId" uuid;
        ALTER TABLE "Teachers" ADD COLUMN IF NOT EXISTS "WardNo" integer;

        ALTER TABLE "SubjectMarks" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "SubjectMarks" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "SubjectMarks" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "studentTransferCertificateLogs" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "studentTransferCertificateLogs" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "studentTransferCertificateLogs" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "Students" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "Students" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "Students" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "StudentFees" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "StudentFees" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "StudentFees" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "StudentEnrollments" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "StudentEnrollments" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "StudentEnrollments" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "studentCharacterCertificateLogs" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "studentCharacterCertificateLogs" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "studentCharacterCertificateLogs" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "Schools" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "Schools" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "Schools" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "Payments" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "Payments" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "Payments" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "FeeTypes" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "FeeTypes" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "FeeTypes" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "FeeStructures" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "FeeStructures" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "FeeStructures" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "FeeAdjustments" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "FeeAdjustments" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "FeeAdjustments" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "ExamResults" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "ExamResults" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "ExamResults" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "Courses" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "Courses" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "Courses" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "ClassRooms" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "ClassRooms" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "ClassRooms" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "ClassCourses" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "ClassCourses" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "ClassCourses" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "AcademicYears" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "AcademicYears" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "AcademicYears" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;

        ALTER TABLE "TeacherClassSections" ADD COLUMN IF NOT EXISTS "AcademicYearId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
        ALTER TABLE "TeacherClassSections" ADD COLUMN IF NOT EXISTS "CourseId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
        ALTER TABLE "TeacherClassSections" ADD COLUMN IF NOT EXISTS "DeletedBy" uuid;
        ALTER TABLE "TeacherClassSections" ADD COLUMN IF NOT EXISTS "DeletedOn" timestamp with time zone;
        ALTER TABLE "TeacherClassSections" ADD COLUMN IF NOT EXISTS "EffectiveFrom" timestamp with time zone;
        ALTER TABLE "TeacherClassSections" ADD COLUMN IF NOT EXISTS "EffectiveTo" timestamp with time zone;
        ALTER TABLE "TeacherClassSections" ADD COLUMN IF NOT EXISTS "IsClassTeacher" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "TeacherClassSections" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;
        ALTER TABLE "TeacherClassSections" ADD COLUMN IF NOT EXISTS "Remarks" character varying(300) NOT NULL DEFAULT '';

        IF NOT EXISTS (
            SELECT 1
            FROM information_schema.table_constraints
            WHERE table_name = 'TeacherClassSections'
              AND constraint_name = 'PK_TeacherClassSections'
        ) THEN
            ALTER TABLE "TeacherClassSections" ADD CONSTRAINT "PK_TeacherClassSections" PRIMARY KEY ("Id");
        END IF;

        CREATE TABLE IF NOT EXISTS "TeacherDocuments" (
            "Id" uuid NOT NULL,
            "TeacherId" uuid NOT NULL,
            "DocumentType" character varying(80) NOT NULL,
            "DocumentTitle" character varying(150) NOT NULL,
            "FilePath" character varying(500) NOT NULL,
            "OriginalFileName" character varying(255) NOT NULL,
            "MimeType" character varying(100) NOT NULL,
            "FileSize" bigint NOT NULL,
            "UploadedDate" timestamp with time zone NOT NULL,
            "IsActive" boolean NOT NULL,
            "IsDeleted" boolean NOT NULL,
            "CreatedDate" timestamp with time zone NOT NULL,
            "ModifiedDate" timestamp with time zone NOT NULL,
            "DeletedOn" timestamp with time zone,
            "CreatedBy" uuid NOT NULL,
            "ModifiedBy" uuid NOT NULL,
            "DeletedBy" uuid,
            CONSTRAINT "PK_TeacherDocuments" PRIMARY KEY ("Id")
        );

        CREATE TABLE IF NOT EXISTS "TeacherExperiences" (
            "Id" uuid NOT NULL,
            "TeacherId" uuid NOT NULL,
            "OrganizationName" character varying(200) NOT NULL,
            "Designation" character varying(100) NOT NULL,
            "SubjectOrDepartment" character varying(100) NOT NULL,
            "StartDate" timestamp with time zone NOT NULL,
            "EndDate" timestamp with time zone,
            "IsCurrent" boolean NOT NULL,
            "Remarks" character varying(300) NOT NULL,
            "IsActive" boolean NOT NULL,
            "IsDeleted" boolean NOT NULL,
            "CreatedDate" timestamp with time zone NOT NULL,
            "ModifiedDate" timestamp with time zone NOT NULL,
            "DeletedOn" timestamp with time zone,
            "CreatedBy" uuid NOT NULL,
            "ModifiedBy" uuid NOT NULL,
            "DeletedBy" uuid,
            CONSTRAINT "PK_TeacherExperiences" PRIMARY KEY ("Id")
        );

        CREATE TABLE IF NOT EXISTS "TeacherQualifications" (
            "Id" uuid NOT NULL,
            "TeacherId" uuid NOT NULL,
            "DegreeName" character varying(150) NOT NULL,
            "InstitutionName" character varying(200) NOT NULL,
            "BoardOrUniversity" character varying(150) NOT NULL,
            "PassedYear" character varying(20) NOT NULL,
            "GradeOrPercentage" character varying(50) NOT NULL,
            "MajorSubject" character varying(100) NOT NULL,
            "Remarks" character varying(300) NOT NULL,
            "IsActive" boolean NOT NULL,
            "IsDeleted" boolean NOT NULL,
            "CreatedDate" timestamp with time zone NOT NULL,
            "ModifiedDate" timestamp with time zone NOT NULL,
            "DeletedOn" timestamp with time zone,
            "CreatedBy" uuid NOT NULL,
            "ModifiedBy" uuid NOT NULL,
            "DeletedBy" uuid,
            CONSTRAINT "PK_TeacherQualifications" PRIMARY KEY ("Id")
        );

        CREATE UNIQUE INDEX IF NOT EXISTS "IX_Teachers_Email"
            ON "Teachers" ("Email")
            WHERE "Email" IS NOT NULL AND "Email" <> '';

        CREATE UNIQUE INDEX IF NOT EXISTS "IX_Teachers_EmployeeCode"
            ON "Teachers" ("EmployeeCode")
            WHERE "EmployeeCode" IS NOT NULL AND "EmployeeCode" <> '';

        CREATE INDEX IF NOT EXISTS "IX_TCS_Acad_ClassSec_Course_Del"
            ON "TeacherClassSections" ("AcademicYearId", "ClassSectionId", "CourseId", "IsDeleted");

        CREATE INDEX IF NOT EXISTS "IX_TCS_Acad_ClassSec_ClassTeacher_ActDel"
            ON "TeacherClassSections" ("AcademicYearId", "ClassSectionId", "IsClassTeacher", "IsActive", "IsDeleted");

        CREATE INDEX IF NOT EXISTS "IX_TeacherClassSections_CourseId"
            ON "TeacherClassSections" ("CourseId");

        CREATE INDEX IF NOT EXISTS "IX_TCS_Teacher_Acad_ClassSec_Course_Del"
            ON "TeacherClassSections" ("TeacherId", "AcademicYearId", "ClassSectionId", "CourseId", "IsDeleted");

        CREATE INDEX IF NOT EXISTS "IX_TeacherDocuments_TeacherId_DocumentType_IsDeleted"
            ON "TeacherDocuments" ("TeacherId", "DocumentType", "IsDeleted");

        CREATE INDEX IF NOT EXISTS "IX_TeacherExperiences_TeacherId"
            ON "TeacherExperiences" ("TeacherId");

        CREATE INDEX IF NOT EXISTS "IX_TQ_Teacher_Degree_Inst_Year_Del"
            ON "TeacherQualifications" ("TeacherId", "DegreeName", "InstitutionName", "PassedYear", "IsDeleted");

        IF NOT EXISTS (
            SELECT 1
            FROM information_schema.table_constraints
            WHERE table_name = 'TeacherDocuments'
              AND constraint_name = 'FK_TeacherDocuments_Teachers_TeacherId'
        ) THEN
            ALTER TABLE "TeacherDocuments"
            ADD CONSTRAINT "FK_TeacherDocuments_Teachers_TeacherId"
            FOREIGN KEY ("TeacherId") REFERENCES "Teachers" ("Id") ON DELETE CASCADE;
        END IF;

        IF NOT EXISTS (
            SELECT 1
            FROM information_schema.table_constraints
            WHERE table_name = 'TeacherExperiences'
              AND constraint_name = 'FK_TeacherExperiences_Teachers_TeacherId'
        ) THEN
            ALTER TABLE "TeacherExperiences"
            ADD CONSTRAINT "FK_TeacherExperiences_Teachers_TeacherId"
            FOREIGN KEY ("TeacherId") REFERENCES "Teachers" ("Id") ON DELETE CASCADE;
        END IF;

        IF NOT EXISTS (
            SELECT 1
            FROM information_schema.table_constraints
            WHERE table_name = 'TeacherQualifications'
              AND constraint_name = 'FK_TeacherQualifications_Teachers_TeacherId'
        ) THEN
            ALTER TABLE "TeacherQualifications"
            ADD CONSTRAINT "FK_TeacherQualifications_Teachers_TeacherId"
            FOREIGN KEY ("TeacherId") REFERENCES "Teachers" ("Id") ON DELETE CASCADE;
        END IF;

        IF NOT EXISTS (
            SELECT 1
            FROM information_schema.table_constraints
            WHERE table_name = 'TeacherClassSections'
              AND constraint_name = 'FK_TeacherClassSections_AcademicYears_AcademicYearId'
        ) THEN
            ALTER TABLE "TeacherClassSections"
            ADD CONSTRAINT "FK_TeacherClassSections_AcademicYears_AcademicYearId"
            FOREIGN KEY ("AcademicYearId") REFERENCES "AcademicYears" ("Id") ON DELETE CASCADE;
        END IF;

        IF NOT EXISTS (
            SELECT 1
            FROM information_schema.table_constraints
            WHERE table_name = 'TeacherClassSections'
              AND constraint_name = 'FK_TeacherClassSections_ClassSections_ClassSectionId'
        ) THEN
            ALTER TABLE "TeacherClassSections"
            ADD CONSTRAINT "FK_TeacherClassSections_ClassSections_ClassSectionId"
            FOREIGN KEY ("ClassSectionId") REFERENCES "ClassSections" ("Id") ON DELETE CASCADE;
        END IF;

        IF NOT EXISTS (
            SELECT 1
            FROM information_schema.table_constraints
            WHERE table_name = 'TeacherClassSections'
              AND constraint_name = 'FK_TeacherClassSections_Courses_CourseId'
        ) THEN
            ALTER TABLE "TeacherClassSections"
            ADD CONSTRAINT "FK_TeacherClassSections_Courses_CourseId"
            FOREIGN KEY ("CourseId") REFERENCES "Courses" ("Id") ON DELETE CASCADE;
        END IF;

        IF NOT EXISTS (
            SELECT 1
            FROM information_schema.table_constraints
            WHERE table_name = 'TeacherClassSections'
              AND constraint_name = 'FK_TeacherClassSections_Teachers_TeacherId'
        ) THEN
            ALTER TABLE "TeacherClassSections"
            ADD CONSTRAINT "FK_TeacherClassSections_Teachers_TeacherId"
            FOREIGN KEY ("TeacherId") REFERENCES "Teachers" ("Id") ON DELETE CASCADE;
        END IF;

        INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
        VALUES ('20260419181157_teacher_entity', '8.0.15');

    END IF;
END $$;


DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = '20260419182857_teacher_entity_Alter'
    ) THEN

        ALTER TABLE "Teachers" DROP COLUMN IF EXISTS "Address";
        ALTER TABLE "TeacherExperiences" DROP COLUMN IF EXISTS "EndDate";
        ALTER TABLE "TeacherExperiences" DROP COLUMN IF EXISTS "StartDate";

        ALTER TABLE "TeacherExperiences" ADD COLUMN IF NOT EXISTS "EndDateEn" text NOT NULL DEFAULT '';
        ALTER TABLE "TeacherExperiences" ADD COLUMN IF NOT EXISTS "EndDateNp" text NOT NULL DEFAULT '';
        ALTER TABLE "TeacherExperiences" ADD COLUMN IF NOT EXISTS "StartDateEn" text NOT NULL DEFAULT '';
        ALTER TABLE "TeacherExperiences" ADD COLUMN IF NOT EXISTS "StartDateNp" text NOT NULL DEFAULT '';

        INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
        VALUES ('20260419182857_teacher_entity_Alter', '8.0.15');

    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = '20260420161421_teacher_entity_InActive_Reason_Alter'
    ) THEN

        ALTER TABLE "Teachers"
        ALTER COLUMN "InactiveReason" DROP NOT NULL;

        INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
        VALUES ('20260420161421_teacher_entity_InActive_Reason_Alter', '8.0.15');

    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = '20260426050942_Academic_Year_Relation'
    ) THEN

        ALTER TABLE "FeeStructures"
        ADD "AcademicYearId" uuid NOT NULL
        DEFAULT '00000000-0000-0000-0000-000000000000';

        CREATE INDEX "IX_FeeStructures_AcademicYearId"
        ON "FeeStructures" ("AcademicYearId");

        ALTER TABLE "FeeStructures"
        ADD CONSTRAINT "FK_FeeStructures_AcademicYears_AcademicYearId"
        FOREIGN KEY ("AcademicYearId")
        REFERENCES "AcademicYears" ("Id")
        ON DELETE CASCADE;

        INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
        VALUES ('20260426050942_Academic_Year_Relation', '8.0.15');

    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = '20260430090000_AddRoleBasedPermissions'
    ) THEN

        CREATE TABLE IF NOT EXISTS "Permissions" (
            "Id" uuid NOT NULL,
            "Code" character varying(100) NOT NULL,
            "Name" character varying(150) NOT NULL,
            "GroupName" character varying(100) NOT NULL,
            "Description" character varying(500),
            "IsActive" boolean NOT NULL DEFAULT true,
            "IsDeleted" boolean NOT NULL DEFAULT false,
            "CreatedDate" timestamp with time zone NOT NULL DEFAULT NOW(),
            "ModifiedDate" timestamp with time zone NOT NULL DEFAULT NOW(),
            "DeletedOn" timestamp with time zone,
            "CreatedBy" uuid NOT NULL,
            "ModifiedBy" uuid NOT NULL,
            "DeletedBy" uuid,
            CONSTRAINT "PK_Permissions" PRIMARY KEY ("Id")
        );

        CREATE TABLE IF NOT EXISTS "RolePermissions" (
            "Id" uuid NOT NULL,
            "RoleId" uuid NOT NULL,
            "PermissionId" uuid NOT NULL,
            "IsActive" boolean NOT NULL DEFAULT true,
            "IsDeleted" boolean NOT NULL DEFAULT false,
            "CreatedDate" timestamp with time zone NOT NULL DEFAULT NOW(),
            "ModifiedDate" timestamp with time zone NOT NULL DEFAULT NOW(),
            "DeletedOn" timestamp with time zone,
            "CreatedBy" uuid NOT NULL,
            "ModifiedBy" uuid NOT NULL,
            "DeletedBy" uuid,
            CONSTRAINT "PK_RolePermissions" PRIMARY KEY ("Id"),
            CONSTRAINT "FK_RolePermissions_AspNetRoles_RoleId"
                FOREIGN KEY ("RoleId")
                REFERENCES "AspNetRoles" ("Id")
                ON DELETE CASCADE,
            CONSTRAINT "FK_RolePermissions_Permissions_PermissionId"
                FOREIGN KEY ("PermissionId")
                REFERENCES "Permissions" ("Id")
                ON DELETE CASCADE
        );

        CREATE UNIQUE INDEX IF NOT EXISTS "IX_Permissions_Code"
            ON "Permissions" ("Code");

        CREATE INDEX IF NOT EXISTS "IX_RolePermissions_PermissionId"
            ON "RolePermissions" ("PermissionId");

        CREATE UNIQUE INDEX IF NOT EXISTS "IX_RolePermissions_RoleId_PermissionId"
            ON "RolePermissions" ("RoleId", "PermissionId");

        INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
        VALUES ('20260430090000_AddRoleBasedPermissions', '8.0.15');

    END IF;
END $$;