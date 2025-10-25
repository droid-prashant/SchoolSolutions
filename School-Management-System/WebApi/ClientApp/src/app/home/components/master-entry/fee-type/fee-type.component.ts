import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../../../shared/api.service';
import { FeeTypeDto } from '../model/dtos/feeType.dto';
import { FeeTypeViewModel } from '../model/viewModels/feeType.viewModel';

@Component({
  selector: 'app-fee-type',
  standalone: false,
  templateUrl: './fee-type.component.html',
  styleUrl: './fee-type.component.scss'
})
export class FeeTypeComponent implements OnInit {
  feeTypeForm!: FormGroup
  feeTypes: FeeTypeViewModel[] = [];

  constructor(private _fb: FormBuilder, private _apiService: ApiService) {
    this.feeTypeForm = _fb.group({
      name: ['', Validators.required],
      isRecurring: [false],
      frequency: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.listFeeType();
  }

  addFeeType() {
    const feeTypeValue: FeeTypeDto = this.feeTypeForm.value
    this._apiService.postFeeType(feeTypeValue).subscribe({
      next: (res) => {
        this.listFeeType();
        this.resetForm();
      },
      error: (err) => {

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

  editFeeType(row: any) {

  }

  resetForm() {
    this.feeTypeForm.reset({
      name: '',
      isRecurring: false,
      frequency: ''
    });
  }
}
