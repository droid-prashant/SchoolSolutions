import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { ApiService } from '../../../../shared/api.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { StudentFeeSummaryViewModel } from '../model/studentFeeSummary.viewModel';
import { AuthService } from '../../../../shared/auth.service';
import { LookupService } from '../../../../shared/common/lookup.service';
import { FilterSelection } from '../../student-filter/student-filter.component';
import { StudentFeeDetailViewModel } from '../model/studentFeeDetail.viewModel';
import { FeeAdjustmentDto } from '../model/feeAdjustment.dto';
import { BulkManualChargeResultViewModel } from '../model/bulkManualChargeResult.viewModel';
import { BulkManualStudentChargeDto } from '../model/bulkManualStudentCharge.dto';
import { FeeStructureViewModel } from '../model/feeStructure.viewModel';
import { ManualStudentChargeDto } from '../model/manualStudentCharge.dto';

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
  manualFeeTemplates: FeeStructureViewModel[] = [];

  paymentForm: FormGroup;
  adjustmentForm: FormGroup;
  manualChargeForm: FormGroup;
  bulkChargeForm: FormGroup;
  educationTaxForm: FormGroup;
  studentFeeSummary: StudentFeeSummaryViewModel | null = null;
  classSectionId: string = "";
  currentFilter: FilterSelection = {};

  isFeeSummaryFetched: boolean = false;

  selectedStudentEnrollId: string = '';
  currentAcademicYearName: string = '';
  hasLoadedStudentList: boolean = false;
  isPaying: boolean = false;
  isApplyingAdjustment: boolean = false;
  isAssigningManualCharge: boolean = false;
  isAssigningBulkCharge: boolean = false;

  paymentModes = [
    { id: 'cash', name: 'Cash' },
    { id: 'bank', name: 'Bank Transfer' },
    { id: 'khalti', name: 'Khalti' }
  ];

  adjustmentTypes = [
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
    private _confirmationService: ConfirmationService,
    private _authService: AuthService,
    private _lookupService: LookupService
  ) {
    this.paymentForm = this._fb.group({
      paymentMode: ['cash', Validators.required],
      fees: this._fb.array([])
    });

    this.adjustmentForm = this._fb.group({
      adjustmentFeeId: [''],
      adjustmentType: ['', Validators.required],
      discountAmount: [0, [Validators.min(0)]],
      fineAmount: [0, [Validators.min(0)]],
      adjustmentRemark: ['']
    });

    this.manualChargeForm = this._fb.group({
      feeStructureId: ['', Validators.required],
      amount: [null, [Validators.min(0.01)]]
    });

    this.bulkChargeForm = this._fb.group({
      feeStructureId: ['', Validators.required],
      amount: [null, [Validators.min(0.01)]],
      assignToAll: [true],
      studentEnrollmentIds: [[]]
    });

    this.educationTaxForm = this._fb.group({
      educationTaxPercentage: [0, [Validators.min(0), Validators.max(100)]]
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

  get displayedTotalFees(): number {
    return (this.studentFeeSummary?.feeDetails || []).reduce((sum, fee) => sum + (Number(fee.totalAmount) || 0), 0);
  }

  get displayedTotalDiscount(): number {
    return (this.studentFeeSummary?.feeDetails || []).reduce((sum, fee) => sum + (Number(fee.discountAmount) || 0), 0);
  }

  get displayedTotalFine(): number {
    return (this.studentFeeSummary?.feeDetails || []).reduce((sum, fee) => sum + (Number(fee.fineAmount) || 0), 0);
  }

  get displayedTotalEducationTax(): number {
    return (this.studentFeeSummary?.feeDetails || []).reduce((sum, fee) => sum + (Number(fee.educationTaxAmount) || 0), 0);
  }

  get displayedNetFees(): number {
    return (this.studentFeeSummary?.feeDetails || []).reduce((sum, fee) => sum + (Number(fee.netAmount) || 0), 0);
  }

  get displayedTotalPaid(): number {
    return (this.studentFeeSummary?.feeDetails || []).reduce((sum, fee) => sum + (Number(fee.paidAmount) || 0), 0);
  }

  get displayedCurrentYearDue(): number {
    return (this.studentFeeSummary?.feeDetails || []).reduce((sum, fee) => sum + (Number(fee.pendingAmount) || 0), 0);
  }

  get displayedPreviousYearDue(): number {
    return (this.studentFeeSummary?.previousYearDues || []).reduce((sum, due) => sum + (Number(due.pendingAmount) || 0), 0);
  }

  get displayedGrandTotalDue(): number {
    return this.displayedCurrentYearDue + this.displayedPreviousYearDue;
  }

  get educationTaxEligibleFees(): StudentFeeDetailViewModel[] {
    return (this.studentFeeSummary?.feeDetails || []).filter(fee => this.isEducationTaxEligibleFee(fee) && !fee.isPaid);
  }

  get hasAppliedEducationTax(): boolean {
    return this.educationTaxEligibleFees.some(fee => (Number(fee.educationTaxAmount) || 0) > 0);
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
    this.manualFeeTemplates = [];
    this.fees.clear();
    this.paymentForm.patchValue({
      paymentMode: 'cash',
    });
    this.adjustmentForm.patchValue({
      adjustmentFeeId: '',
      adjustmentType: '',
      discountAmount: 0,
      fineAmount: 0,
      adjustmentRemark: ''
    });
    this.educationTaxForm.patchValue({
      educationTaxPercentage: 0
    });
    this.manualChargeForm.reset({
      feeStructureId: '',
      amount: null
    });
    this.bulkChargeForm.reset({
      feeStructureId: '',
      amount: null,
      assignToAll: true,
      studentEnrollmentIds: []
    });

    if (filter.classSectionId) {
      this.loadManualFeeTemplatesForClassSection(filter.classSectionId);
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
      if (this.classSectionId) {
        this.loadManualFeeTemplatesForClassSection(this.classSectionId);
      } else {
        this.manualFeeTemplates = [];
      }
    }
  }

  getStudentFeeSummary(studentId: string) {
    this._apiService.getStudentFeeSummary(studentId, this.classSectionId).subscribe({
      next: (summary) => {
        this.studentFeeSummary = summary;
        this.isFeeSummaryFetched = !!this.studentFeeSummary;
      },
      error: (err) => {
        this.isFeeSummaryFetched = false;
        console.log(err);
      },
      complete: () => console.log("Request is complete")
    });
  }

  loadManualFeeTemplatesForClassSection(classSectionId: string) {
    this._apiService.getManualFeeTemplatesByClassSection(classSectionId).subscribe({
      next: (templates) => {
        this.manualFeeTemplates = templates ?? [];
      },
      error: (err) => {
        this.manualFeeTemplates = [];
        console.log(err);
      }
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
    const adjustmentType = this.adjustmentForm.get('adjustmentType')?.value;
    const discountAmount = Number(this.adjustmentForm.get('discountAmount')?.value) || 0;
    const fineAmount = Number(this.adjustmentForm.get('fineAmount')?.value) || 0;
    const remark = (this.adjustmentForm.get('adjustmentRemark')?.value || '').trim();

    if (!studentFeeId) {
      this._messageService.add({ severity: 'warn', summary: 'Select Fee', detail: 'Select a fee row to apply the adjustment or tax.' });
      return;
    }

    if (!adjustmentType) {
      this._messageService.add({ severity: 'warn', summary: 'Select Type', detail: 'Select an adjustment type.' });
      return;
    }

    if (discountAmount <= 0 && fineAmount <= 0) {
      this._messageService.add({ severity: 'warn', summary: 'Enter Value', detail: 'Enter a discount amount or fine amount.' });
      return;
    }

    const reason = remark ? `${adjustmentType}: ${remark}` : adjustmentType;
    const request: FeeAdjustmentDto = {
      studentFeeId,
      discountAmount,
      fineAmount,
      educationTaxPercentage: 0,
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
          detail: 'The adjustment has been applied to the selected fee.'
        });
        this.adjustmentForm.patchValue({
          adjustmentFeeId: '',
          adjustmentType: '',
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

  applyEducationTax(): void {
    if (!this.selectedStudentEnrollId) {
      this._messageService.add({ severity: 'warn', summary: 'Select Student', detail: 'Select a student before applying education tax.' });
      return;
    }

    if (this.educationTaxForm.invalid) {
      this.educationTaxForm.markAllAsTouched();
      this._messageService.add({ severity: 'warn', summary: 'Invalid Percentage', detail: 'Enter a valid education tax percentage.' });
      return;
    }

    const educationTaxPercentage = Number(this.educationTaxForm.get('educationTaxPercentage')?.value) || 0;
    if (educationTaxPercentage <= 0) {
      this._messageService.add({ severity: 'warn', summary: 'Enter Percentage', detail: 'Enter an education tax percentage greater than zero.' });
      return;
    }

    const targetFees = this.educationTaxEligibleFees.filter(fee => (Number(fee.educationTaxAmount) || 0) <= 0);
    if (!targetFees.length) {
      this._messageService.add({ severity: 'info', summary: 'Already Applied', detail: 'Education tax is already applied to the available tuition fees.' });
      return;
    }

    const requests = targetFees.map(fee => this._apiService.applyFeeAdjustment({
      studentFeeId: fee.id,
      discountAmount: 0,
      fineAmount: 0,
      educationTaxPercentage,
      reason: `Education Tax ${educationTaxPercentage}%`
    }));

    this.isApplyingAdjustment = true;
    forkJoin(requests).pipe(
      finalize(() => {
        this.isApplyingAdjustment = false;
      })
    ).subscribe({
      next: () => {
        this._messageService.add({
          severity: 'success',
          summary: 'Education Tax Applied',
          detail: `Education tax has been added to ${targetFees.length} tuition fee item(s).`
        });
        this.clearPaymentSelection();
        this.getStudentFeeSummary(this.selectedStudentEnrollId);
      },
      error: (err) => {
        this._messageService.add({
          severity: 'error',
          summary: 'Education Tax Failed',
          detail: err?.error?.message || err?.error || 'Unable to apply education tax.'
        });
      }
    });
  }

  onManualFeeTemplateChange(event: any) {
    const feeStructureId = event.value;
    const selectedTemplate = this.manualFeeTemplates.find(x => x.id === feeStructureId);
    this.manualChargeForm.patchValue({
      amount: selectedTemplate?.amount ?? null
    }, { emitEvent: false });
  }

  onBulkFeeTemplateChange(event: any) {
    const feeStructureId = event.value;
    const selectedTemplate = this.manualFeeTemplates.find(x => x.id === feeStructureId);
    this.bulkChargeForm.patchValue({
      amount: selectedTemplate?.amount ?? null
    }, { emitEvent: false });
  }

  onBulkAssignModeChange() {
    if (this.bulkChargeForm.get('assignToAll')?.value) {
      this.bulkChargeForm.patchValue({
        studentEnrollmentIds: []
      }, { emitEvent: false });
    }
  }

  getBulkTargetCount(): number {
    if (this.bulkChargeForm.get('assignToAll')?.value) {
      return this.students.length;
    }

    const selectedIds = this.bulkChargeForm.get('studentEnrollmentIds')?.value as string[] | null;
    return selectedIds?.length ?? 0;
  }

  assignManualCharge() {
    if (!this.selectedStudentEnrollId) {
      this._messageService.add({ severity: 'warn', summary: 'Select Student', detail: 'Select a student before assigning a manual charge.' });
      return;
    }

    if (this.manualChargeForm.invalid) {
      this.manualChargeForm.markAllAsTouched();
      this._messageService.add({ severity: 'warn', summary: 'Invalid Charge', detail: 'Select a fee template and valid amount.' });
      return;
    }

    const value = this.manualChargeForm.value;
    const request: ManualStudentChargeDto = {
      studentEnrollmentId: this.selectedStudentEnrollId,
      feeStructureId: value.feeStructureId,
      amount: value.amount ? Number(value.amount) : null
    };

    this.isAssigningManualCharge = true;
    this._apiService.assignManualCharge(request).pipe(
      finalize(() => {
        this.isAssigningManualCharge = false;
      })
    ).subscribe({
      next: () => {
        this._messageService.add({
          severity: 'success',
          summary: 'Charge Assigned',
          detail: 'The manual fee has been assigned to the selected student.'
        });
        this.manualChargeForm.reset({
          feeStructureId: '',
          amount: null
        });
        this.getStudentFeeSummary(this.selectedStudentEnrollId);
      },
      error: (err) => {
        this._messageService.add({
          severity: 'error',
          summary: 'Charge Failed',
          detail: err?.error?.message || err?.error || 'Unable to assign the manual fee.'
        });
      }
    });
  }

  assignBulkManualCharge() {
    if (!this.classSectionId) {
      this._messageService.add({ severity: 'warn', summary: 'Select Class', detail: 'Load a class and section before assigning bulk manual charges.' });
      return;
    }

    if (!this.students.length) {
      this._messageService.add({ severity: 'warn', summary: 'No Students', detail: 'There are no active students available for bulk charge assignment.' });
      return;
    }

    if (this.bulkChargeForm.invalid) {
      this.bulkChargeForm.markAllAsTouched();
      this._messageService.add({ severity: 'warn', summary: 'Invalid Charge', detail: 'Select a fee template and valid amount for bulk assignment.' });
      return;
    }

    const assignToAll = !!this.bulkChargeForm.get('assignToAll')?.value;
    const selectedStudentEnrollmentIds = assignToAll
      ? this.students.map(x => x.studentEnrollmentId).filter((x): x is string => !!x)
      : ((this.bulkChargeForm.get('studentEnrollmentIds')?.value as string[] | null) ?? []);

    if (!selectedStudentEnrollmentIds.length) {
      this._messageService.add({ severity: 'warn', summary: 'Select Students', detail: 'Choose one or more students for the bulk manual charge.' });
      return;
    }

    const request: BulkManualStudentChargeDto = {
      classSectionId: this.classSectionId,
      feeStructureId: this.bulkChargeForm.get('feeStructureId')?.value,
      amount: this.bulkChargeForm.get('amount')?.value ? Number(this.bulkChargeForm.get('amount')?.value) : null,
      studentEnrollmentIds: selectedStudentEnrollmentIds
    };

    this.isAssigningBulkCharge = true;
    this._apiService.assignBulkManualCharge(request).pipe(
      finalize(() => {
        this.isAssigningBulkCharge = false;
      })
    ).subscribe({
      next: (result: BulkManualChargeResultViewModel) => {
        this._messageService.add({
          severity: 'success',
          summary: 'Bulk Charge Completed',
          detail: `Assigned ${result.assignedCount} charge(s). Already assigned: ${result.alreadyAssignedCount}. Invalid: ${result.invalidEnrollmentCount}.`
        });
        this.bulkChargeForm.patchValue({
          feeStructureId: '',
          amount: null,
          assignToAll: true,
          studentEnrollmentIds: []
        });

        if (this.selectedStudentEnrollId) {
          this.getStudentFeeSummary(this.selectedStudentEnrollId);
        }
      },
      error: (err) => {
        this._messageService.add({
          severity: 'error',
          summary: 'Bulk Charge Failed',
          detail: err?.error?.message || err?.error || 'Unable to assign bulk manual charges.'
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

    const receiptLines = updatedFees
      .map((fee, index) => ({
        ...this.selectedFees[index],
        amountPaidNow: Number(fee.amount) || 0
      }))
      .filter(fee => fee.amountPaidNow > 0);

    const receiptStudentName = this.studentFeeSummary?.studentName ?? '';
    const receiptClassName = this.studentFeeSummary?.className ?? '';
    const receiptSectionName = this.studentFeeSummary?.sectionName ?? '';
    const receiptAcademicYearName = this.studentFeeSummary?.academicYearName ?? '';
    const receiptPaymentMode = this.selectedPaymentMode;
    const receiptDate = new Date();

    this.isPaying = true;
    forkJoin(paymentRequests).pipe(
      finalize(() => {
        this.isPaying = false;
      })
    ).subscribe({
      next: () => {
        this._messageService.add({ severity: 'success', summary: 'Payment Successful', detail: 'Selected fee payments have been recorded.' });
        this.clearPaymentSelection();
        this.getStudentFeeSummary(this.selectedStudentEnrollId);
        this.promptReceiptPrint({
          studentName: receiptStudentName,
          className: receiptClassName,
          sectionName: receiptSectionName,
          academicYearName: receiptAcademicYearName,
          paymentMode: receiptPaymentMode,
          paidOn: receiptDate,
          items: receiptLines
        });
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

  private isEducationTaxEligibleFee(fee: StudentFeeDetailViewModel): boolean {
    const feeType = fee?.feeType?.toLowerCase() ?? '';
    return feeType.includes('month') || feeType.includes('tuition') || feeType.includes('tution');
  }

  private clearPaymentSelection(): void {
    this.selectedFees = [];
    this.selectedCurrentYearFees = [];
    this.selectedPreviousYearFees = [];
    this.fees.clear();
  }

  private promptReceiptPrint(receipt: {
    studentName: string;
    className: string;
    sectionName: string;
    academicYearName: string;
    paymentMode: string;
    paidOn: Date;
    items: Array<StudentFeeDetailViewModel & { amountPaidNow: number }>;
  }): void {
    this._confirmationService.confirm({
      header: 'Print Receipt',
      message: 'Do you want to print the payment receipt now?',
      icon: 'pi pi-print',
      acceptLabel: 'Yes',
      rejectLabel: 'No',
      accept: () => this.printReceipt(receipt)
    });
  }

  private printReceipt(receipt: {
    studentName: string;
    className: string;
    sectionName: string;
    academicYearName: string;
    paymentMode: string;
    paidOn: Date;
    items: Array<StudentFeeDetailViewModel & { amountPaidNow: number }>;
  }): void {
    const printWindow = window.open('', '_blank', 'width=900,height=700');
    if (!printWindow) {
      return;
    }

    const rows = receipt.items.map(item => `
      <tr>
        <td>${this.escapeHtml(item.feeType)}</td>
        <td>${this.escapeHtml(item.feeMonth ? new Date(item.feeMonth).toLocaleDateString('en-US', { month: 'short', year: 'numeric' }) : '-')}</td>
        <td>${this.formatCurrency(item.totalAmount)}</td>
        <td>${this.formatCurrency(item.educationTaxAmount || 0)}</td>
        <td>${this.formatCurrency(item.amountPaidNow)}</td>
      </tr>
    `).join('');

    const total = receipt.items.reduce((sum, item) => sum + item.amountPaidNow, 0);

    printWindow.document.write(`
      <html>
        <head>
          <title>Fee Receipt - ${this.escapeHtml(receipt.studentName)}</title>
          <style>
            body { font-family: Arial, sans-serif; color: #1f2937; padding: 24px; }
            h1 { margin: 0 0 4px; font-size: 24px; }
            .muted { color: #64748b; margin-bottom: 18px; }
            .grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 10px 24px; margin-bottom: 20px; }
            .item span { display: block; color: #64748b; font-size: 12px; }
            .item strong { font-size: 14px; }
            table { width: 100%; border-collapse: collapse; margin-top: 8px; }
            th, td { border: 1px solid #dbe5f0; padding: 8px; text-align: left; font-size: 13px; }
            th { background: #f1f6fd; }
            .total { margin-top: 16px; text-align: right; font-size: 16px; font-weight: 700; }
          </style>
        </head>
        <body>
          <h1>Fee Payment Receipt</h1>
          <div class="muted">Generated on ${this.escapeHtml(receipt.paidOn.toLocaleString())}</div>
          <div class="grid">
            <div class="item"><span>Student</span><strong>${this.escapeHtml(receipt.studentName)}</strong></div>
            <div class="item"><span>Class / Section</span><strong>${this.escapeHtml(`${receipt.className} / ${receipt.sectionName}`)}</strong></div>
            <div class="item"><span>Academic Year</span><strong>${this.escapeHtml(receipt.academicYearName)}</strong></div>
            <div class="item"><span>Payment Mode</span><strong>${this.escapeHtml(receipt.paymentMode)}</strong></div>
          </div>
          <table>
            <thead>
              <tr>
                <th>Fee Type</th>
                <th>Month</th>
                <th>Total Amount</th>
                <th>Edu. Tax</th>
                <th>Paid Now</th>
              </tr>
            </thead>
            <tbody>${rows}</tbody>
          </table>
          <div class="total">Total Paid: ${this.formatCurrency(total)}</div>
        </body>
      </html>
    `);
    printWindow.document.close();
    printWindow.focus();
    printWindow.print();
  }

  private formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'NPR' }).format(value || 0);
  }

  private escapeHtml(value: string): string {
    const entities: Record<string, string> = {
      '&': '&amp;',
      '<': '&lt;',
      '>': '&gt;',
      '"': '&quot;',
      "'": '&#039;'
    };

    return (value || '').replace(/[&<>"']/g, char => entities[char] ?? char);
  }

  private rebuildSelectedFees() {
    this.selectedFees = [...this.selectedCurrentYearFees, ...this.selectedPreviousYearFees];
    this.fees.clear();
    this.setFees();
  }
}
