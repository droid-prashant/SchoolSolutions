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
import { StudentsByClassViewModel } from '../home/components/dashboard/model/studentsByClass.viewModel';
import { SubjectMarksViewModel } from '../home/components/exam/shared/viewModels/student-marks.viewModel';

@Injectable({
  providedIn: 'root'
})
export class MasterApiService {
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
  getProvinceDetails(): Observable<ProvinceViewModel[]> {
    return this._httpClient.get<ProvinceViewModel[]>(this.baseUrl + "api/MasterData/GetAllProvince");
  }

  getAllSections(): Observable<SectionViewModel[]> {
    return this._httpClient.get<SectionViewModel[]>(this.baseUrl + "api/MasterData/GetAllSection")
  }

  getClassRooms(): Observable<ClassRoomViewModel[]> {
    return this._httpClient.get<ClassRoomViewModel[]>(this.baseUrl + "api/MasterData/GetAllClass")
  }

  getCourses(): Observable<CourseViewModel[]> {
    return this._httpClient.get<CourseViewModel[]>(this.baseUrl + "api/MasterData/GetAllCourses")
  }

  getAcademicYears(): Observable<AcademicViewModel[]> {
    return this._httpClient.get<AcademicViewModel[]>(this.baseUrl + "api/MasterData/GetAllAcademicYear")
  }

}
