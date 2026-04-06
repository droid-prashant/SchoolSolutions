import { Component, OnInit } from '@angular/core';
import { MenuItem, MessageService, PrimeIcons } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { StudentViewModel } from '../shared/models/viewModels/student.viewModel';

@Component({
  selector: 'app-list-student',
  standalone: false,
  templateUrl: './list-student.component.html',
  styleUrl: './list-student.component.scss'
})
export class ListStudentComponent implements OnInit {
  students: StudentViewModel[] = [];
  isEditDialogVisible: boolean = false;
  selectedStudent: StudentViewModel | null = null;

  constructor(private _apiService: ApiService, private _messageService: MessageService) {
  }
  ngOnInit(): void {
    this.listStudent();
  }

  getActionItems(student: StudentViewModel): MenuItem[] {
    return [
      {
        icon: PrimeIcons.PENCIL,
        tooltipOptions: { tooltipLabel: 'Edit', tooltipPosition: 'top' },
        command: () => {
          this.editStudent(student);
        }
      }
    ];
  }

  editStudent(student: StudentViewModel) {
    this.selectedStudent = { ...student };
    this.isEditDialogVisible = true;
  }

  getGender(genderId: number): string {
    switch (genderId) {
      case 1:
        return 'Male';
      case 2:
        return 'Female';
      case 3:
        return 'Other';
      default:
        return 'Unknown';
    }
  }

  getClassName(classRoomId: string): string {
    const classRoom = this.students.find(s => s.classRoomId === classRoomId);
    return classRoom ? classRoom.classRoomName : 'Unknown';
  }

  getSectionName(sectionId: string): string {
    const section = this.students.find(s => s.sectionId === sectionId);
    return section ? section.sectionName : 'Unknown';
  }

  onStudentSaved() {
    if (this.isEditDialogVisible) {
      this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Student updated successfully' });
    } else {
      this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Student added successfully' });
    }
    this.isEditDialogVisible = false;
    this.selectedStudent = null;
    this.listStudent();
  }
  listStudent() {
    this._apiService.getStudents().subscribe(
      {
        next: (response) => {
          this.students = response;
          console.log(this.students);
        },
        error: (err) => {

        },
        complete: () => console.log("Req is completed")
      }
    )
  }
}
