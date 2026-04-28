export interface StudentFeeDetailViewModel {
  id: string;
  academicYearName: string;
  className: string;
  feeType: string;
  feeMonth?: string; // Use string for date from API (ISO format)
  discountAmount: number;
  fineAmount: number;
  totalAmount: number;
  netAmount: number;
  paidAmount: number;
  pendingAmount: number;
  isPaid: boolean;
}
