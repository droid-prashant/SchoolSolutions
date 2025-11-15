import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { ResultViewModel } from '../shared/viewModels/result.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { StudentCertificateViewModel } from '../../certificate/model/studentCertificate.ViewModel';

@Component({
  selector: 'app-result',
  standalone: false,
  templateUrl: './result.component.html',
  styleUrl: './result.component.scss'
})
export class ResultComponent implements OnInit {
  showResultPreview: boolean = false;
  students: StudentViewModel[] = [];
  classRooms: ClassRoomViewModel[] = []
  sections: SectionViewModel[] = [];
  classId: string = "";
  student!: StudentViewModel;
  showResult: boolean = false;
  isClassSelected: boolean = false;
  result!: ResultViewModel
  classSectionId: string = "";
  constructor(private _apiService: ApiService) {

  }
  ngOnInit(): void {
    this.getClassRooms();
  }
  previewResult(student: StudentViewModel) {
    this.student = student;
    this.getResult(student.id);
    this.students
  }

  getResult(studentEnrollmentId: string) {
    this._apiService.getResult(studentEnrollmentId).subscribe(
      {
        next: (response) => {
          this.result = response;
          this.showResult = true;
        },
        error: (err) => {

        },
        complete: () => console.log("Req is completed")
      }
    )
  }

  listStudent() {
    this._apiService.getStudentsByClass(this.classId).subscribe(
      {
        next: (response) => {
          this.students = response;
        },
        error: (err) => {

        },
        complete: () => console.log("Req is completed")
      }
    )
  }
  onClassRoomChange(event: any) {
    this.classId = event.value;
    if (this.classId) {
      this.isClassSelected = true;
      const selectedClass = this.classRooms.filter(x => x.id === this.classId);
      const sections = selectedClass.map(x => x.sections);
      this.sections = sections[0];
    }
  }

  getClassRooms() {
    this._apiService.getClassRooms().subscribe(
      {
        next: (response) => {
          this.classRooms = response;
        },
        error: (err) => console.log(err),
        complete: () => console.log("Request is complete")
      }
    )
  }

  onClassSectionChange(event: any) {
    const classsSectionId = event.value;
    if (classsSectionId) {
      this.classSectionId = classsSectionId;
      this.isClassSelected = true;
      const selectedClassSection = this.sections.filter(x => x.classSectionId === classsSectionId);
      if (selectedClassSection) {
        this.getStudentByClassSection(classsSectionId);

      }
    }
  }

  getStudentByClassSection(classSectionId: string) {
    this._apiService.getStudentsByClassSectionId(classSectionId).subscribe({
      next: (response) => {
        this.students = response;
      },
      error: (err) => console.log(err),
      complete: () => console.log("Request is complete")
    });
  }
}
