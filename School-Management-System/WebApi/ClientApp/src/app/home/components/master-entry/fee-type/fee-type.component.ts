import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../../../shared/api.service';
import { FeeTypeDto } from '../model/dtos/feeType.dto';
import { FeeTypeViewModel } from '../model/viewModels/feeType.viewModel';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-fee-type',
  standalone: false,
  templateUrl: './fee-type.component.html',
  styleUrl: './fee-type.component.scss'
})
export class FeeTypeComponent implements OnInit {
  feeTypeForm!: FormGroup
  feeTypes: FeeTypeViewModel[] = [];
  isSubmitted: boolean = false;
  isUpdate: boolean = false;
  submitButtonLabel: string = "Save";
  feeTypeId: string = "";

  constructor(private _fb: FormBuilder, private _apiService: ApiService, private _messageService: MessageService) {
    this.feeTypeForm = _fb.group({
      name: ['', Validators.required],
      isRecurring: [false],
      frequency: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.listFeeType();
  }

  submit() {
    if (!this.isUpdate) {
      this.addFeeType();
    }
    else {
      this.updateFeeType();
    }
  }

  addFeeType() {
    this.isSubmitted = true;
    if (!this.feeTypeForm.valid) {
      return;
    }
    const feeTypeValue: FeeTypeDto = this.feeTypeForm.value
    this._apiService.postFeeType(feeTypeValue).subscribe({
      next: (res) => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Fee Type Added' });
        this.listFeeType();
        this.resetForm();
      },
      error: (err) => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Failed to add fee type ' });
      },
      complete: () => {
        this.isSubmitted = false;
        this.listFeeType();
        this.resetForm();
      }
    });
  }

  updateFeeType() {
    this.isSubmitted = true;
    if (!this.feeTypeForm.valid) {
      return;
    }
    const feeTypeValue: FeeTypeDto = this.feeTypeForm.value
    this._apiService.updateFeeType(feeTypeValue, this.feeTypeId).subscribe({
      next: (res) => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Fee Type Updated' });
      },
      error: (err) => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Failed to update fee type' });
      },
      complete: () => {
        this.isSubmitted = false;
        this.isUpdate = false;
        this.listFeeType();
        this.resetForm();
        this.submitButtonLabel = "Save";
      }
    });
  }

  listFeeType() {
    this._apiService.getFeeType().subscribe({
      next: (res) => {
        this.feeTypes = res;
      },
      error: (err) => {

      }
    });
  }

  editFeeType(row: FeeTypeViewModel) {
    this.isUpdate = true;
    this.feeTypeForm.patchValue(row);
    if (row.isRecurring === 'Yes') {
      this.feeTypeForm.get('isRecurring')?.setValue(true);
    }
    this.feeTypeId = row.id
    this.submitButtonLabel = "Update";
  }

  resetForm() {
    this.feeTypeForm.reset({
      id: '',
      name: '',
      isRecurring: false,
      frequency: ''
    });
  }
}
