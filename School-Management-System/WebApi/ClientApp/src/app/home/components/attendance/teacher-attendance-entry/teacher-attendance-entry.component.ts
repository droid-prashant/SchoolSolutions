import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { AuthService } from '../../../../shared/auth.service';
import { DateConverterService } from '../../../../shared/calender/date-convertor.service';
import { PermissionNames } from '../../../../shared/common/constants/permission-names';
import { LookupService } from '../../../../shared/common/lookup.service';
import { AcademicViewModel } from '../../master-entry/model/viewModels/academicYear.ViewModel';
import {
  AttendanceStatusOption,
  AttendanceSummaryViewModel,
  TeacherAttendanceRowViewModel,
  TeacherAttendanceStatus,
  TeacherDailyAttendanceViewModel
} from '../shared/models/attendance.models';
import { AttendanceApiService } from '../shared/services/attendance-api.service';

@Component({
  selector: 'app-teacher-attendance-entry',
  standalone: false,
  templateUrl: './teacher-attendance-entry.component.html',
  styleUrl: './teacher-attendance-entry.component.scss'
})
export class TeacherAttendanceEntryComponent implements OnInit {
  readonly permissions = PermissionNames;
  readonly statusOptions: AttendanceStatusOption[] = [
    { label: 'Present', value: TeacherAttendanceStatus.Present, shortLabel: 'P' },
    { label: 'Absent', value: TeacherAttendanceStatus.Absent, shortLabel: 'A' },
    { label: 'Late', value: TeacherAttendanceStatus.Late, shortLabel: 'L' },
    { label: 'Leave', value: TeacherAttendanceStatus.Leave, shortLabel: 'LV' },
    { label: 'Half Day', value: TeacherAttendanceStatus.HalfDay, shortLabel: 'HD' },
    { label: 'Work From Home', value: TeacherAttendanceStatus.WorkFromHome, shortLabel: 'WFH' }
  ];

  filterForm: FormGroup;
  teachers: TeacherAttendanceRowViewModel[] = [];
  summary: AttendanceSummaryViewModel = this.emptySummary();
  selectedNpDate = '';
  isLoading = false;
  isSaving = false;
  hasLoaded = false;

  constructor(
    private fb: FormBuilder,
    private attendanceApi: AttendanceApiService,
    private lookupService: LookupService,
    private authService: AuthService,
    private dateConverter: DateConverterService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {
    const today = new Date();
    const todayEn = this.dateConverter.formatAd(today);
    const todayNp = this.dateConverter.adToBs(todayEn);

    this.selectedNpDate = todayNp;
    this.filterForm = this.fb.group({
      academicYearId: [this.authService.getCurrentAcademicYearId(), Validators.required],
      attendanceDateNp: [todayNp, Validators.required],
      attendanceDateEn: [todayEn, Validators.required]
    });
  }

  ngOnInit(): void {
    this.lookupService.getAcademicYears().subscribe({
      next: years => this.applyAcademicYearContext(years),
      error: err => this.showError(err?.error?.message ?? 'Failed to load academic years.')
    });
  }

  onDateChange(date: { bs: string; ad: string }): void {
    this.selectedNpDate = date.bs;
    this.filterForm.patchValue({
      attendanceDateNp: date.bs,
      attendanceDateEn: date.ad
    });
    this.resetRows();
  }

  loadAttendance(): void {
    this.filterForm.markAllAsTouched();
    if (this.filterForm.invalid) {
      this.showWarn('Please select attendance date.');
      return;
    }

    const value = this.filterForm.getRawValue();
    this.isLoading = true;
    this.attendanceApi.getTeacherAttendanceByDate(value.academicYearId, value.attendanceDateEn).subscribe({
      next: response => {
        this.applyDailyResponse(response);
        this.isLoading = false;
        this.hasLoaded = true;
      },
      error: err => {
        this.isLoading = false;
        this.showError(err?.error?.message ?? 'Failed to load teacher attendance.');
      }
    });
  }

  markAll(status: TeacherAttendanceStatus): void {
    this.teachers = this.teachers.map(teacher => ({ ...teacher, status, statusName: this.getStatusLabel(status) }));
    this.recalculateSummary();
  }

  onStatusChange(row: TeacherAttendanceRowViewModel): void {
    row.statusName = this.getStatusLabel(row.status);
    this.recalculateSummary();
  }

  saveAttendance(): void {
    if (this.filterForm.invalid || this.teachers.length === 0) {
      this.showWarn('Load teachers before saving attendance.');
      return;
    }

    const invalidTime = this.teachers.some(x => x.checkInTime && x.checkOutTime && x.checkOutTime < x.checkInTime);
    if (invalidTime) {
      this.showWarn('Check-out time cannot be earlier than check-in time.');
      return;
    }

    const value = this.filterForm.getRawValue();
    this.isSaving = true;
    this.attendanceApi.upsertTeacherAttendance({
      academicYearId: value.academicYearId,
      attendanceDateEn: value.attendanceDateEn,
      attendanceDateNp: value.attendanceDateNp,
      entries: this.teachers.map(teacher => ({
        teacherId: teacher.teacherId,
        status: teacher.status,
        checkInTime: this.normalizeTime(teacher.checkInTime),
        checkOutTime: this.normalizeTime(teacher.checkOutTime),
        remarks: teacher.remarks?.trim() || null
      }))
    }).subscribe({
      next: response => {
        this.applyDailyResponse(response);
        this.isSaving = false;
        this.messageService.add({ severity: 'success', summary: 'Saved', detail: 'Teacher attendance saved successfully.' });
      },
      error: err => {
        this.isSaving = false;
        this.showError(err?.error?.message ?? 'Failed to save teacher attendance.');
      }
    });
  }

  checkIn(): void {
    this.submitCheckInOut('in');
  }

  checkOut(): void {
    this.submitCheckInOut('out');
  }

  deleteAttendance(row: TeacherAttendanceRowViewModel): void {
    if (!row.attendanceId) {
      return;
    }

    this.confirmationService.confirm({
      message: `Delete attendance entry for ${row.teacherName}?`,
      header: 'Delete Attendance',
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.attendanceApi.deleteTeacherAttendance(row.attendanceId as string).subscribe({
          next: () => {
            row.attendanceId = null;
            row.hasAttendance = false;
            row.status = TeacherAttendanceStatus.Present;
            row.statusName = this.getStatusLabel(TeacherAttendanceStatus.Present);
            row.checkInTime = null;
            row.checkOutTime = null;
            row.remarks = '';
            this.recalculateSummary();
            this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Attendance entry deleted.' });
          },
          error: err => this.showError(err?.error?.message ?? 'Failed to delete attendance.')
        });
      }
    });
  }

  resetRows(): void {
    this.teachers = [];
    this.summary = this.emptySummary();
    this.hasLoaded = false;
  }

  getStatusLabel(status: number): string {
    return this.statusOptions.find(x => x.value === status)?.label ?? 'Unknown';
  }

  private submitCheckInOut(direction: 'in' | 'out'): void {
    if (this.filterForm.invalid) {
      this.showWarn('Select date before check-in or check-out.');
      return;
    }

    const value = this.filterForm.getRawValue();
    const payload = {
      academicYearId: value.academicYearId,
      attendanceDateEn: value.attendanceDateEn,
      attendanceDateNp: value.attendanceDateNp,
      attendanceTime: `${this.currentTime()}:00`,
      remarks: null
    };

    const request = direction === 'in' ? this.attendanceApi.checkIn(payload) : this.attendanceApi.checkOut(payload);
    request.subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: direction === 'in' ? 'Checked In' : 'Checked Out',
          detail: direction === 'in' ? 'Your check-in time has been recorded.' : 'Your check-out time has been recorded.'
        });
        this.loadAttendance();
      },
      error: err => this.showError(err?.error?.message ?? `Failed to check ${direction}.`)
    });
  }

  private applyDailyResponse(response: TeacherDailyAttendanceViewModel): void {
    this.teachers = (response.teachers ?? []).map(teacher => ({
      ...teacher,
      checkInTime: this.trimTime(teacher.checkInTime),
      checkOutTime: this.trimTime(teacher.checkOutTime)
    }));
    this.summary = response.summary ?? this.emptySummary();
  }

  private applyAcademicYearContext(academicYears: AcademicViewModel[]): void {
    const selectedAcademicYearId = this.filterForm.get('academicYearId')?.value;
    const selectedYear = academicYears.find(x => x.id === selectedAcademicYearId) ?? academicYears.find(x => x.isActive);

    if (selectedYear) {
      this.filterForm.patchValue({ academicYearId: selectedYear.id }, { emitEvent: false });
    }
  }

  private recalculateSummary(): void {
    const total = this.teachers.length;
    const present = this.teachers.filter(x => x.status === TeacherAttendanceStatus.Present).length;
    const late = this.teachers.filter(x => x.status === TeacherAttendanceStatus.Late).length;
    const halfDay = this.teachers.filter(x => x.status === TeacherAttendanceStatus.HalfDay).length;
    const workFromHome = this.teachers.filter(x => x.status === TeacherAttendanceStatus.WorkFromHome).length;
    const absent = this.teachers.filter(x => x.status === TeacherAttendanceStatus.Absent).length;
    const leave = this.teachers.filter(x => x.status === TeacherAttendanceStatus.Leave).length;
    const effectivePresent = present + late + workFromHome + halfDay * 0.5;

    this.summary = {
      total,
      present,
      absent,
      late,
      leave,
      halfDay,
      workFromHome,
      attendancePercentage: total ? Math.round((effectivePresent / total) * 10000) / 100 : 0,
      statusCounts: []
    };
  }

  private currentTime(): string {
    const now = new Date();
    return `${String(now.getHours()).padStart(2, '0')}:${String(now.getMinutes()).padStart(2, '0')}`;
  }

  private normalizeTime(value?: string | null): string | null {
    if (!value) {
      return null;
    }

    return value.length === 5 ? `${value}:00` : value;
  }

  private trimTime(value?: string | null): string | null {
    return value ? value.substring(0, 5) : null;
  }

  private emptySummary(): AttendanceSummaryViewModel {
    return {
      total: 0,
      present: 0,
      absent: 0,
      late: 0,
      leave: 0,
      halfDay: 0,
      workFromHome: 0,
      attendancePercentage: 0,
      statusCounts: []
    };
  }

  private showWarn(detail: string): void {
    this.messageService.add({ severity: 'warn', summary: 'Attention', detail });
  }

  private showError(detail: string): void {
    this.messageService.add({ severity: 'error', summary: 'Error', detail });
  }
}
