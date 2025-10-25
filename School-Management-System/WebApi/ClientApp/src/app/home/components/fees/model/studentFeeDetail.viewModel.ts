export interface StudentFeeDetailViewModel {
  className: string;
  feeType: string;
  feeMonth?: string; // Use string for date from API (ISO format)
  totalAmount: number;
  paidAmount: number;
  pendingAmount: number;
  isPaid: boolean;
}