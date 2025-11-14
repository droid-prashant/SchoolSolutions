import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { ResultViewModel } from '../shared/viewModels/result.viewModel';

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
  classId: string = "";
  student!: StudentViewModel;
  showResult: boolean = false;
  isClassSelected: boolean = false;
  result!: ResultViewModel
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
      this.listStudent();
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
}
