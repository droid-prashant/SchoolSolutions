import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../../../shared/api.service';
import { FeeTypeViewModel } from '../../master-entry/model/viewModels/feeType.viewModel';
import { FeeStructureDto } from '../model/feeStructure.dto';
import { FeeStructureViewModel } from '../model/feeStructure.viewModel';
import { finalize } from 'rxjs';
import { MessageService } from 'primeng/api';
import { AuthService } from '../../../../shared/auth.service';
import { LookupService } from '../../../../shared/common/lookup.service';
import { FilterSelection } from '../../student-filter/student-filter.component';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';

@Component({
  selector: 'app-manage-fees-types',
  standalone: false,
  templateUrl: './manage-fees-types.component.html',
  styleUrl: './manage-fees-types.component.scss'
})
export class ManageFeesTypesComponent implements OnInit {
  feeStructureForm!: FormGroup
  feeStrucures: FeeStructureViewModel[] = [];
  feeTypes: FeeTypeViewModel[] = [];
  classId: string = "";
  isNewRow: boolean = false;
  isUpdateRow: boolean = false;
  isClassSelected: boolean = false;
  feeStructureId: string = "";
  currentAcademicYearId: string = "";
  hasLoadedClassFeeStructures: boolean = false;
  classOptions: ClassRoomViewModel[] = [];
  selectedClassId: string | null = null;

  constructor(
    private _apiService: ApiService,
    private _fb: FormBuilder,
    private _messageService: MessageService,
    private _authService: AuthService,
    private _lookupService: LookupService
  ) {
    this.feeStructureForm = this._fb.group({
      academicYearId: [''],
      classId: ['', Validators.required],
      feeTypeId: ['', Validators.required],
      amount: ['', Validators.required],
      description: ['', Validators.required]
    });
  }
  ngOnInit(): void {
    this.currentAcademicYearId = this._authService.getCurrentAcademicYearId();
    this.feeStructureForm.get('academicYearId')?.setValue(this.currentAcademicYearId);
    this.listFeeType();
    this.loadClasses();
  }

  loadClasses(): void {
    this._lookupService.getClassRooms().subscribe({
      next: classes => this.classOptions = classes ?? [],
      error: () => this._messageService.add({ severity: 'error', summary: 'Class Load Failed', detail: 'Failed to load classes.' })
    });
  }

  listFeeType(forceRefresh = false) {
    this._lookupService.getfeeTypes(forceRefresh).subscribe({
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

  onLoadStudents(filter: FilterSelection) {
    this.classId = filter.classId ?? "";
    this.selectedClassId = this.classId || null;
    this.loadSelectedClassFeeStructures();
  }

  onClassChange(): void {
    this.classId = this.selectedClassId ?? "";
    this.feeStrucures = [];
    this.isClassSelected = !!this.classId;
    this.isNewRow = false;
    this.isUpdateRow = false;
    this.hasLoadedClassFeeStructures = false;
    this.resetForm();
  }

  loadSelectedClassFeeStructures(): void {
    this.classId = this.selectedClassId ?? this.classId;
    this.feeStructureForm.get('classId')?.setValue(this.classId);
    this.isClassSelected = !!this.classId;
    this.isNewRow = false;
    this.isUpdateRow = false;
    this.hasLoadedClassFeeStructures = !!this.classId;
    this.resetForm();

    if (this.classId) {
      this.listFeeStructure();
    } else {
      this.feeStrucures = [];
      this.hasLoadedClassFeeStructures = false;
    }
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
    this.isNewRow = false;
    this.isUpdateRow = false;
    this.resetForm();
  }
  onSave() {
    this.feeStructureForm.get('academicYearId')?.setValue(this.currentAcademicYearId);
    if (this.classId) {
      this.feeStructureForm.get('classId')?.setValue(this.classId);
    }
    if (!this.feeStructureForm.valid) {
      return;
    }
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
    this.feeStructureForm.get('academicYearId')?.setValue(this.currentAcademicYearId);
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
    this.feeStructureForm.reset({
      academicYearId: this.currentAcademicYearId
    });
    this.feeStructureForm.markAsPristine();
    this.feeStructureForm.markAsUntouched();
    this.feeStructureForm.updateValueAndValidity();
  }
}
