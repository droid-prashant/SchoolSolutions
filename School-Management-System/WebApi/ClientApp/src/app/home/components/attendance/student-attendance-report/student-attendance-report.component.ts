import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { ApiService } from '../../../../shared/api.service';
import { AuthService } from '../../../../shared/auth.service';
import { DateConverterService } from '../../../../shared/calender/date-convertor.service';
import { LookupService } from '../../../../shared/common/lookup.service';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { AcademicViewModel } from '../../master-entry/model/viewModels/academicYear.ViewModel';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import {
  AttendanceSummaryViewModel,
  StudentAttendanceReportRowViewModel,
  StudentAttendanceStatus
} from '../shared/models/attendance.models';
import { AttendanceApiService } from '../shared/services/attendance-api.service';

type StudentReportType = 'daily' | 'monthly' | 'history' | 'absent';

@Component({
  selector: 'app-student-attendance-report',
  standalone: false,
  templateUrl: './student-attendance-report.component.html',
  styleUrl: './student-attendance-report.component.scss'
})
export class StudentAttendanceReportComponent implements OnInit {
  reportForm: FormGroup;
  classRooms: ClassRoomViewModel[] = [];
  sections: SectionViewModel[] = [];
  students: StudentViewModel[] = [];
  rows: StudentAttendanceReportRowViewModel[] = [];
  summary: AttendanceSummaryViewModel = this.emptySummary();
  selectedDateNp = '';
  selectedMonthNp = '';
  isLoading = false;
  reportTitle = 'Student Attendance Report';

  readonly reportTypes = [
    { label: 'Daily Report', value: 'daily' },
    { label: 'Monthly Report', value: 'monthly' },
    { label: 'Student History', value: 'history' },
    { label: 'Absent Students', value: 'absent' }
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
      classRoomId: [null, Validators.required],
      classSectionId: [null, Validators.required],
      studentId: [null],
      reportType: ['daily' as StudentReportType, Validators.required],
      attendanceDateNp: [todayNp, Validators.required],
      attendanceDateEn: [todayEn, Validators.required],
      monthReferenceNp: [todayNp, Validators.required],
      monthReferenceEn: [todayEn, Validators.required]
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
      error: err => this.showError(err?.error?.message ?? 'Failed to load report filters.')
    });
  }

  onClassRoomChange(): void {
    const classRoom = this.classRooms.find(x => x.id === this.reportForm.get('classRoomId')?.value);
    this.sections = classRoom?.sections ?? [];
    this.reportForm.patchValue({ classSectionId: null, studentId: null });
    this.students = [];
    this.resetReport();
  }

  onSectionChange(): void {
    this.reportForm.patchValue({ studentId: null });
    this.students = [];
    this.resetReport();

    const classSectionId = this.reportForm.get('classSectionId')?.value;
    if (!classSectionId) {
      return;
    }

    this.apiService.getStudentsByClassSectionId(classSectionId, null, true).subscribe({
      next: students => this.students = (students ?? []).map(student => ({
        ...student,
        displayName: student.displayName || `${student.rollNumber ? student.rollNumber + ' - ' : ''}${student.firstName} ${student.lastName}`.trim()
      })),
      error: () => this.students = []
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

  onReportTypeChange(): void {
    this.resetReport();
    const reportType = this.reportForm.get('reportType')?.value as StudentReportType;
    const studentControl = this.reportForm.get('studentId');

    if (reportType === 'history') {
      studentControl?.setValidators([Validators.required]);
    } else {
      studentControl?.clearValidators();
      studentControl?.setValue(null);
    }

    studentControl?.updateValueAndValidity();
  }

  runReport(): void {
    this.reportForm.markAllAsTouched();
    if (this.reportForm.invalid) {
      this.showWarn('Please complete the required report filters.');
      return;
    }

    const value = this.reportForm.getRawValue();
    const reportType = value.reportType as StudentReportType;
    this.isLoading = true;

    if (reportType === 'daily') {
      this.reportTitle = 'Daily Student Attendance';
      this.attendanceApi.getStudentAttendanceByDate(value.academicYearId, value.classSectionId, value.attendanceDateEn).subscribe({
        next: response => {
          this.rows = (response.students ?? []).filter(x => x.hasAttendance).map(student => ({
            attendanceId: student.attendanceId ?? '',
            academicYearId: value.academicYearId,
            classSectionId: value.classSectionId,
            studentEnrollmentId: student.studentEnrollmentId,
            studentId: student.studentId,
            studentName: student.studentName,
            classRoomName: student.classRoomName,
            sectionName: student.sectionName,
            rollNumber: student.rollNumber,
            attendanceDateEn: value.attendanceDateEn,
            attendanceDateNp: value.attendanceDateNp,
            status: student.status,
            statusName: student.statusName,
            remarks: student.remarks,
            isEnrollmentActive: student.isEnrollmentActive
          }));
          this.summary = response.summary ?? this.emptySummary();
          this.isLoading = false;
        },
        error: err => this.handleReportError(err)
      });
      return;
    }

    if (reportType === 'monthly') {
      const monthDate = new Date(`${value.monthReferenceEn}T00:00:00`);
      this.reportTitle = 'Monthly Student Attendance';
      this.attendanceApi.getStudentMonthlyAttendance(value.academicYearId, value.classSectionId, monthDate.getFullYear(), monthDate.getMonth() + 1).subscribe({
        next: response => {
          this.rows = response.rows ?? [];
          this.summary = response.summary ?? this.emptySummary();
          this.isLoading = false;
        },
        error: err => this.handleReportError(err)
      });
      return;
    }

    if (reportType === 'absent') {
      this.reportTitle = 'Absent Student Report';
      this.attendanceApi.getAbsentStudents(value.academicYearId, value.classSectionId, value.attendanceDateEn).subscribe({
        next: rows => {
          this.rows = rows ?? [];
          this.summary = this.calculateSummary(this.rows);
          this.isLoading = false;
        },
        error: err => this.handleReportError(err)
      });
      return;
    }

    this.reportTitle = 'Student Attendance History';
    this.attendanceApi.getStudentAttendanceHistory(value.studentId, value.academicYearId).subscribe({
      next: response => {
        this.rows = response.rows ?? [];
        this.summary = response.summary ?? this.emptySummary();
        this.isLoading = false;
      },
      error: err => this.handleReportError(err)
    });
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

  resetReport(): void {
    this.rows = [];
    this.summary = this.emptySummary();
  }

  private calculateSummary(rows: StudentAttendanceReportRowViewModel[]): AttendanceSummaryViewModel {
    const total = rows.length;
    const present = rows.filter(x => x.status === StudentAttendanceStatus.Present).length;
    const absent = rows.filter(x => x.status === StudentAttendanceStatus.Absent).length;
    const late = rows.filter(x => x.status === StudentAttendanceStatus.Late).length;
    const leave = rows.filter(x => x.status === StudentAttendanceStatus.Leave).length;
    const halfDay = rows.filter(x => x.status === StudentAttendanceStatus.HalfDay).length;
    const effectivePresent = present + late + halfDay * 0.5;

    return {
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

  private applyAcademicYearContext(academicYears: AcademicViewModel[]): void {
    const selectedAcademicYearId = this.reportForm.get('academicYearId')?.value;
    const selectedYear = academicYears.find(x => x.id === selectedAcademicYearId) ?? academicYears.find(x => x.isActive);

    if (selectedYear) {
      this.reportForm.patchValue({ academicYearId: selectedYear.id }, { emitEvent: false });
    }
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
