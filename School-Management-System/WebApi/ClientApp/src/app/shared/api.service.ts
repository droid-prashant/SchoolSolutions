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
import { FeeStructureViewModel } from '../home/components/fees/model/feeStructure.viewModel';
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

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  baseUrl: string = environment.API_BASE_URL;
  constructor(private _httpClient: HttpClient) { }

  postAcademicYear(academicYearDetail: AcademicYearDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Academic/AddAcademicYear", academicYearDetail);
  }

  getAcademicYear(): Observable<AcademicViewModel[]> {
    return this._httpClient.get<AcademicViewModel[]>(this.baseUrl + "api/Academic/GetAllAcademicYear");
  }

  putAcademicYear(academicYearDetail: AcademicYearDto, academicYearId: string): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + `api/Academic/UpdateAcademicYear?academicYearId?${academicYearId}`, academicYearDetail);
  }

  getProvinceDetails(): Observable<ProvinceViewModel[]> {
    return this._httpClient.get<ProvinceViewModel[]>(this.baseUrl + "api/MasterData/GetAllProvince");
  }

  postStudent(student: StudentDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Student/AddStudent", student);
  }

  getStudents(): Observable<StudentViewModel[]> {
    return this._httpClient.get<StudentViewModel[]>(this.baseUrl + "api/Student/GetStudent");
  }
  getStudentsByClass(classId: string): Observable<StudentViewModel[]> {
    return this._httpClient.get<StudentViewModel[]>(this.baseUrl + `api/Student/GetStudentByClassId?classId=${classId}`);
  }

  getStudentsByClassSectionId(classSectionId: string): Observable<StudentViewModel[]> {
    return this._httpClient.get<StudentViewModel[]>(this.baseUrl + `api/Student/GetStudentByClassSectionId?classSectionId=${classSectionId}`);
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

  getClassCourseByClassId(classId: string): Observable<ClassCreditCourseViewModel[]> {
    return this._httpClient.get<ClassCreditCourseViewModel[]>(this.baseUrl + `api/Course/GetClassCourseByClassId?classId=${classId}`);
  }

  postStudentMarks(studentmarks: SubjectMarkDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Exam/AddMarks", studentmarks);
  }

  getResult(studentEnrollmentId: string): Observable<ResultViewModel> {
    return this._httpClient.get<ResultViewModel>(this.baseUrl + `api/Exam/GetResult?studentEnrollmentId=${studentEnrollmentId}`);
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

  ensureMissingMonthlyFees(studentEnrollmentIdGuid: string): Observable<void> {
    return this._httpClient.get<void>(this.baseUrl + `api/Fees/EnsureMissingMonthlyFees?studentEnrollmentIdGuid=${studentEnrollmentIdGuid}`);
  }

  putFeeStructure(feeStructure: FeeStructureDto, feeStructureId: string): Observable<void> {
    return this._httpClient.put<void>(this.baseUrl + `api/Fees/UpdateFeeStructure?feeStructureId=${feeStructureId}`, feeStructure);
  }

  getFeeStructure(classId: string): Observable<FeeStructureViewModel[]> {
    return this._httpClient.get<FeeStructureViewModel[]>(this.baseUrl + `api/Fees/GetFeeStructure?classId=${classId}`);
  }

  getStudentFeeSummary(studentId: string, classSectionId: string): Observable<StudentFeeSummaryViewModel> {
    return this._httpClient.get<StudentFeeSummaryViewModel>(this.baseUrl + `api/Fees/GetStudentFeeSummary?studentEnrollmentIdGuid=${studentId}&classSectionId=${classSectionId}`);
  }

  payFees(paymentRequest: any): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Fees/AddFeeStructure", paymentRequest);
  }
  assignRegistrationAndSymbolNumber(paymentRequest: StudentStudentEnrollmentDto, studentEnrollmentId: string): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + `api/Student/AssignRegistrationAndSymbolNumber?studentEnrollmentId=${studentEnrollmentId}`, paymentRequest);
  }

  getRegAndSymCompliantStudent(classSectionId: any): Observable<StudentEnrollmentViewModel[]> {
    return this._httpClient.get<StudentEnrollmentViewModel[]>(this.baseUrl + `api/Student/GetRegAndSymCompliantEnrolledStudents?classSectionId=${classSectionId}`);
  }

  addStudentCertificateLog(certificateLog: StudentCertificateDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Student/AddStudentCertificateLog", certificateLog);
  }
  getStudentCertificateLog(certificateType: CertificateType): Observable<StudentCertificateLogViewModel> {
    return this._httpClient.get<StudentCertificateLogViewModel>(this.baseUrl + `api/Student/GetStudentCertificateLog?certificateType=${certificateType}`);
  }
}
