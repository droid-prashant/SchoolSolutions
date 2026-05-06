export enum StudentAttendanceStatus {
  Present = 1,
  Absent = 2,
  Late = 3,
  Leave = 4,
  HalfDay = 5
}

export enum TeacherAttendanceStatus {
  Present = 1,
  Absent = 2,
  Late = 3,
  Leave = 4,
  HalfDay = 5,
  WorkFromHome = 6
}

export interface AttendanceStatusCountViewModel {
  status: number;
  statusName: string;
  count: number;
}

export interface AttendanceSummaryViewModel {
  total: number;
  present: number;
  absent: number;
  late: number;
  leave: number;
  halfDay: number;
  workFromHome: number;
  attendancePercentage: number;
  statusCounts: AttendanceStatusCountViewModel[];
}

export interface StudentDailyAttendanceViewModel {
  academicYearId: string;
  classSectionId: string;
  attendanceDateEn: string;
  attendanceDateNp: string;
  summary: AttendanceSummaryViewModel;
  students: StudentAttendanceRowViewModel[];
}

export interface StudentAttendanceRowViewModel {
  attendanceId?: string | null;
  studentEnrollmentId: string;
  studentId: string;
  studentName: string;
  classRoomName: string;
  sectionName: string;
  rollNumber?: number | null;
  status: StudentAttendanceStatus;
  statusName: string;
  remarks: string;
  hasAttendance: boolean;
  isEnrollmentActive: boolean;
}

export interface StudentAttendanceReportRowViewModel {
  attendanceId: string;
  academicYearId: string;
  classSectionId: string;
  studentEnrollmentId: string;
  studentId: string;
  studentName: string;
  classRoomName: string;
  sectionName: string;
  rollNumber?: number | null;
  attendanceDateEn: string;
  attendanceDateNp: string;
  status: StudentAttendanceStatus;
  statusName: string;
  remarks: string;
  isEnrollmentActive: boolean;
}

export interface TeacherDailyAttendanceViewModel {
  academicYearId: string;
  attendanceDateEn: string;
  attendanceDateNp: string;
  summary: AttendanceSummaryViewModel;
  teachers: TeacherAttendanceRowViewModel[];
}

export interface TeacherAttendanceRowViewModel {
  attendanceId?: string | null;
  teacherId: string;
  teacherName: string;
  employeeCode: string;
  designation: string;
  status: TeacherAttendanceStatus;
  statusName: string;
  checkInTime?: string | null;
  checkOutTime?: string | null;
  remarks: string;
  hasAttendance: boolean;
  isTeacherActive: boolean;
}

export interface TeacherAttendanceReportRowViewModel {
  attendanceId: string;
  academicYearId: string;
  teacherId: string;
  teacherName: string;
  employeeCode: string;
  designation: string;
  attendanceDateEn: string;
  attendanceDateNp: string;
  status: TeacherAttendanceStatus;
  statusName: string;
  checkInTime?: string | null;
  checkOutTime?: string | null;
  remarks: string;
  isTeacherActive: boolean;
}

export interface MonthlyAttendanceReportViewModel<T> {
  academicYearId: string;
  year: number;
  month: number;
  summary: AttendanceSummaryViewModel;
  rows: T[];
}

export interface StudentAttendanceHistoryViewModel {
  studentId: string;
  studentName: string;
  academicYearId: string;
  summary: AttendanceSummaryViewModel;
  rows: StudentAttendanceReportRowViewModel[];
}

export interface StudentAttendanceBatchDto {
  academicYearId: string;
  classSectionId: string;
  attendanceDateEn: string;
  attendanceDateNp: string;
  entries: StudentAttendanceEntryDto[];
}

export interface StudentAttendanceEntryDto {
  studentEnrollmentId: string;
  status: StudentAttendanceStatus;
  remarks?: string | null;
}

export interface TeacherAttendanceBatchDto {
  academicYearId: string;
  attendanceDateEn: string;
  attendanceDateNp: string;
  entries: TeacherAttendanceEntryDto[];
}

export interface TeacherAttendanceEntryDto {
  teacherId: string;
  status: TeacherAttendanceStatus;
  checkInTime?: string | null;
  checkOutTime?: string | null;
  remarks?: string | null;
}

export interface TeacherCheckInOutDto {
  academicYearId: string;
  attendanceDateEn: string;
  attendanceDateNp: string;
  attendanceTime: string;
  remarks?: string | null;
}

export interface AttendanceStatusOption {
  label: string;
  value: number;
  shortLabel: string;
}
