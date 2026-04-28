export interface PromotionCandidateViewModel {
  studentEnrollmentId: string;
  studentName: string;
  rollNumber?: number | null;
  currentClassName: string;
  currentSectionName: string;
  examType: number;
  gpa?: number | null;
  resultStatus: string;
  isPromotable: boolean;
  isAlreadyPromoted: boolean;
  targetClassName: string;
  targetSectionName: string;
  remarks: string;
}
