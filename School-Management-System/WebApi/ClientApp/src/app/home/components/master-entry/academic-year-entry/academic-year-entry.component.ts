import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { AcademicYearDto } from '../model/dtos/academicYear.dto';
import { ApiService } from '../../../../shared/api.service';
import { AcademicViewModel } from '../model/viewModels/academicYear.ViewModel';

@Component({
  selector: 'app-academic-year-entry',
  standalone: false,
  templateUrl: './academic-year-entry.component.html',
  styleUrl: './academic-year-entry.component.scss'
})
export class AcademicYearEntryComponent implements OnInit {
  academicYearForm!: FormGroup
  academicYearList: AcademicViewModel[] = [];
  submitButtonLabel: string = "Save";
  academicYearId: string = "";

  isSubmitted: boolean = false;
  isUpdate: boolean = false;

  constructor(private _apiService: ApiService, private _fb: FormBuilder, private _messageService: MessageService) {
    this.academicYearForm = this._fb.group({
      yearName: ['', Validators.required],
      isActive: [false, Validators.required],
      startDate: [new Date(), Validators.required],
      endDate: [new Date(), Validators.required]
    });
  }
  ngOnInit(): void {
    this.listAcademicYear();
  }

  submit() {
    if (!this.isUpdate) {
      this.onSave();
    }
    else {
      this.onUpdate();
    }
  }

  listAcademicYear() {
    this._apiService.getAcademicYear().subscribe({
      next: (res) => {
        this.academicYearList = res;
      },
      error: (res) => {
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load Academic Year list' });
      }
    })
  }

  onSave() {
    this.isSubmitted = true;
    if (!this.academicYearForm.valid) {
      return;
    }
    const academicYearFormValue: AcademicYearDto = this.academicYearForm.value;
    this._apiService.postAcademicYear(academicYearFormValue).subscribe({
      next: (res) => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Academic Year added Successfully' });
      },
      error: (err) => {
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to add Academic Year' });
      },
      complete: () => {
        this.isSubmitted = false;
        this.resetForm();
      }
    });
  }

  onUpdate() {
    this.isSubmitted = true;
    if (!this.academicYearForm.valid) {
      return;
    }
    const academicYearFormValue: AcademicYearDto = this.academicYearForm.value;
    this._apiService.putAcademicYear(academicYearFormValue, this.academicYearId).subscribe({
      next: (res) => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Academic Year updated Successfully' });
      },
      error: (err) => {
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update Academic Year' });
      },
      complete: () => {
        this.isSubmitted = false;
        this.isUpdate = false;
        this.resetForm();
      }
    });
  }

  editAcademicYear(academicYear: AcademicViewModel) {
    this.isUpdate = true;
    this.academicYearId = academicYear.id;
    this.submitButtonLabel = "Update";
    this.academicYearForm.patchValue(academicYear);
  }

  resetForm() {
    this.academicYearForm.reset({
      yearName: '',
      isActive: false,
      startDate: new Date(),
      endDate: new Date()
    });
  }
}
