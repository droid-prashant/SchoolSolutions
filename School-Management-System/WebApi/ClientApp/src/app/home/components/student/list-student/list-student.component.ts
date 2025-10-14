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
  actionItems: MenuItem[] = [];
  students: StudentViewModel[] = [];

  constructor(private _apiService: ApiService) {
  }
  ngOnInit(): void {
    this.listStudent();
    this.actionItems = [
      {
        icon: PrimeIcons.EYE,
        command: () => {
          this.viewStudent();
        }
      }
    ];
  }
  viewStudent() {
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
