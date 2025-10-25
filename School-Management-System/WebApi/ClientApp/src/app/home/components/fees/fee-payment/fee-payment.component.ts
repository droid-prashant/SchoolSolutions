import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ApiService } from '../../../../shared/api.service';
import { MessageService } from 'primeng/api';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { StudentFeeSummaryViewModel } from '../model/studentFeeSummary.viewModel';

@Component({
  selector: 'app-fee-payment',
  standalone: false,
  templateUrl: './fee-payment.component.html',
  styleUrl: './fee-payment.component.scss'
})
export class FeePaymentComponent implements OnInit {
  classRooms: ClassRoomViewModel[] = []
  sections: SectionViewModel[] = [];
  students: StudentViewModel[] = [];
  studentFeeSummary!: StudentFeeSummaryViewModel;
  classId: string = "";
  classSectionId: string = "";
  isClassSelected: boolean = false;
  isClassSectionSelected: boolean = false;
  isFeeSummaryFetched: boolean = false;

  constructor(private _fb: FormBuilder, private _apiService: ApiService, private _messageService: MessageService) { }

  ngOnInit() {
    this.getClassRooms();
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

  onClassSectionChange(event: any) {
    const classsSectionId = event.value;
    if (classsSectionId) {
      this.isClassSectionSelected = true;
      const selectedClassSection = this.sections.filter(x => x.classSectionId === classsSectionId);
      this.classSectionId = selectedClassSection[0].classSectionId;
      this.getStudentByClassSection(this.classSectionId);
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

  getStudentByClassSection(classSectionId: string) {
    this._apiService.getStudentsByClassSectionId(classSectionId).subscribe({
      next: (response) => {
        this.students = response;
      },
      error: (err) => console.log(err),
      complete: () => console.log("Request is complete")
    });
  }

  onStudentChange(event: any) {
    const studentId = event.value;
    if (studentId) {
      this._apiService.ensureMissingMonthlyFees(studentId).subscribe({
        next: (response) => {
          this.getStudentFeeSummary(studentId);
        },
        error: (err) => console.log(err),
      });
    }
  }

  getStudentFeeSummary(studentId: string) {
    this._apiService.getStudentFeeSummary(studentId, this.classSectionId).subscribe({
      next: (response) => {
        this.studentFeeSummary = response;
        if (this.studentFeeSummary) {
          this.isFeeSummaryFetched = true;
        }
      },
      error: (err) => console.log(err),
      complete: () => console.log("Request is complete")
    });
  }
}
