import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { ApiService } from '../../../../shared/api.service';
import { AuthService } from '../../../../shared/auth.service';
import { DateConverterService } from '../../../../shared/calender/date-convertor.service';
import { LookupService } from '../../../../shared/common/lookup.service';
import { AcademicViewModel } from '../../master-entry/model/viewModels/academicYear.ViewModel';
import { TeacherViewModel } from '../../teacher/shared/models/viewModels/teacher.viewModel';
import {
  AttendanceSummaryViewModel,
  TeacherAttendanceReportRowViewModel,
  TeacherAttendanceStatus
} from '../shared/models/attendance.models';
import { AttendanceApiService } from '../shared/services/attendance-api.service';

type TeacherReportType = 'daily' | 'monthly' | 'late' | 'leave';

@Component({
  selector: 'app-teacher-attendance-report',
  standalone: false,
  templateUrl: './teacher-attendance-report.component.html',
  styleUrl: './teacher-attendance-report.component.scss'
})
export class TeacherAttendanceReportComponent implements OnInit {
  reportForm: FormGroup;
  academicYears: AcademicViewModel[] = [];
  teachers: TeacherViewModel[] = [];
  rows: TeacherAttendanceReportRowViewModel[] = [];
  summary: AttendanceSummaryViewModel = this.emptySummary();
  selectedDateNp = '';
  selectedMonthNp = '';
  isLoading = false;
  reportTitle = 'Teacher Attendance Report';

  readonly reportTypes = [
    { label: 'Daily Report', value: 'daily' },
    { label: 'Monthly Report', value: 'monthly' },
    { label: 'Late Arrival Report', value: 'late' },
    { label: 'Leave Report', value: 'leave' }
  ];

  constructor(
    private fb: FormBuilder,
    private attendanceApi: AttendanceApiService,
    private apiService: ApiService,
    private lookupService: LookupService,
    private authService: AuthService,
    private dateConverter: DateConverterService,
    private messageService: MessageService
  ) {
    const todayEn = this.dateConverter.formatAd(new Date());
    const todayNp = this.dateConverter.adToBs(todayEn);

    this.selectedDateNp = todayNp;
    this.selectedMonthNp = todayNp;
    this.reportForm = this.fb.group({
      academicYearId: [this.authService.getCurrentAcademicYearId(), Validators.required],
      teacherId: [null],
      reportType: ['daily' as TeacherReportType, Validators.required],
      attendanceDateNp: [todayNp, Validators.required],
      attendanceDateEn: [todayEn, Validators.required],
      monthReferenceNp: [todayNp, Validators.required],
      monthReferenceEn: [todayEn, Validators.required]
    });
  }

  ngOnInit(): void {
    forkJoin({
      academicYears: this.lookupService.getAcademicYears(),
      teachers: this.apiService.getTeachers(undefined, false)
    }).subscribe({
      next: ({ academicYears, teachers }) => {
        this.academicYears = academicYears;
        this.teachers = teachers;
      },
      error: err => this.showError(err?.error?.message ?? 'Failed to load teacher report filters.')
    });
  }

  onDateChange(date: { bs: string; ad: string }): void {
    this.selectedDateNp = date.bs;
    this.reportForm.patchValue({ attendanceDateNp: date.bs, attendanceDateEn: date.ad });
    this.resetReport();
  }

  onMonthChange(date: { bs: string; ad: string }): void {
    this.selectedMonthNp = date.bs;
    this.reportForm.patchValue({ monthReferenceNp: date.bs, monthReferenceEn: date.ad });
    this.resetReport();
  }

  runReport(): void {
    this.reportForm.markAllAsTouched();
    if (this.reportForm.invalid) {
      this.showWarn('Please complete the required report filters.');
      return;
    }

    const value = this.reportForm.getRawValue();
    const reportType = value.reportType as TeacherReportType;
    this.isLoading = true;

    if (reportType === 'daily') {
      this.reportTitle = 'Daily Teacher Attendance';
      this.attendanceApi.getTeacherAttendanceByDate(value.academicYearId, value.attendanceDateEn).subscribe({
        next: response => {
          this.rows = (response.teachers ?? []).filter(x => x.hasAttendance).map(teacher => ({
            attendanceId: teacher.attendanceId ?? '',
            academicYearId: value.academicYearId,
            teacherId: teacher.teacherId,
            teacherName: teacher.teacherName,
            employeeCode: teacher.employeeCode,
            designation: teacher.designation,
            attendanceDateEn: value.attendanceDateEn,
            attendanceDateNp: value.attendanceDateNp,
            status: teacher.status,
            statusName: teacher.statusName,
            checkInTime: this.trimTime(teacher.checkInTime),
            checkOutTime: this.trimTime(teacher.checkOutTime),
            remarks: teacher.remarks,
            isTeacherActive: teacher.isTeacherActive
          }));
          this.summary = response.summary ?? this.emptySummary();
          this.isLoading = false;
        },
        error: err => this.handleReportError(err)
      });
      return;
    }

    const monthDate = new Date(`${value.monthReferenceEn}T00:00:00`);
    const year = monthDate.getFullYear();
    const month = monthDate.getMonth() + 1;

    if (reportType === 'monthly') {
      this.reportTitle = 'Monthly Teacher Attendance';
      this.attendanceApi.getTeacherMonthlyAttendance(value.academicYearId, year, month, value.teacherId).subscribe({
        next: response => {
          this.rows = (response.rows ?? []).map(x => ({ ...x, checkInTime: this.trimTime(x.checkInTime), checkOutTime: this.trimTime(x.checkOutTime) }));
          this.summary = response.summary ?? this.emptySummary();
          this.isLoading = false;
        },
        error: err => this.handleReportError(err)
      });
      return;
    }

    if (reportType === 'late') {
      this.reportTitle = 'Late Arrival Report';
      this.attendanceApi.getTeacherLateArrivalReport(value.academicYearId, year, month).subscribe({
        next: rows => {
          this.rows = (rows ?? []).map(x => ({ ...x, checkInTime: this.trimTime(x.checkInTime), checkOutTime: this.trimTime(x.checkOutTime) }));
          this.summary = this.calculateSummary(this.rows);
          this.isLoading = false;
        },
        error: err => this.handleReportError(err)
      });
      return;
    }

    this.reportTitle = 'Teacher Leave Report';
    this.attendanceApi.getTeacherLeaveReport(value.academicYearId, year, month).subscribe({
      next: rows => {
        this.rows = (rows ?? []).map(x => ({ ...x, checkInTime: this.trimTime(x.checkInTime), checkOutTime: this.trimTime(x.checkOutTime) }));
        this.summary = this.calculateSummary(this.rows);
        this.isLoading = false;
      },
      error: err => this.handleReportError(err)
    });
  }

  getStatusSeverity(status: number): 'success' | 'info' | 'warning' | 'danger' | 'secondary' {
    switch (status) {
      case TeacherAttendanceStatus.Present:
      case TeacherAttendanceStatus.WorkFromHome:
        return 'success';
      case TeacherAttendanceStatus.Absent:
        return 'danger';
      case TeacherAttendanceStatus.Late:
        return 'warning';
      case TeacherAttendanceStatus.Leave:
        return 'info';
      default:
        return 'secondary';
    }
  }

  resetReport(): void {
    this.rows = [];
    this.summary = this.emptySummary();
  }

  private calculateSummary(rows: TeacherAttendanceReportRowViewModel[]): AttendanceSummaryViewModel {
    const total = rows.length;
    const present = rows.filter(x => x.status === TeacherAttendanceStatus.Present).length;
    const absent = rows.filter(x => x.status === TeacherAttendanceStatus.Absent).length;
    const late = rows.filter(x => x.status === TeacherAttendanceStatus.Late).length;
    const leave = rows.filter(x => x.status === TeacherAttendanceStatus.Leave).length;
    const halfDay = rows.filter(x => x.status === TeacherAttendanceStatus.HalfDay).length;
    const workFromHome = rows.filter(x => x.status === TeacherAttendanceStatus.WorkFromHome).length;
    const effectivePresent = present + late + workFromHome + halfDay * 0.5;

    return {
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

  private trimTime(value?: string | null): string | null {
    return value ? value.substring(0, 5) : null;
  }

  private handleReportError(err: any): void {
    this.isLoading = false;
    this.showError(err?.error?.message ?? 'Failed to load attendance report.');
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
