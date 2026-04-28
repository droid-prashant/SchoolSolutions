import { FeeReportRowViewModel } from "./feeReportRow.viewModel";

export interface FeeReportViewModel {
  academicYearName: string;
  className: string;
  sectionName: string;
  totalStudents: number;
  totalFees: number;
  totalDiscount: number;
  totalFine: number;
  netFees: number;
  totalPaid: number;
  totalPending: number;
  totalPreviousYearPending: number;
  grandTotalPending: number;
  students: FeeReportRowViewModel[];
}
