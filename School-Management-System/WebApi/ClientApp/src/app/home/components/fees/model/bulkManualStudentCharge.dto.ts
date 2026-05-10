export interface BulkManualStudentChargeDto {
  classSectionId: string;
  feeStructureId: string;
  amount: number | null;
  studentEnrollmentIds: string[];
}
