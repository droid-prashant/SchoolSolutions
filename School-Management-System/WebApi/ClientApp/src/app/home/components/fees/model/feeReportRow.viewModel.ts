export interface FeeReportRowViewModel {
  studentEnrollmentId: string;
  studentName: string;
  rollNumber?: number | null;
  totalFees: number;
  totalDiscount: number;
  totalFine: number;
  netFees: number;
  totalPaid: number;
  totalPending: number;
  previousYearPending: number;
  grandTotalPending: number;
  hasPendingFees: boolean;
}
