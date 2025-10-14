import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StudentDto } from '../home/components/student/shared/models/dtos/student.dto';
import { StudentViewModel } from '../home/components/student/shared/models/viewModels/student.viewModel';
import { ClassSectionViewModel } from '../home/components/class-room/shared/models/viewModels/classSectionviewModel';
import { CourseDto } from '../home/components/course/shared/models/course.dto';
import { CourseViewModel } from '../home/components/course/shared/models/course.viewModel';
import { SubjectMarkDto } from '../home/components/exam/shared/models/examMarksEntry.dto';
import { ClassRoomViewModel } from '../home/components/class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../home/components/class-room/shared/models/viewModels/section.viewModel';
import { ClassSectionDto } from '../home/components/class-room/shared/models/dtos/classSection.dto';
import { SectionDto } from '../home/components/master-entry/model/dtos/section.dto';
import { ClassCreditCourseViewModel } from '../home/components/course/shared/models/classCourse.viewModel';
import { ClassCourseDto } from '../home/components/course/shared/models/classCourse.dto';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  baseUrl: string = environment.API_BASE_URL;
  constructor(private _httpClient: HttpClient) { }

  postStudent(student: StudentDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/Student/AddStudent", student);
  }

  getStudents(): Observable<StudentViewModel[]> {
    return this._httpClient.get<StudentViewModel[]>(this.baseUrl + "api/Student/GetStudent");
  }
  getStudentsByClass(classId: string): Observable<StudentViewModel[]> {
    return this._httpClient.get<StudentViewModel[]>(this.baseUrl + `api/Student/GetStudentByClassId?classId=${classId}`);
  }

  addSection(section: SectionDto): Observable<void> {
    return this._httpClient.post<void>(this.baseUrl + "api/ClassSection/AddSection", section);
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
}
