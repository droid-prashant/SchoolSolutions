import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StudentDto } from '../home/components/student/shared/models/dtos/student.dto';
import { StudentViewModel } from '../home/components/student/shared/models/viewModels/student.viewModel';
import { CourseDto } from '../home/components/course/shared/models/course.dto';
import { CourseViewModel } from '../home/components/course/shared/models/course.viewModel';
import { SubjectMarkDto } from '../home/components/exam/shared/models/examMarksEntry.dto';
import { ClassRoomViewModel } from '../home/components/class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../home/components/class-room/shared/models/viewModels/section.viewModel';
import { ClassSectionDto } from '../home/components/class-room/shared/models/dtos/classSection.dto';
import { SectionDto } from '../home/components/master-entry/model/dtos/section.dto';
import { ClassCreditCourseViewModel } from '../home/components/course/shared/models/classCourse.viewModel';
import { ClassCourseDto } from '../home/components/course/shared/models/classCourse.dto';
import { ResultViewModel } from '../home/components/exam/shared/viewModels/result.viewModel';
import { FeeTypeDto } from '../home/components/master-entry/model/dtos/feeType.dto';
import { FeeTypeViewModel } from '../home/components/master-entry/model/viewModels/feeType.viewModel';
import { FeeStructureDto } from '../home/components/fees/model/feeStructure.dto';
import { FeeAdjustmentDto } from '../home/components/fees/model/feeAdjustment.dto';
import { FeeStructureViewModel } from '../home/components/fees/model/feeStructure.viewModel';
import { FeeReportViewModel } from '../home/components/fees/model/feeReport.viewModel';
import { StudentFeeSummaryViewModel } from '../home/components/fees/model/studentFeeSummary.viewModel';
import { ClassRoomDto } from '../home/components/master-entry/model/dtos/classRoom.dto';
import { AcademicYearDto } from '../home/components/master-entry/model/dtos/academicYear.dto';
import { AcademicViewModel } from '../home/components/master-entry/model/viewModels/academicYear.ViewModel';
import { StudentEnrollmentViewModel } from '../home/components/exam/shared/viewModels/studentEnrollment.viewModel';
import { StudentStudentEnrollmentDto } from '../home/components/exam/shared/models/examEnrollment.dto';
import { ProvinceViewModel } from './common/models/master/master.ViewModel';
import { StudentCertificateViewModel } from '../home/components/certificate/model/studentCertificate.ViewModel';
import { StudentCertificateDto } from '../home/components/certificate/model/studentCertificate.dto';
import { StudentCertificateLogViewModel } from '../home/components/certificate/model/certificateLog.ViewModel';
import { CertificateType } from '../home/components/certificate/model/certificateType.enum';
import { StudentsByClassViewModel } from '../home/components/dashboard/model/studentsByClass.viewModel';
import { SubjectMarksViewModel } from '../home/components/exam/shared/viewModels/student-marks.viewModel';
import { TeacherAccountCreateDto, TeacherAccountStatusDto, TeacherAssignmentCopyDto, TeacherClassSectionDto, TeacherDto, TeacherPasswordResetDto, TeacherStatusDto } from '../home/components/teacher/shared/models/dtos/teacher.dto';
import { TeacherAccountViewModel, TeacherClassSectionViewModel, TeacherDashboardViewModel, TeacherDocumentViewModel, TeacherViewModel } from '../home/components/teacher/shared/models/viewModels/teacher.viewModel';
import { PromotionCandidateViewModel } from '../home/components/student/shared/models/viewModels/promotionCandidate.viewModel';
import { PromoteStudentsDto } from '../home/components/student/shared/models/dtos/promoteStudents.dto';
import { PromotionExecutionResultViewModel } from '../home/components/student/shared/models/viewModels/promotionExecutionResult.viewModel';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  baseUrl: string = environment.API_BASE_URL;
  constructor(private _httpClient: HttpClient) { }

  getStudentsCount(): Observable<number> {
    return this._httpClient.get<number>(this.baseUrl + "api/Dashboard/GetStudentsCount");
  }

  getCoursesCount(): Observable<number> {
    return this._httpClient.get<number>(this.baseUrl + "api/Dashboard/GetCoursesCount");
  }

  getStudentsByClassCount(): Observable<StudentsByClassViewModel[]> {
    return this._httpClient.get<StudentsByClassViewModel[]>(this.baseUrl + "api/Dashboard/GetStudentsByClass");
  }

  getTeacherDashboard(academicYearId?: string): Observable<TeacherDashboardViewModel> {
    const academicYearQuery = academicYearId ? `?academicYearId=${academicYearId}` : '';
    return this._httpClient.get<TeacherDashboardViewModel>(this.baseUrl + `api/Teacher/GetTeacherDashboard${academicYearQuery}`);
  }

  postAcademicYear(academicYearDetail: AcademicYearDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Academic/AddAcademicYear", academicYearDetail);
  }

  getAcademicYear(): Observable<AcademicViewModel[]> {
    return this._httpClient.get<AcademicViewModel[]>(this.baseUrl + "api/Academic/GetAllAcademicYear");
  }

  putAcademicYear(academicYearDetail: AcademicYearDto, academicYearId: string): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + `api/Academic/UpdateAcademicYear?academicYearId=${academicYearId}`, academicYearDetail);
  }

  getProvinceDetails(): Observable<ProvinceViewModel[]> {
    return this._httpClient.get<ProvinceViewModel[]>(this.baseUrl + "api/MasterData/GetAllProvince");
  }

  postStudent(student: StudentDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Student/AddStudent", student);
  }

  updateStudent(student: StudentDto): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + "api/Student/UpdateStudent", student);
  }

  postTeacher(teacher: TeacherDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Teacher/AddTeacher", teacher);
  }

  updateTeacher(teacher: TeacherDto): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + "api/Teacher/UpdateTeacher", teacher);
  }

  getTeachers(academicYearId?: string, includeInactive = false): Observable<TeacherViewModel[]> {
    const academicYearQuery = academicYearId ? `academicYearId=${academicYearId}&` : '';
    return this._httpClient.get<TeacherViewModel[]>(this.baseUrl + `api/Teacher/GetTeachers?${academicYearQuery}includeInactive=${includeInactive}`);
  }

  getTeacherById(teacherId: string): Observable<TeacherViewModel> {
    return this._httpClient.get<TeacherViewModel>(this.baseUrl + `api/Teacher/GetTeacherById?teacherId=${teacherId}`);
  }

  updateTeacherStatus(teacherId: string, status: TeacherStatusDto): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + `api/Teacher/UpdateTeacherStatus?teacherId=${teacherId}`, status);
  }

  deleteTeacher(teacherId: string): Observable<void> {
    return this._httpClient.delete<void>(this.baseUrl + `api/Teacher/DeleteTeacher?teacherId=${teacherId}`);
  }

  getTeacherAssignments(teacherId: string, academicYearId?: string | null): Observable<TeacherClassSectionViewModel[]> {
    const academicYearQuery = academicYearId ? `&academicYearId=${academicYearId}` : '';
    return this._httpClient.get<TeacherClassSectionViewModel[]>(this.baseUrl + `api/Teacher/GetTeacherAssignments?teacherId=${teacherId}${academicYearQuery}`);
  }

  addTeacherAssignment(teacherId: string, assignment: TeacherClassSectionDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + `api/Teacher/AddTeacherAssignment?teacherId=${teacherId}`, assignment);
  }

  updateTeacherAssignment(teacherId: string, assignment: TeacherClassSectionDto): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + `api/Teacher/UpdateTeacherAssignment?teacherId=${teacherId}`, assignment);
  }

  deleteTeacherAssignment(assignmentId: string): Observable<void> {
    return this._httpClient.delete<void>(this.baseUrl + `api/Teacher/DeleteTeacherAssignment?assignmentId=${assignmentId}`);
  }

  copyTeacherAssignments(teacherId: string, payload: TeacherAssignmentCopyDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + `api/Teacher/CopyTeacherAssignments?teacherId=${teacherId}`, payload);
  }

  uploadTeacherDocument(formData: FormData): Observable<TeacherDocumentViewModel> {
    return this._httpClient.post<TeacherDocumentViewModel>(this.baseUrl + "api/Teacher/UploadTeacherDocument", formData);
  }

  deleteTeacherDocument(documentId: string): Observable<void> {
    return this._httpClient.delete<void>(this.baseUrl + `api/Teacher/DeleteTeacherDocument?documentId=${documentId}`);
  }

  getTeacherAccount(teacherId: string): Observable<TeacherAccountViewModel> {
    return this._httpClient.get<TeacherAccountViewModel>(this.baseUrl + `api/Teacher/GetTeacherAccount?teacherId=${teacherId}`);
  }

  createTeacherUser(teacherId: string, account: TeacherAccountCreateDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + `api/Teacher/CreateTeacherUser?teacherId=${teacherId}`, account);
  }

  updateTeacherUserStatus(teacherId: string, status: TeacherAccountStatusDto): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + `api/Teacher/UpdateTeacherUserStatus?teacherId=${teacherId}`, status);
  }

  resetTeacherUserPassword(teacherId: string, payload: TeacherPasswordResetDto): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + `api/Teacher/ResetTeacherUserPassword?teacherId=${teacherId}`, payload);
  }

  getStudents(): Observable<StudentViewModel[]> {
    return this._httpClient.get<StudentViewModel[]>(this.baseUrl + "api/Student/GetStudent");
  }
  getStudentsByClass(classId: string): Observable<StudentViewModel[]> {
    return this._httpClient.get<StudentViewModel[]>(this.baseUrl + `api/Student/GetStudentByClassId?classId=${classId}`);
  }

  getStudentsByClassSectionId(classSectionId: string, examType?: number | null): Observable<StudentViewModel[]> {
    const examTypeQuery = examType ? `&examType=${examType}` : '';
    return this._httpClient.get<StudentViewModel[]>(this.baseUrl + `api/Student/GetStudentByClassSectionId?classSectionId=${classSectionId}${examTypeQuery}`);
  }
  getStudentCertificateDataByClassSectionId(classSectionId: string): Observable<StudentCertificateViewModel[]> {
    return this._httpClient.get<StudentCertificateViewModel[]>(this.baseUrl + `api/Student/GetStudentCertificateData?classSectionId=${classSectionId}`);
  }

  addClass(classRoom: ClassRoomDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/ClassSection/AddClass", classRoom);
  }

  updateClass(classRoom: ClassRoomDto): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + "api/ClassSection/UpdateClass", classRoom);
  }

  addSection(section: SectionDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/ClassSection/AddSection", section);
  }

  updateSection(section: SectionDto): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + "api/ClassSection/UpdateSection", section);
  }

  getSections(): Observable<SectionViewModel[]> {
    return this._httpClient.get<SectionViewModel[]>(this.baseUrl + "api/ClassSection/GetSections")
  }

  postClassSections(classSection: ClassSectionDto): Observable<any> {
    return this._httpClient.post<any>(this.baseUrl + "api/ClassSection/MapClassSection", classSection);
  }

  getClassRooms(): Observable<ClassRoomViewModel[]> {
    return this._httpClient.get<ClassRoomViewModel[]>(this.baseUrl + "api/ClassSection/GetClassRooms")
  }

  postCourse(course: CourseDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Course/AddCourse", course);
  }

  updateCourse(course: CourseDto): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + "api/Course/UpdateCourse", course);
  }

  getCourses(): Observable<CourseViewModel[]> {
    return this._httpClient.get<CourseViewModel[]>(this.baseUrl + "api/Course/GetCourse");
  }

  getAllClassCourse(): Observable<ClassCreditCourseViewModel[]> {
    return this._httpClient.get<ClassCreditCourseViewModel[]>(this.baseUrl + "api/Course/GetAllClassCourse");
  }

  postClassCourse(classCourseDto: ClassCourseDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Course/AddClassCourse", classCourseDto);
  }

  putClassCourse(classCourseDto: ClassCourseDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Course/UpdateClassCourse", classCourseDto);
  }

  deleteClassCourse(classCourseId: string): Observable<void> {
    return this._httpClient.delete<void>(this.baseUrl + `api/Course/DeleteClassCourse/${classCourseId}?classCourseId=${classCourseId}`);
  }

  getClassCourseByClassId(classId: string): Observable<ClassCreditCourseViewModel[]> {
    return this._httpClient.get<ClassCreditCourseViewModel[]>(this.baseUrl + `api/Course/GetClassCourseByClassId?classId=${classId}`);
  }

  postStudentMarks(studentmarks: SubjectMarkDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Exam/AddMarks", studentmarks);
  }
  updateStudentMarks(studentmarks: SubjectMarkDto): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + "api/Exam/UpdateMarks", studentmarks);
  }
  getStudentMarks(studentEnrollmentId: string, examType: number): Observable<SubjectMarksViewModel> {
    return this._httpClient.get<SubjectMarksViewModel>(this.baseUrl + `api/Exam/GetStudentMarks?studentEnrollmentId=${studentEnrollmentId}&examType=${examType}`);
  }

  getResult(studentEnrollmentId: string, examType?: number | null): Observable<ResultViewModel> {
    const examTypeQuery = examType ? `&examType=${examType}` : '';
    return this._httpClient.get<ResultViewModel>(this.baseUrl + `api/Exam/GetResult?studentEnrollmentId=${studentEnrollmentId}${examTypeQuery}`);
  }

  postFeeType(feeTypeData: FeeTypeDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Fees/AddFeeType", feeTypeData);
  }

  updateFeeType(feeTypeData: FeeTypeDto, feeTypeId: string): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + `api/Fees/UpdateFeeType?feeTypeId=${feeTypeId}`, feeTypeData);
  }

  getFeeType(): Observable<FeeTypeViewModel[]> {
    return this._httpClient.get<FeeTypeViewModel[]>(this.baseUrl + "api/Fees/GetFeeType");
  }

  postFeeStructure(feeStructure: FeeStructureDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Fees/AddFeeStructure", feeStructure);
  }

  applyFeeAdjustment(feeAdjustment: FeeAdjustmentDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Fees/ApplyFeeAdjustment", feeAdjustment);
  }

  ensureMissingMonthlyFees(studentEnrollmentIdGuid: string): Observable<void> {
    return this._httpClient.get<void>(this.baseUrl + `api/Fees/EnsureMissingMonthlyFees?studentEnrollmentIdGuid=${studentEnrollmentIdGuid}`);
  }

  putFeeStructure(feeStructure: FeeStructureDto, feeStructureId: string): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + `api/Fees/UpdateFeeStructure?feeStructureId=${feeStructureId}`, feeStructure);
  }

  getFeeStructure(classId: string): Observable<FeeStructureViewModel[]> {
    return this._httpClient.get<FeeStructureViewModel[]>(this.baseUrl + `api/Fees/GetFeeStructure?classId=${classId}`);
  }

  getFeeReport(classSectionId: string): Observable<FeeReportViewModel> {
    return this._httpClient.get<FeeReportViewModel>(this.baseUrl + `api/Fees/GetFeeReport?classSectionId=${classSectionId}`);
  }

  getStudentFeeSummary(studentId: string, classSectionId: string): Observable<StudentFeeSummaryViewModel> {
    return this._httpClient.get<StudentFeeSummaryViewModel>(this.baseUrl + `api/Fees/GetStudentFeeSummary?studentEnrollmentIdGuid=${studentId}&classSectionId=${classSectionId}`);
  }

  payStudentFee(studentFeeId: string, currentStudentEnrollmentId: string, amount: number, paymentMode: string): Observable<void> {
    return this._httpClient.post<void>(
      this.baseUrl + `api/Fees/PayStudentFee?studentFeeId=${studentFeeId}&currentStudentEnrollmentId=${currentStudentEnrollmentId}&amount=${amount}&paymentMode=${encodeURIComponent(paymentMode)}`,
      {}
    );
  }
  assignRegistrationAndSymbolNumber(paymentRequest: StudentStudentEnrollmentDto, studentEnrollmentId: string): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + `api/Student/AssignRegistrationAndSymbolNumber?studentEnrollmentId=${studentEnrollmentId}`, paymentRequest);
  }

  getRegAndSymCompliantStudent(classSectionId: any): Observable<StudentEnrollmentViewModel[]> {
    return this._httpClient.get<StudentEnrollmentViewModel[]>(this.baseUrl + `api/Student/GetRegAndSymCompliantEnrolledStudents?classSectionId=${classSectionId}`);
  }

  getPromotionCandidates(classSectionId: string, examType: number): Observable<PromotionCandidateViewModel[]> {
    return this._httpClient.get<PromotionCandidateViewModel[]>(this.baseUrl + `api/Student/GetPromotionCandidates?classSectionId=${classSectionId}&examType=${examType}`);
  }

  promoteStudents(request: PromoteStudentsDto): Observable<PromotionExecutionResultViewModel> {
    return this._httpClient.post<PromotionExecutionResultViewModel>(this.baseUrl + "api/Student/PromoteStudents", request);
  }

  sustainStudents(request: PromoteStudentsDto): Observable<PromotionExecutionResultViewModel> {
    return this._httpClient.post<PromotionExecutionResultViewModel>(this.baseUrl + "api/Student/SustainStudents", request);
  }

  manuallyPromoteStudents(request: PromoteStudentsDto): Observable<PromotionExecutionResultViewModel> {
    return this._httpClient.post<PromotionExecutionResultViewModel>(this.baseUrl + "api/Student/ManuallyPromoteStudents", request);
  }

  addStudentCertificateLog(certificateLog: StudentCertificateDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Student/AddStudentCertificateLog", certificateLog);
  }
  getStudentCertificateLog(certificateType: CertificateType): Observable<StudentCertificateLogViewModel> {
    return this._httpClient.get<StudentCertificateLogViewModel>(this.baseUrl + `api/Student/GetStudentCertificateLog?certificateType=${certificateType}`);
  }
}
