export interface PromoteStudentsDto {
  classSectionId: string;
  examType: number;
  targetAcademicYearId: string;
  targetClassSectionId?: string | null;
  promoteAllEligible: boolean;
  studentEnrollmentIds: string[];
}
