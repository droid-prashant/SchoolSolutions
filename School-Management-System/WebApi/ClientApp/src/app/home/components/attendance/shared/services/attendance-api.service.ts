import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import {
  AttendanceSummaryViewModel,
  MonthlyAttendanceReportViewModel,
  StudentAttendanceBatchDto,
  StudentAttendanceHistoryViewModel,
  StudentAttendanceReportRowViewModel,
  StudentDailyAttendanceViewModel,
  TeacherAttendanceBatchDto,
  TeacherAttendanceReportRowViewModel,
  TeacherAttendanceRowViewModel,
  TeacherCheckInOutDto,
  TeacherDailyAttendanceViewModel
} from '../models/attendance.models';

@Injectable({
  providedIn: 'root'
})
export class AttendanceApiService {
  private readonly baseUrl = environment.API_BASE_URL;

  constructor(private httpClient: HttpClient) {}

  getStudentAttendanceByDate(academicYearId: string, classSectionId: string, attendanceDateEn: string): Observable<StudentDailyAttendanceViewModel> {
    const params = new HttpParams()
      .set('academicYearId', academicYearId)
      .set('classSectionId', classSectionId)
      .set('attendanceDateEn', attendanceDateEn);

    return this.httpClient.get<StudentDailyAttendanceViewModel>(this.baseUrl + 'api/StudentAttendance/GetAttendanceByDate', { params });
  }

  upsertStudentAttendance(payload: StudentAttendanceBatchDto): Observable<StudentDailyAttendanceViewModel> {
    return this.httpClient.post<StudentDailyAttendanceViewModel>(this.baseUrl + 'api/StudentAttendance/UpsertDailyAttendance', payload);
  }

  getStudentMonthlyAttendance(
    academicYearId: string,
    classSectionId: string,
    year: number,
    month: number
  ): Observable<MonthlyAttendanceReportViewModel<StudentAttendanceReportRowViewModel>> {
    const params = new HttpParams()
      .set('academicYearId', academicYearId)
      .set('classSectionId', classSectionId)
      .set('year', year)
      .set('month', month);

    return this.httpClient.get<MonthlyAttendanceReportViewModel<StudentAttendanceReportRowViewModel>>(
      this.baseUrl + 'api/StudentAttendance/GetMonthlyAttendance',
      { params }
    );
  }

  getStudentAttendanceHistory(studentId: string, academicYearId: string): Observable<StudentAttendanceHistoryViewModel> {
    const params = new HttpParams()
      .set('studentId', studentId)
      .set('academicYearId', academicYearId);

    return this.httpClient.get<StudentAttendanceHistoryViewModel>(this.baseUrl + 'api/StudentAttendance/GetAttendanceByStudent', { params });
  }

  getAbsentStudents(academicYearId: string, classSectionId: string, attendanceDateEn: string): Observable<StudentAttendanceReportRowViewModel[]> {
    const params = new HttpParams()
      .set('academicYearId', academicYearId)
      .set('classSectionId', classSectionId)
      .set('attendanceDateEn', attendanceDateEn);

    return this.httpClient.get<StudentAttendanceReportRowViewModel[]>(this.baseUrl + 'api/StudentAttendance/GetAbsentStudents', { params });
  }

  getStudentAttendanceSummary(academicYearId: string, classSectionId: string, attendanceDateEn: string): Observable<AttendanceSummaryViewModel> {
    const params = new HttpParams()
      .set('academicYearId', academicYearId)
      .set('classSectionId', classSectionId)
      .set('attendanceDateEn', attendanceDateEn);

    return this.httpClient.get<AttendanceSummaryViewModel>(this.baseUrl + 'api/StudentAttendance/GetAttendanceSummary', { params });
  }

  deleteStudentAttendance(attendanceId: string): Observable<void> {
    const params = new HttpParams().set('attendanceId', attendanceId);
    return this.httpClient.delete<void>(this.baseUrl + 'api/StudentAttendance/DeleteAttendance', { params });
  }

  getTeacherAttendanceByDate(academicYearId: string, attendanceDateEn: string): Observable<TeacherDailyAttendanceViewModel> {
    const params = new HttpParams()
      .set('academicYearId', academicYearId)
      .set('attendanceDateEn', attendanceDateEn);

    return this.httpClient.get<TeacherDailyAttendanceViewModel>(this.baseUrl + 'api/TeacherAttendance/GetAttendanceByDate', { params });
  }

  upsertTeacherAttendance(payload: TeacherAttendanceBatchDto): Observable<TeacherDailyAttendanceViewModel> {
    return this.httpClient.post<TeacherDailyAttendanceViewModel>(this.baseUrl + 'api/TeacherAttendance/UpsertDailyAttendance', payload);
  }

  getTeacherMonthlyAttendance(
    academicYearId: string,
    year: number,
    month: number,
    teacherId?: string | null
  ): Observable<MonthlyAttendanceReportViewModel<TeacherAttendanceReportRowViewModel>> {
    let params = new HttpParams()
      .set('academicYearId', academicYearId)
      .set('year', year)
      .set('month', month);

    if (teacherId) {
      params = params.set('teacherId', teacherId);
    }

    return this.httpClient.get<MonthlyAttendanceReportViewModel<TeacherAttendanceReportRowViewModel>>(
      this.baseUrl + 'api/TeacherAttendance/GetMonthlyAttendance',
      { params }
    );
  }

  getTeacherLateArrivalReport(academicYearId: string, year: number, month: number): Observable<TeacherAttendanceReportRowViewModel[]> {
    const params = new HttpParams()
      .set('academicYearId', academicYearId)
      .set('year', year)
      .set('month', month);

    return this.httpClient.get<TeacherAttendanceReportRowViewModel[]>(this.baseUrl + 'api/TeacherAttendance/GetLateArrivalReport', { params });
  }

  getTeacherLeaveReport(academicYearId: string, year: number, month: number): Observable<TeacherAttendanceReportRowViewModel[]> {
    const params = new HttpParams()
      .set('academicYearId', academicYearId)
      .set('year', year)
      .set('month', month);

    return this.httpClient.get<TeacherAttendanceReportRowViewModel[]>(this.baseUrl + 'api/TeacherAttendance/GetLeaveReport', { params });
  }

  getTeacherAttendanceSummary(academicYearId: string, attendanceDateEn: string): Observable<AttendanceSummaryViewModel> {
    const params = new HttpParams()
      .set('academicYearId', academicYearId)
      .set('attendanceDateEn', attendanceDateEn);

    return this.httpClient.get<AttendanceSummaryViewModel>(this.baseUrl + 'api/TeacherAttendance/GetAttendanceSummary', { params });
  }

  checkIn(payload: TeacherCheckInOutDto): Observable<TeacherAttendanceRowViewModel> {
    return this.httpClient.post<TeacherAttendanceRowViewModel>(this.baseUrl + 'api/TeacherAttendance/CheckIn', payload);
  }

  checkOut(payload: TeacherCheckInOutDto): Observable<TeacherAttendanceRowViewModel> {
    return this.httpClient.post<TeacherAttendanceRowViewModel>(this.baseUrl + 'api/TeacherAttendance/CheckOut', payload);
  }

  deleteTeacherAttendance(attendanceId: string): Observable<void> {
    const params = new HttpParams().set('attendanceId', attendanceId);
    return this.httpClient.delete<void>(this.baseUrl + 'api/TeacherAttendance/DeleteAttendance', { params });
  }
}
