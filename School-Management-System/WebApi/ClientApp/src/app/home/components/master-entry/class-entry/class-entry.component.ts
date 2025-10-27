import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../../../shared/api.service';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { ClassRoomDto } from '../model/dtos/classRoom.dto';
import { MessageService } from 'primeng/api';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-class-entry',
  standalone: false,
  templateUrl: './class-entry.component.html',
  styleUrl: './class-entry.component.scss'
})
export class ClassEntryComponent implements OnInit {
  classForm: FormGroup
  submitButtonLabel: string = "Save"
  classList: ClassRoomViewModel[] = [];
  isUpdate: boolean = false;
  isSubmitted: boolean = false;
  constructor(private _formBuilder: FormBuilder, private _apiService: ApiService, private _messageService: MessageService
  ) {
    let fb = _formBuilder;
    this.classForm = fb.group({
      id: [''],
      name: ['', Validators.required],
      orderNumber: [0, Validators.required]
    })
  }

  ngOnInit(): void {
    this.listClass();
  }

  submit() {
    if (!this.isUpdate) {
      this.addClass();
    }
    else {
      this.updateClass();
    }
  }
  addClass() {
    this.isSubmitted = true;
    if (!this.classForm.valid) {
      return;
    }
    const classFormValue: ClassRoomDto = this.classForm.value;
    this._apiService.addClass(classFormValue).subscribe({
      next: (response) => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Added Class' });
      },
      error: (err) => {
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to add Class' });
      },
      complete: () => {
        this.isSubmitted = false;
        this.listClass();
        this.classForm.reset();
      }
    });
  }

  updateClass() {
    this.isSubmitted = true;
    if (!this.classForm.valid) {
      return;
    }
    const classFormValue: ClassRoomDto = this.classForm.value;
    this._apiService.updateClass(classFormValue).subscribe({
      next: (response) => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Updated Class' });
      },
      error: (err) => {
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update Class' });
      },
      complete: () => {
        this.isSubmitted = false;
        this.isUpdate = false;
        this.listClass();
        this.submitButtonLabel = "Save";
        this.classForm.reset();
      }
    })
  }

  listClass() {
    this._apiService.getClassRooms().subscribe({
      next: (response) => {
        this.classList = response;
      },
      error: (err) => {
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load Class' });
      }
    })
  }

  editClass(selectedClass: ClassRoomDto) {
    this.isUpdate = true;
    this.classForm.patchValue(selectedClass);
    this.submitButtonLabel = "Update"
  }
}
