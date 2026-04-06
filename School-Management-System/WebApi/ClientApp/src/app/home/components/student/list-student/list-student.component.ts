import { Component, OnInit } from '@angular/core';
import { MenuItem, PrimeIcons } from 'primeng/api';
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

  constructor(private _apiService: ApiService) {
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
    this.selectedStudent = student;
    this.isEditDialogVisible = true;
  }

  onStudentSaved() {
    this.isEditDialogVisible = false;
    this.selectedStudent = null;
    this.listStudent(); // Refresh the list
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
