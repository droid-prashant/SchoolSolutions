import { Component, OnInit, OnChanges, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../../../shared/api.service';
import { Router } from '@angular/router';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { StudentDto } from '../shared/models/dtos/student.dto';
import { StudentViewModel } from '../shared/models/viewModels/student.viewModel';
import { ProvinceViewModel } from '../../../../shared/common/models/master/master.ViewModel';
import { DistrictViewModel } from '../../../../shared/common/models/master/district.ViewModel';
import { MunicipalityViewModel } from '../../../../shared/common/models/master/municipality.ViewModel';

@Component({
  selector: 'app-add-student',
  standalone: false,
  templateUrl: './add-student.component.html',
  styleUrl: './add-student.component.scss'
})
export class AddStudentComponent implements OnInit, OnChanges {
  @Input() isEditMode: boolean = false;
  @Input() studentToEdit: StudentViewModel | null = null;
  @Output() studentSaved = new EventEmitter<void>();

  genders: any[] = []
  classRooms: ClassRoomViewModel[] = []
  sections: SectionViewModel[] = [];
  provinceDetails: ProvinceViewModel[] = [];
  selectedDistricts: DistrictViewModel[] = [];
  selectedMunicipalities: MunicipalityViewModel[] = [];

  selectedClass!: ClassRoomViewModel
  studentForm: FormGroup
  constructor(private _fb: FormBuilder,
    private _apiService: ApiService,
    private _router: Router
  ) {
    let fb = this._fb;
    this.studentForm = fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      grandFatherName: ['', Validators.required],
      fatherName: ['', Validators.required],
      motherName: ['', Validators.required],
      gender: [null, Validators.required],
      age: [null],
      address: ['', Validators.required],
      dobNp: ['', Validators.required],
      dobEn: ['', Validators.required],
      wardNo: ['', Validators.required],
      contactNumber: ['', Validators.required],
      parentContactNumber: ['', Validators.required],
      parentEmail: [''],
      classSectionId: [null, Validators.required],
      provinceId: [null, Validators.required],
      districtId: [null, Validators.required],
      municipalityId: [null, Validators.required]
    });
  }
  ngOnInit(): void {
    this.genders = [
      { label: 'Male', value: 1 },
      { label: 'Female', value: 2 },
      { label: 'Other', value: 3 }
    ];

    this.getClassRooms();
    this.getProvinceDetails();

    this.studentForm.get('provinceId')?.valueChanges.subscribe(value => {
      if (value != null) {
        this.onProvinceChange(value);
      }
    });

    this.studentForm.get('districtId')?.valueChanges.subscribe(value => {
      if (value != null) {
        this.onDistrictChange(value);
      }
    });

    if (this.isEditMode && this.studentToEdit) {
      this.populateFormForEdit();
    }
  }

  ngOnChanges(): void {
    // Handle changes to inputs (e.g., when studentToEdit changes)
    if (this.isEditMode && this.studentToEdit) {
      this.populateFormForEdit();
      this.onProvinceChange(this.studentToEdit.provinceId);
    }
  }

  private populateFormForEdit(): void {
    if (!this.studentToEdit) {
      return;
    }

    // Load dependent dropdown data first
    const selectedProvince = this.provinceDetails.find(x => x.id == this.studentToEdit?.provinceId);
    this.selectedDistricts = selectedProvince?.districts ?? [];

    const selectedDistrict = this.selectedDistricts.find(x => x.id == this.studentToEdit?.districtId);
    this.selectedMunicipalities = selectedDistrict?.municipalities ?? [];

    // Patch without triggering valueChanges reset logic
    this.studentForm.patchValue({
      firstName: this.studentToEdit.firstName || '',
      lastName: this.studentToEdit.lastName || '',
      grandFatherName: (this.studentToEdit as any).grandFatherName || '',
      fatherName: this.studentToEdit.fatherName || '',
      motherName: this.studentToEdit.motherName || '',
      gender: this.studentToEdit.gender || null,
      age: this.studentToEdit.age || null,
      address: this.studentToEdit.address || '',
      contactNumber: (this.studentToEdit as any).contactNumber || '',
      parentContactNumber: (this.studentToEdit as any).parentContactNumber || '',
      parentEmail: (this.studentToEdit as any).parentEmail || '',
      provinceId: this.studentToEdit.provinceId || null,
      districtId: this.studentToEdit.districtId || null,
      municipalityId: this.studentToEdit.municipalityId || null,
      wardNo: (this.studentToEdit as any).wardNo || '',
      dobNp: (this.studentToEdit as any).dobNp || '',
      dobEn: (this.studentToEdit as any).dobEn || '',
      classSectionId: (this.studentToEdit as any).classSectionId || null
    }, { emitEvent: false });
  }

  onDobChange(event: { bs: string; ad: string }) {
    const dobNp = event.bs;
    const dobEn = event.ad;
    if (dobNp && dobEn) {
      this.studentForm.get('dobNp')?.setValue(dobNp);
      this.studentForm.get('dobEn')?.setValue(dobEn);
    }
  }

  getProvinceDetails() {
    this._apiService.getProvinceDetails().subscribe(
      {
        next: (response) => {
          this.provinceDetails = response;
        },
        error: (err) => console.log(err),
        complete: () => console.log("Request is complete")
      }
    )
  }

  getClassRooms() {
    this._apiService.getClassRooms().subscribe(
      {
        next: (response) => {
          this.classRooms = response;
        },
        error: (err) => console.log(err),
        complete: () => console.log("Request is complete")
      }
    )
  }

  saveStudent() {
    if (this.studentForm.invalid) {
      return;
    }

    let student: StudentDto = this.studentForm.value;

    if (this.isEditMode && this.studentToEdit) {
      // Update mode
      student.id = this.studentToEdit.id;
      this._apiService.updateStudent(student).subscribe(
        {
          next: (response) => {
            this.studentSaved.emit();
          },
          error: (err) => console.log(err),
          complete: () => console.log("Update request completed")
        }
      );
    } else {
      // Add mode
      this._apiService.postStudent(student).subscribe(
        {
          next: (response) => {
            this.studentSaved.emit();
          },
          error: (err) => console.log(err),
          complete: () => console.log("Add request completed")
        }
      );
    }
  }

  onClassRoomChange(event: any) {
    const classRoomId = event.value;
    if (classRoomId) {
      const selectedClass = this.classRooms.filter(x => x.id === classRoomId);
      const sections = selectedClass.map(x => x.sections);
      this.sections = sections[0];
    }
  }

  onSectionChange(event: any) {
    const sectionId = event.value;
    if (sectionId) {
      const selectedClassSection = this.sections.filter(x => x.sectionId === sectionId);
      const selectedCLassSectionId = selectedClassSection[0].classSectionId;
      this.studentForm.get('classSectionId')?.setValue(selectedCLassSectionId);
    }
  }

  onProvinceChange(provinceId: any) {
    this.selectedDistricts = [];
    this.selectedMunicipalities = [];

    if (provinceId) {
      const selectedProvince = this.provinceDetails.find(x => x.id == provinceId);
      if (selectedProvince) {
        this.selectedDistricts = selectedProvince.districts;
      }
    }

    // Reset only when user changes province manually
    this.studentForm.patchValue({
      districtId: null,
      municipalityId: null
    }, { emitEvent: false });
  }

  onDistrictChange(districtId: any) {
    this.selectedMunicipalities = [];

    if (districtId) {
      const selectedDistrict = this.selectedDistricts.find(x => x.id == districtId);
      if (selectedDistrict) {
        this.selectedMunicipalities = selectedDistrict.municipalities;
      }
    }

    // Reset municipality only when user changes district manually
    this.studentForm.patchValue({
      municipalityId: null
    }, { emitEvent: false });
  }
}
