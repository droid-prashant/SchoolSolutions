import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ApiService } from '../../../../shared/api.service';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';

@Component({
  selector: 'app-class-entry',
  standalone: false,
  templateUrl: './class-entry.component.html',
  styleUrl: './class-entry.component.scss'
})
export class ClassEntryComponent implements OnInit {
  classForm: FormGroup

  classList: ClassRoomViewModel[] = [];
  constructor(private _formBuilder: FormBuilder,
    private _apiService: ApiService
  ) {
    let fb = _formBuilder;
    this.classForm = fb.group({
      name: [],
      roomNumber: []
    })
  }

  ngOnInit(): void {
    this.listClass();
  }

  addClass() { }

  listClass() {
    this._apiService.getClassRooms().subscribe({
      next: (response) => {
        this.classList = response;
      },
      error: (err) => {

      },
      complete: () => console.log("Req Complete")
    })
  }

  EditClass(selectedClass: ClassRoomViewModel) {
    
  }
}
