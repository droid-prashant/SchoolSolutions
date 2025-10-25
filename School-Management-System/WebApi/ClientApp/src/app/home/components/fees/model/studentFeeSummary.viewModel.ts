import { StudentFeeDetailViewModel } from "./studentFeeDetail.viewModel";

export interface StudentFeeSummaryViewModel {
  studentId: string;
  studentName: string;
  className: string;
  sectionName: string;
  totalFees: number;
  totalPaid: number;
  totalPending: number;
  feeDetails: StudentFeeDetailViewModel[];
}
