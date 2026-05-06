import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../../../shared/auth.service';
import { DateConverterService } from '../../../../shared/calender/date-convertor.service';
import { PermissionNames } from '../../../../shared/common/constants/permission-names';
import { LookupService } from '../../../../shared/common/lookup.service';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { AcademicViewModel } from '../../master-entry/model/viewModels/academicYear.ViewModel';
import {
  AttendanceStatusOption,
  AttendanceSummaryViewModel,
  StudentAttendanceRowViewModel,
  StudentAttendanceStatus,
  StudentDailyAttendanceViewModel
} from '../shared/models/attendance.models';
import { AttendanceApiService } from '../shared/services/attendance-api.service';

@Component({
  selector: 'app-student-attendance-entry',
  standalone: false,
  templateUrl: './student-attendance-entry.component.html',
  styleUrl: './student-attendance-entry.component.scss'
})
export class StudentAttendanceEntryComponent implements OnInit {
  readonly permissions = PermissionNames;
  readonly statusOptions: AttendanceStatusOption[] = [
    { label: 'Present', value: StudentAttendanceStatus.Present, shortLabel: 'P' },
    { label: 'Absent', value: StudentAttendanceStatus.Absent, shortLabel: 'A' },
    { label: 'Late', value: StudentAttendanceStatus.Late, shortLabel: 'L' },
    { label: 'Leave', value: StudentAttendanceStatus.Leave, shortLabel: 'LV' },
    { label: 'Half Day', value: StudentAttendanceStatus.HalfDay, shortLabel: 'HD' }
  ];

  filterForm: FormGroup;
  classRooms: ClassRoomViewModel[] = [];
  sections: SectionViewModel[] = [];
  students: StudentAttendanceRowViewModel[] = [];
  summary: AttendanceSummaryViewModel = this.emptySummary();
  isLoading = false;
  isSaving = false;
  hasLoaded = false;
  selectedNpDate = '';

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
      classRoomId: [null, Validators.required],
      classSectionId: [null, Validators.required],
      attendanceDateNp: [todayNp, Validators.required],
      attendanceDateEn: [todayEn, Validators.required]
    });
  }

  ngOnInit(): void {
    forkJoin({
      academicYears: this.lookupService.getAcademicYears(),
      classRooms: this.lookupService.getClassRooms()
    }).subscribe({
      next: ({ academicYears, classRooms }) => {
        this.classRooms = classRooms;
        this.applyAcademicYearContext(academicYears);
      },
      error: err => this.showError(err?.error?.message ?? 'Failed to load attendance filters.')
    });
  }

  onClassRoomChange(): void {
    const classRoomId = this.filterForm.get('classRoomId')?.value;
    const classRoom = this.classRooms.find(x => x.id === classRoomId);
    this.sections = classRoom?.sections ?? [];
    this.filterForm.patchValue({ classSectionId: null });
    this.resetRows();
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
      this.showWarn('Please select class, section, and date.');
      return;
    }

    const value = this.filterForm.getRawValue();
    this.isLoading = true;
    this.attendanceApi.getStudentAttendanceByDate(value.academicYearId, value.classSectionId, value.attendanceDateEn).subscribe({
      next: response => {
        this.applyDailyResponse(response);
        this.isLoading = false;
        this.hasLoaded = true;
      },
      error: err => {
        this.isLoading = false;
        this.showError(err?.error?.message ?? 'Failed to load student attendance.');
      }
    });
  }

  markAll(status: StudentAttendanceStatus): void {
    this.students = this.students.map(student => ({ ...student, status, statusName: this.getStatusLabel(status) }));
    this.recalculateSummary();
  }

  onStatusChange(row: StudentAttendanceRowViewModel): void {
    row.statusName = this.getStatusLabel(row.status);
    this.recalculateSummary();
  }

  saveAttendance(): void {
    if (this.filterForm.invalid || this.students.length === 0) {
      this.showWarn('Load students before saving attendance.');
      return;
    }

    const value = this.filterForm.getRawValue();
    this.isSaving = true;
    this.attendanceApi.upsertStudentAttendance({
      academicYearId: value.academicYearId,
      classSectionId: value.classSectionId,
      attendanceDateEn: value.attendanceDateEn,
      attendanceDateNp: value.attendanceDateNp,
      entries: this.students.map(student => ({
        studentEnrollmentId: student.studentEnrollmentId,
        status: student.status,
        remarks: student.remarks?.trim() || null
      }))
    }).subscribe({
      next: response => {
        this.applyDailyResponse(response);
        this.isSaving = false;
        this.messageService.add({ severity: 'success', summary: 'Saved', detail: 'Student attendance saved successfully.' });
      },
      error: err => {
        this.isSaving = false;
        this.showError(err?.error?.message ?? 'Failed to save student attendance.');
      }
    });
  }

  deleteAttendance(row: StudentAttendanceRowViewModel): void {
    if (!row.attendanceId) {
      return;
    }

    this.confirmationService.confirm({
      message: `Delete attendance entry for ${row.studentName}?`,
      header: 'Delete Attendance',
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.attendanceApi.deleteStudentAttendance(row.attendanceId as string).subscribe({
          next: () => {
            row.attendanceId = null;
            row.hasAttendance = false;
            row.status = StudentAttendanceStatus.Present;
            row.statusName = this.getStatusLabel(StudentAttendanceStatus.Present);
            row.remarks = '';
            this.recalculateSummary();
            this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Attendance entry deleted.' });
          },
          error: err => this.showError(err?.error?.message ?? 'Failed to delete attendance.')
        });
      }
    });
  }

  getStatusLabel(status: number): string {
    return this.statusOptions.find(x => x.value === status)?.label ?? 'Unknown';
  }

  getStatusSeverity(status: number): 'success' | 'info' | 'warning' | 'danger' | 'secondary' {
    switch (status) {
      case StudentAttendanceStatus.Present:
        return 'success';
      case StudentAttendanceStatus.Absent:
        return 'danger';
      case StudentAttendanceStatus.Late:
        return 'warning';
      case StudentAttendanceStatus.Leave:
        return 'info';
      default:
        return 'secondary';
    }
  }

  private applyDailyResponse(response: StudentDailyAttendanceViewModel): void {
    this.students = response.students ?? [];
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
    const total = this.students.length;
    const present = this.students.filter(x => x.status === StudentAttendanceStatus.Present).length;
    const late = this.students.filter(x => x.status === StudentAttendanceStatus.Late).length;
    const halfDay = this.students.filter(x => x.status === StudentAttendanceStatus.HalfDay).length;
    const absent = this.students.filter(x => x.status === StudentAttendanceStatus.Absent).length;
    const leave = this.students.filter(x => x.status === StudentAttendanceStatus.Leave).length;
    const effectivePresent = present + late + halfDay * 0.5;

    this.summary = {
      total,
      present,
      absent,
      late,
      leave,
      halfDay,
      workFromHome: 0,
      attendancePercentage: total ? Math.round((effectivePresent / total) * 10000) / 100 : 0,
      statusCounts: []
    };
  }

  resetRows(): void {
    this.students = [];
    this.summary = this.emptySummary();
    this.hasLoaded = false;
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
