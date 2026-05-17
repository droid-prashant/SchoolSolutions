import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { StudentViewModel } from '../shared/models/viewModels/student.viewModel';
import { GuardianViewModel, StudentGuardianViewModel } from '../shared/models/guardian.models';

@Component({
  selector: 'app-student-guardians',
  standalone: false,
  templateUrl: './student-guardians.component.html',
  styleUrl: './student-guardians.component.scss'
})
export class StudentGuardiansComponent implements OnChanges {
  @Input() student: StudentViewModel | null = null;

  guardians: StudentGuardianViewModel[] = [];
  guardianSearchResults: GuardianViewModel[] = [];
  createGuardianForm: FormGroup;
  linkGuardianForm: FormGroup;
  isLoading = false;
  isSaving = false;
  isSearching = false;
  isCreateSubmitted = false;
  isLinkSubmitted = false;
  private shouldHydrateCreateFormFromLinkedGuardian = false;

  constructor(
    private _fb: FormBuilder,
    private _apiService: ApiService,
    private _messageService: MessageService,
    private _confirmationService: ConfirmationService
  ) {
    this.createGuardianForm = this._fb.group({
      fullName: ['', Validators.required],
      contactNumber: ['', Validators.required],
      email: [''],
      relationType: ['Father', Validators.required],
      userName: ['', Validators.required],
      password: ['', Validators.required],
      isPrimaryGuardian: [false],
      canViewFees: [true],
      canViewResults: [true],
      canViewAttendance: [true],
      canPayFees: [false]
    });

    this.linkGuardianForm = this._fb.group({
      keyword: [''],
      guardianId: ['', Validators.required],
      isPrimaryGuardian: [false],
      canViewFees: [true],
      canViewResults: [true],
      canViewAttendance: [true],
      canPayFees: [false]
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['student'] && this.student?.id) {
      this.guardians = [];
      this.shouldHydrateCreateFormFromLinkedGuardian = true;
      this.resetCreateGuardianForm();
      this.loadGuardians();
      this.searchGuardians();
    }
  }

  loadGuardians(): void {
    if (!this.student?.id) {
      return;
    }

    this.isLoading = true;
    this._apiService.getStudentGuardians(this.student.id).subscribe({
      next: guardians => {
        this.guardians = guardians;
        if (this.shouldHydrateCreateFormFromLinkedGuardian) {
          const linkedGuardian = guardians[0];
          if (linkedGuardian) {
            this.patchCreateGuardianFormFromLinkedGuardian(linkedGuardian);
          } else if (this.createGuardianForm.pristine) {
            this.createGuardianForm.patchValue({ isPrimaryGuardian: true });
          }
          this.shouldHydrateCreateFormFromLinkedGuardian = false;
        }
      },
      error: err => this.showError(err?.error?.message ?? 'Failed to load guardians.'),
      complete: () => this.isLoading = false
    });
  }

  searchGuardians(): void {
    const keyword = this.linkGuardianForm.get('keyword')?.value ?? '';
    this.isSearching = true;
    this._apiService.searchGuardians(keyword).subscribe({
      next: guardians => this.guardianSearchResults = guardians,
      error: err => this.showError(err?.error?.message ?? 'Failed to search guardians.'),
      complete: () => this.isSearching = false
    });
  }

  createGuardian(): void {
    this.isCreateSubmitted = true;

    if (!this.student?.id || this.createGuardianForm.invalid) {
      return;
    }

    this.isSaving = true;
    this._apiService.createGuardianForStudent(this.student.id, this.createGuardianForm.value).subscribe({
      next: guardian => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Guardian created and linked.' });
        this.shouldHydrateCreateFormFromLinkedGuardian = false;
        this.patchCreateGuardianFormFromLinkedGuardian(guardian);
        this.loadGuardians();
        this.searchGuardians();
      },
      error: err => this.showError(err?.error?.message ?? 'Failed to create guardian.'),
      complete: () => this.isSaving = false
    });
  }

  linkGuardian(): void {
    this.isLinkSubmitted = true;

    if (!this.student?.id || this.linkGuardianForm.invalid) {
      return;
    }

    const value = this.linkGuardianForm.value;
    this.isSaving = true;
    this._apiService.linkGuardianToStudent({
      guardianId: value.guardianId,
      studentId: this.student.id,
      isPrimaryGuardian: value.isPrimaryGuardian,
      canViewFees: value.canViewFees,
      canViewResults: value.canViewResults,
      canViewAttendance: value.canViewAttendance,
      canPayFees: value.canPayFees
    }).subscribe({
      next: () => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Guardian linked to student.' });
        this.linkGuardianForm.patchValue({ guardianId: '' });
        this.isLinkSubmitted = false;
        this.loadGuardians();
      },
      error: err => this.showError(err?.error?.message ?? 'Failed to link guardian.'),
      complete: () => this.isSaving = false
    });
  }

  updateAccess(guardian: StudentGuardianViewModel): void {
    this._apiService.updateStudentGuardianAccess(guardian.guardianStudentId, {
      isPrimaryGuardian: guardian.isPrimaryGuardian,
      canViewFees: guardian.canViewFees,
      canViewResults: guardian.canViewResults,
      canViewAttendance: guardian.canViewAttendance,
      canPayFees: guardian.canPayFees
    }).subscribe({
      next: () => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Guardian access updated.' });
        this.loadGuardians();
      },
      error: err => this.showError(err?.error?.message ?? 'Failed to update guardian access.')
    });
  }

  unlinkGuardian(guardian: StudentGuardianViewModel): void {
    this._confirmationService.confirm({
      message: `Remove ${guardian.fullName}'s access to this student?`,
      header: 'Unlink Guardian',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Unlink',
      rejectLabel: 'Cancel',
      accept: () => {
        this._apiService.unlinkGuardianFromStudent(guardian.guardianStudentId).subscribe({
          next: () => {
            this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Guardian unlinked.' });
            this.loadGuardians();
          },
          error: err => this.showError(err?.error?.message ?? 'Failed to unlink guardian.')
        });
      }
    });
  }

  get studentName(): string {
    return this.student ? `${this.student.firstName} ${this.student.lastName}`.trim() : '';
  }

  isCreateControlInvalid(controlName: string): boolean {
    const control = this.createGuardianForm.get(controlName);
    return !!(control && control.invalid && this.isCreateSubmitted);
  }

  isLinkControlInvalid(controlName: string): boolean {
    const control = this.linkGuardianForm.get(controlName);
    return !!(control && control.invalid && this.isLinkSubmitted);
  }

  private resetCreateGuardianForm(): void {
    const parentName = this.getPreferredParentName();
    this.createGuardianForm.reset({
      fullName: parentName,
      contactNumber: this.student?.parentContactNumber ?? '',
      email: this.student?.parentEmail ?? '',
      relationType: this.getPreferredRelationType(),
      userName: this.buildSuggestedUserName(parentName),
      password: '',
      isPrimaryGuardian: this.guardians.length === 0,
      canViewFees: true,
      canViewResults: true,
      canViewAttendance: true,
      canPayFees: false
    });
    this.createGuardianForm.markAsPristine();
    this.createGuardianForm.markAsUntouched();
    this.createGuardianForm.updateValueAndValidity();
    this.isCreateSubmitted = false;
  }

  private patchCreateGuardianFormFromLinkedGuardian(guardian: StudentGuardianViewModel): void {
    this.createGuardianForm.reset({
      fullName: guardian.fullName,
      contactNumber: guardian.contactNumber,
      email: guardian.email ?? '',
      relationType: guardian.relationType,
      userName: guardian.userName,
      password: '',
      isPrimaryGuardian: guardian.isPrimaryGuardian,
      canViewFees: guardian.canViewFees,
      canViewResults: guardian.canViewResults,
      canViewAttendance: guardian.canViewAttendance,
      canPayFees: guardian.canPayFees
    });
    this.createGuardianForm.markAsPristine();
    this.createGuardianForm.markAsUntouched();
    this.createGuardianForm.updateValueAndValidity();
    this.isCreateSubmitted = false;
  }

  private getPreferredParentName(): string {
    return this.student?.fatherName || this.student?.motherName || '';
  }

  private getPreferredRelationType(): string {
    if (this.student?.fatherName) {
      return 'Father';
    }
    if (this.student?.motherName) {
      return 'Mother';
    }
    return 'Guardian';
  }

  private buildSuggestedUserName(name: string): string {
    const cleanName = name.toLowerCase().replace(/[^a-z0-9]/g, '');
    const suffix = this.student?.id?.substring(0, 6) ?? '';
    return cleanName ? `${cleanName}${suffix}` : '';
  }

  private showError(message: string): void {
    this._messageService.add({ severity: 'error', summary: 'Error', detail: message });
  }
}
