export interface ManualStudentChargeDto {
  studentEnrollmentId: string;
  feeStructureId: string;
  amount?: number | null;
}
