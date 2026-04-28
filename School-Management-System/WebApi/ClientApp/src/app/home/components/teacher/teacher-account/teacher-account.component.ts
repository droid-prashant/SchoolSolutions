import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { TeacherAccountCreateDto, TeacherPasswordResetDto } from '../shared/models/dtos/teacher.dto';
import { TeacherAccountViewModel, TeacherViewModel } from '../shared/models/viewModels/teacher.viewModel';

@Component({
  selector: 'app-teacher-account',
  standalone: false,
  templateUrl: './teacher-account.component.html',
  styleUrl: './teacher-account.component.scss'
})
export class TeacherAccountComponent implements OnInit {
  @Input() teacherId = '';
  @Input() embedded = false;
  teacher: TeacherViewModel | null = null;
  account: TeacherAccountViewModel | null = null;
  isLoading = false;
  isSaving = false;
  createAccountForm: FormGroup;
  resetPasswordForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService,
    private messageService: MessageService,
    private fb: FormBuilder
  ) {
    this.createAccountForm = this.fb.group({
      userName: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    this.resetPasswordForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    const routeTeacherId = this.route.snapshot.paramMap.get('teacherId') ?? '';
    this.teacherId = this.teacherId || routeTeacherId;
    if (!this.teacherId) {
      this.messageService.add({
        severity: 'error',
        summary: 'Teacher Not Found',
        detail: 'Teacher id is missing in the route.'
      });
      return;
    }

    this.loadPage();
  }

  loadPage(): void {
    this.isLoading = true;
    this.apiService.getTeacherById(this.teacherId).subscribe({
      next: teacher => {
        this.teacher = teacher;
        this.seedCreateUserName();
        this.loadAccount();
      },
      error: err => {
        this.isLoading = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Load Failed',
          detail: err?.error?.message ?? 'Failed to load teacher profile.'
        });
      }
    });
  }

  loadAccount(): void {
    this.apiService.getTeacherAccount(this.teacherId).subscribe({
      next: account => {
        this.account = account;
        this.isLoading = false;
      },
      error: err => {
        this.isLoading = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Load Failed',
          detail: err?.error?.message ?? 'Failed to load teacher account.'
        });
      }
    });
  }

  createAccount(): void {
    if (this.createAccountForm.invalid || this.isSaving) {
      this.createAccountForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const payload = this.createAccountForm.value as TeacherAccountCreateDto;
    this.apiService.createTeacherUser(this.teacherId, payload).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Account Created',
          detail: 'Teacher login has been created successfully.'
        });
        this.resetPasswordForm.reset();
        this.createAccountForm.patchValue({ password: '' });
        this.loadPage();
      },
      error: err => {
        this.messageService.add({
          severity: 'error',
          summary: 'Create Failed',
          detail: err?.error?.message ?? 'Failed to create teacher login.'
        });
      },
      complete: () => this.isSaving = false
    });
  }

  toggleAccountStatus(): void {
    if (!this.account?.isAccountCreated || this.isSaving) {
      return;
    }

    this.isSaving = true;
    this.apiService.updateTeacherUserStatus(this.teacherId, { isActive: !this.account.isActive }).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Account Updated',
          detail: `Teacher login ${this.account?.isActive ? 'deactivated' : 'activated'} successfully.`
        });
        this.loadAccount();
      },
      error: err => {
        this.messageService.add({
          severity: 'error',
          summary: 'Update Failed',
          detail: err?.error?.message ?? 'Failed to update teacher login status.'
        });
      },
      complete: () => this.isSaving = false
    });
  }

  resetPassword(): void {
    if (this.resetPasswordForm.invalid || !this.account?.isAccountCreated || this.isSaving) {
      this.resetPasswordForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const payload = this.resetPasswordForm.value as TeacherPasswordResetDto;
    this.apiService.resetTeacherUserPassword(this.teacherId, payload).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Password Reset',
          detail: 'Teacher login password has been reset successfully.'
        });
        this.resetPasswordForm.reset();
      },
      error: err => {
        this.messageService.add({
          severity: 'error',
          summary: 'Reset Failed',
          detail: err?.error?.message ?? 'Failed to reset teacher password.'
        });
      },
      complete: () => this.isSaving = false
    });
  }

  get teacherDisplayName(): string {
    if (!this.teacher) {
      return 'Teacher Account';
    }

    return `${this.teacher.fullName} - ${this.teacher.employeeCode || 'No code'}`;
  }

  private seedCreateUserName(): void {
    if (!this.teacher) {
      return;
    }

    const currentUserName = this.createAccountForm.get('userName')?.value;
    if (currentUserName) {
      return;
    }

    const first = (this.teacher.firstName || '').trim().toLowerCase();
    const last = (this.teacher.lastName || '').trim().toLowerCase();
    const code = (this.teacher.employeeCode || '').trim().toLowerCase();
    const suggested = [first, last].filter(Boolean).join('.') || code || 'teacher';
    this.createAccountForm.patchValue({ userName: suggested });
  }
}
