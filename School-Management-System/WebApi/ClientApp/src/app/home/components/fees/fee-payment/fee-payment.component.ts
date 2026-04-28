import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { ApiService } from '../../../../shared/api.service';
import { MessageService } from 'primeng/api';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { StudentFeeSummaryViewModel } from '../model/studentFeeSummary.viewModel';
import { AuthService } from '../../../../shared/auth.service';
import { LookupService } from '../../../../shared/common/lookup.service';
import { FilterSelection } from '../../student-filter/student-filter.component';
import { StudentFeeDetailViewModel } from '../model/studentFeeDetail.viewModel';
import { FeeAdjustmentDto } from '../model/feeAdjustment.dto';

@Component({
  selector: 'app-fee-payment',
  standalone: false,
  templateUrl: './fee-payment.component.html',
  styleUrl: './fee-payment.component.scss'
})
export class FeePaymentComponent implements OnInit {
  students: StudentViewModel[] = [];
  selectedFees: StudentFeeDetailViewModel[] = [];
  selectedCurrentYearFees: StudentFeeDetailViewModel[] = [];
  selectedPreviousYearFees: StudentFeeDetailViewModel[] = [];

  paymentForm: FormGroup;
  adjustmentForm: FormGroup;
  studentFeeSummary: StudentFeeSummaryViewModel | null = null;
  classSectionId: string = "";
  currentFilter: FilterSelection = {};

  isFeeSummaryFetched: boolean = false;

  selectedStudentEnrollId: string = '';
  currentAcademicYearName: string = '';
  hasLoadedStudentList: boolean = false;
  isPaying: boolean = false;
  isApplyingAdjustment: boolean = false;

  paymentModes = [
    { id: 'cash', name: 'Cash' },
    { id: 'bank', name: 'Bank Transfer' },
    { id: 'khalti', name: 'Khalti' }
  ];

  concessionTypes = [
    { id: 'Scholarship', name: 'Scholarship' },
    { id: 'Disability Support', name: 'Disability Support' },
    { id: 'Financial Hardship', name: 'Financial Hardship' },
    { id: 'Sibling Discount', name: 'Sibling Discount' },
    { id: 'Staff Child', name: 'Staff Child' },
    { id: 'Special Management', name: 'Special Management' },
    { id: 'Late Fine', name: 'Late Fine' }
  ];

  constructor(
    private _fb: FormBuilder,
    private _apiService: ApiService,
    private _messageService: MessageService,
    private _authService: AuthService,
    private _lookupService: LookupService
  ) {
    this.paymentForm = this._fb.group({
      paymentMode: ['cash', Validators.required],
      fees: this._fb.array([])
    });

    this.adjustmentForm = this._fb.group({
      adjustmentFeeId: [''],
      concessionType: ['', Validators.required],
      discountAmount: [0, [Validators.min(0)]],
      fineAmount: [0, [Validators.min(0)]],
      adjustmentRemark: ['']
    });
  }

  ngOnInit() {
    const academicYearId = this._authService.getCurrentAcademicYearId();
    if (academicYearId) {
      this._lookupService.getAcademicYearById(academicYearId).subscribe({
        next: (academicYear) => {
          this.currentAcademicYearName = academicYear?.yearName ?? '';
        }
      });
    }
  }

  get fees(): FormArray {
    return this.paymentForm.get('fees') as FormArray;
  }

  get selectedPaymentMode(): string {
    return this.paymentForm.get('paymentMode')?.value || 'cash';
  }

  setFees() {
    this.selectedFees.forEach(fee => {
      this.fees.push(this._fb.group({
        amount: [fee.pendingAmount, [Validators.required, Validators.min(0.01)]]
      }));
    });
  }

  onLoadStudents(filter: FilterSelection) {
    this.currentFilter = filter;
    this.students = [];
    this.selectedStudentEnrollId = '';
    this.classSectionId = filter.classSectionId ?? '';
    this.isFeeSummaryFetched = false;
    this.selectedFees = [];
    this.selectedCurrentYearFees = [];
    this.selectedPreviousYearFees = [];
    this.hasLoadedStudentList = false;
    this.fees.clear();
    this.paymentForm.patchValue({
      paymentMode: 'cash',
    });
    this.adjustmentForm.patchValue({
      adjustmentFeeId: '',
      concessionType: '',
      discountAmount: 0,
      fineAmount: 0,
      adjustmentRemark: ''
    });

    if (filter.classSectionId) {
      this.getStudentByClassSection(filter.classSectionId);
    }
  }

  getStudentByClassSection(classSectionId: string) {
    this._apiService.getStudentsByClassSectionId(classSectionId).subscribe({
      next: (response) => {
        this.students = response.map(student => ({
          ...student,
          displayName: `${student.firstName ?? ''} ${student.lastName ?? ''}`.trim()
        })) as StudentViewModel[];
        this.hasLoadedStudentList = true;
      },
      error: (err) => console.log(err),
    });
  }

  onStudentChange(event: any) {
    const studentEnrollmentId = event.value;
    this.selectedStudentEnrollId = studentEnrollmentId;
    this.isFeeSummaryFetched = false;
    this.selectedFees = [];
    this.selectedCurrentYearFees = [];
    this.selectedPreviousYearFees = [];
    this.fees.clear();
    if (studentEnrollmentId) {
      this.getStudentFeeSummary(studentEnrollmentId);
    } else {
      this.studentFeeSummary = null;
    }
  }

  getStudentFeeSummary(studentId: string) {
    this._apiService.getStudentFeeSummary(studentId, this.classSectionId).subscribe({
      next: (response) => {
        this.studentFeeSummary = response;
        this.isFeeSummaryFetched = !!this.studentFeeSummary;
      },
      error: (err) => {
        this.isFeeSummaryFetched = false;
        console.log(err);
      },
      complete: () => console.log("Request is complete")
    });
  }

  getFormGroupAt(index: number): FormGroup {
    return this.fees.at(index) as FormGroup;
  }

  onSelectionChange(event: any) {
    this.selectedCurrentYearFees = (event ?? []).filter((fee: StudentFeeDetailViewModel) => !fee.isPaid);
    this.rebuildSelectedFees();
  }

  onPreviousYearSelectionChange(event: any) {
    this.selectedPreviousYearFees = (event ?? []).filter((fee: StudentFeeDetailViewModel) => !fee.isPaid);
    this.rebuildSelectedFees();
  }

  getSelectedTotal(): number {
    const updatedFees = this.fees.value as { amount: number }[];
    return updatedFees?.reduce((total, fee) => total + (Number(fee.amount) || 0), 0) || 0;
  }

  selectAllPendingFees() {
    if (!this.studentFeeSummary?.feeDetails) return;
    this.selectedCurrentYearFees = this.studentFeeSummary.feeDetails.filter(f => !f.isPaid);
    this.rebuildSelectedFees();
  }

  selectAllPreviousYearPendingFees() {
    this.selectedPreviousYearFees = (this.studentFeeSummary?.previousYearFeeDetails || [])
      .filter(fee => !fee.isPaid && fee.pendingAmount > 0);
    this.rebuildSelectedFees();
  }

  removeSelectedFee(feeId: string) {
    this.selectedCurrentYearFees = this.selectedCurrentYearFees.filter(x => x.id !== feeId);
    this.selectedPreviousYearFees = this.selectedPreviousYearFees.filter(x => x.id !== feeId);
    this.rebuildSelectedFees();
  }

  applyAdjustment() {
    const studentFeeId = this.adjustmentForm.get('adjustmentFeeId')?.value;
    const concessionType = this.adjustmentForm.get('concessionType')?.value;
    const discountAmount = Number(this.adjustmentForm.get('discountAmount')?.value) || 0;
    const fineAmount = Number(this.adjustmentForm.get('fineAmount')?.value) || 0;
    const remark = (this.adjustmentForm.get('adjustmentRemark')?.value || '').trim();

    if (!studentFeeId) {
      this._messageService.add({ severity: 'warn', summary: 'Select Fee', detail: 'Select a fee row to apply the concession or fine.' });
      return;
    }

    if (!concessionType) {
      this._messageService.add({ severity: 'warn', summary: 'Select Type', detail: 'Select a concession or adjustment type.' });
      return;
    }

    if (discountAmount <= 0 && fineAmount <= 0) {
      this._messageService.add({ severity: 'warn', summary: 'Enter Amount', detail: 'Enter a discount amount or a fine amount.' });
      return;
    }

    const reason = remark ? `${concessionType}: ${remark}` : concessionType;
    const request: FeeAdjustmentDto = {
      studentFeeId,
      discountAmount,
      fineAmount,
      reason
    };

    this.isApplyingAdjustment = true;
    this._apiService.applyFeeAdjustment(request).pipe(
      finalize(() => {
        this.isApplyingAdjustment = false;
      })
    ).subscribe({
      next: () => {
        this._messageService.add({
          severity: 'success',
          summary: 'Adjustment Applied',
          detail: 'The concession or fine has been applied to the selected fee.'
        });
        this.adjustmentForm.patchValue({
          adjustmentFeeId: '',
          concessionType: '',
          discountAmount: 0,
          fineAmount: 0,
          adjustmentRemark: ''
        });
        this.getStudentFeeSummary(this.selectedStudentEnrollId);
      },
      error: (err) => {
        this._messageService.add({
          severity: 'error',
          summary: 'Adjustment Failed',
          detail: err?.error?.message || err?.error || 'Unable to apply the fee adjustment.'
        });
      }
    });
  }

  paySelectedFees() {
    if (this.selectedFees.length === 0) {
      this._messageService.add({ severity: 'warn', summary: 'No Fees Selected', detail: 'Please select fees to pay.' });
      return;
    }

    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      this._messageService.add({ severity: 'warn', summary: 'Invalid Payment', detail: 'Please correct the payment values before continuing.' });
      return;
    }

    const updatedFees = this.fees.value as { amount: number }[];
    const invalidEntry = updatedFees.find((fee, index) => {
      const amount = Number(fee.amount) || 0;
      return amount <= 0 || amount > this.selectedFees[index].pendingAmount;
    });

    if (invalidEntry) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Invalid Amount',
        detail: 'Each payment amount must be greater than zero and not exceed the pending amount.'
      });
      return;
    }

    const paymentRequests = updatedFees
      .map((fee, index) => ({
        feeId: this.selectedFees[index].id,
        amount: Number(fee.amount) || 0
      }))
      .filter(fee => fee.amount > 0)
      .map(fee => this._apiService.payStudentFee(fee.feeId, this.selectedStudentEnrollId, fee.amount, this.selectedPaymentMode));

    if (!paymentRequests.length) {
      this._messageService.add({ severity: 'warn', summary: 'No Payment Amount', detail: 'Enter at least one payment amount to continue.' });
      return;
    }

    this.isPaying = true;
    forkJoin(paymentRequests).pipe(
      finalize(() => {
        this.isPaying = false;
      })
    ).subscribe({
      next: () => {
        this._messageService.add({ severity: 'success', summary: 'Payment Successful', detail: 'Selected fee payments have been recorded.' });
        this.selectedFees = [];
        this.selectedCurrentYearFees = [];
        this.selectedPreviousYearFees = [];
        this.fees.clear();
        this.getStudentFeeSummary(this.selectedStudentEnrollId);
      },
      error: (err) => {
        this._messageService.add({
          severity: 'error',
          summary: 'Payment Failed',
          detail: err?.error?.message || err?.error || 'Unable to complete the selected fee payments.'
        });
      }
    });
  }

  private rebuildSelectedFees() {
    this.selectedFees = [...this.selectedCurrentYearFees, ...this.selectedPreviousYearFees];
    this.fees.clear();
    this.setFees();
  }
}
