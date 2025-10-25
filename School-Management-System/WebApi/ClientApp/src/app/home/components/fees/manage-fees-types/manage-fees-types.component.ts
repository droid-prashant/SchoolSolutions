import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { ApiService } from '../../../../shared/api.service';
import { FeeTypeViewModel } from '../../master-entry/model/viewModels/feeType.viewModel';
import { FeeStructureDto } from '../model/feeStructure.dto';
import { FeeStructureViewModel } from '../model/feeStructure.viewModel';
import { finalize } from 'rxjs';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-manage-fees-types',
  standalone: false,
  templateUrl: './manage-fees-types.component.html',
  styleUrl: './manage-fees-types.component.scss'
})
export class ManageFeesTypesComponent implements OnInit {
  feeStructureForm!: FormGroup
  feeStrucures: FeeStructureViewModel[] = [];
  classRooms: ClassRoomViewModel[] = []
  feeTypes: FeeTypeViewModel[] = [];
  classId: string = "";
  isNewRow: boolean = false;
  isUpdateRow: boolean = false;
  isClassSelected: boolean = false;
  feeStructureId: string = "";

  constructor(private _apiService: ApiService, private _fb: FormBuilder, private _messageService: MessageService) {
    this.feeStructureForm = this._fb.group({
      classId: ['', Validators.required],
      feeTypeId: ['', Validators.required],
      amount: ['', Validators.required],
      description: ['', Validators.required]
    });
  }
  ngOnInit(): void {
    this.getClassRooms();
    this.listFeeType();
  }

  getClassRooms() {
    this._apiService.getClassRooms().subscribe(
      {
        next: (response) => {
          this.classRooms = response;
        },
        error: (err) => {
           this._messageService.add({ severity: 'fail', summary: 'Fail', detail: 'failed to load Class rooms' });
        }
      }
    )
  }

  listFeeType() {
    this._apiService.getFeeType().subscribe({
      next: (res) => {
        this.feeTypes = res;
      },
      error: (err) => {
        this._messageService.add({ severity: 'fail', summary: 'Fail', detail: 'failed to load Fee Types' });
      }
    });
  }

  listFeeStructure() {
    this._apiService.getFeeStructure(this.classId).subscribe({
      next: (res) => {
        this.feeStrucures = res;
      },
      error: (err) => {
        this._messageService.add({ severity: 'fail', summary: 'Fail', detail: 'failed to load Fee Structure' });
      }
    });
  }

  onClassRoomChange(event: any) {
    this.classId = event.value;
    this.feeStructureForm.get('classId')?.setValue(this.classId);
    this.isClassSelected = true;
    this.listFeeStructure()
  }

  addNewRow() {
    this.isNewRow = true;
  }

  onRowSubmit() {
    if (!this.isUpdateRow) {
      this.onSave();
    }
    else {
      this.onUpdate();
    }
  }

  onRowCancel() {

  }
  onSave() {
    const feeStructure: FeeStructureDto = this.feeStructureForm.value;
    this._apiService.postFeeStructure(feeStructure).pipe
      (
        finalize(() => {
          this.isNewRow = false;
        })
      ).subscribe({
        next: (res) => {
          this._messageService.add({ severity: 'success', summary: 'Success', detail: 'successfully configured Fee Structure' });
          this.listFeeStructure();
          this.resetForm();
        },
        error: (err) => {
          this._messageService.add({ severity: 'fail', summary: 'Fail', detail: 'failed to configure Fee Structure' });
        }
      });
  }

  onUpdate() {
    const feeStructure: FeeStructureDto = this.feeStructureForm.value;
    this._apiService.putFeeStructure(feeStructure, this.feeStructureId).pipe
      (finalize(() => {
        this.isUpdateRow = false;
      })
      ).subscribe({
        next: (res) => {
          this._messageService.add({ severity: 'success', summary: 'Success', detail: 'successfully updated Fee Structure' });
          this.isUpdateRow = false;
          this.listFeeStructure();
          this.resetForm();
        },
        error: (err) => {
          this._messageService.add({ severity: 'fail', summary: 'Fail', detail: 'failed to update Fee Structure' });
        }
      });
  }

  onRowEdit(feeStructure: any) {
    this.feeStructureId = feeStructure.id;
    this.isUpdateRow = true;
    this.resetForm();
    this.feeStructureForm.patchValue(feeStructure);
  }

  resetForm() {
    this.feeStructureForm.reset();
    this.feeStructureForm.markAsPristine();
    this.feeStructureForm.markAsUntouched();
    this.feeStructureForm.updateValueAndValidity();
  }
}
