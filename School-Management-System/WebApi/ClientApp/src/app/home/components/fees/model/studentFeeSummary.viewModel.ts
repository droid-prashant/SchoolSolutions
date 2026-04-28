import { StudentFeeDetailViewModel } from "./studentFeeDetail.viewModel";
import { PreviousYearDueViewModel } from "./previousYearDue.viewModel";

export interface StudentFeeSummaryViewModel {
  studentEnrollmentId: string;
  academicYearName: string;
  studentName: string;
  className: string;
  sectionName: string;
  totalFees: number;
  totalDiscount: number;
  totalFine: number;
  netFees: number;
  totalPaid: number;
  totalPending: number;
  previousYearPending: number;
  grandTotalPending: number;
  feeDetails: StudentFeeDetailViewModel[];
  previousYearDues: PreviousYearDueViewModel[];
  previousYearFeeDetails: StudentFeeDetailViewModel[];
}
