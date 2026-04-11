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
import { MessageService } from 'primeng/api';
import { LookupService } from '../../../../shared/common/lookup.service';

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
    private _messageService: MessageService,
    private _lookupService: LookupService
  ) {
    let fb = this._fb;
    this.studentForm = fb.group({
      id: [null],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      grandFatherName: ['', Validators.required],
      fatherName: ['', Validators.required],
      motherName: ['', Validators.required],
      gender: [null, Validators.required],
      age: [null],
      dobNp: ['', Validators.required],
      dobEn: ['', Validators.required],
      wardNo: ['', Validators.required],
      contactNumber: ['', Validators.required],
      parentContactNumber: ['', Validators.required],
      parentEmail: [''],
      classRoomId: [null, Validators.required],
      sectionId: [null, Validators.required],
      classSectionId: [null, Validators.required],
      provinceId: [null, Validators.required],
      districtId: [null, Validators.required],
      municipalityId: [null, Validators.required],
      rollNumber: [null]
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
    if (this.isEditMode && this.studentToEdit) {
      this.populateFormForEdit();
    }
  }

  populateFormForEdit(): void {
    if (!this.studentToEdit) {
      return;
    }

    const student: any = this.studentToEdit;
    const selectedProvince = this.provinceDetails.find(
      x => x.id === student.provinceId
    );
    this.selectedDistricts = selectedProvince?.districts ?? [];

    const selectedDistrict = this.selectedDistricts.find(
      x => x.id === student.districtId
    );
    this.selectedMunicipalities = selectedDistrict?.municipalities ?? [];

    this.onClassRoomChange({ value: student.classRoomId });
    this.studentForm.patchValue({
      id: student.id,
      firstName: student.firstName || '',
      lastName: student.lastName || '',
      grandFatherName: student.grandFatherName || '',
      fatherName: student.fatherName || '',
      motherName: student.motherName || '',
      gender: student.gender,
      age: student.age ?? null,
      contactNumber: student.contactNumber || '',
      parentContactNumber: student.parentContactNumber || '',
      parentEmail: student.parentEmail || '',
      classRoomId: student.classRoomId ?? null,
      sectionId: student.sectionId ?? null,
      classSectionId: student.classSectionId ?? null,
      address: student.address || '',
      provinceId: student.provinceId ?? null,
      districtId: student.districtId ?? null,
      municipalityId: student.municipalityId ?? null,
      wardNo: student.wardNo ?? '',
      dobNp: student.dateOfBirthNp || '',
      dobEn: student.dateOfBirthEn || '',
      rollNumber: student.rollNumber || null
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
    this._lookupService.getClassRooms().subscribe(classes => {
        this.classRooms = classes;
      });
  }

  saveStudent() {
    if (this.studentForm.invalid) {
      return;
    }
    let student: StudentDto = this.studentForm.value;
    if (this.isEditMode && this.studentToEdit) {
      student.id = this.studentToEdit.id;
      this.updateStudent(student);
    } else {
      this.addStudent(student);
    }
  }

  updateStudent(student: StudentDto) {
    this._apiService.updateStudent(student).subscribe(
      {
        next: (response) => {
          this.studentSaved.emit();
        },
        error: (err) => {
          console.log(err);
          this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update student' });
        }
      }
    );
  }

  addStudent(student: StudentDto) {
    this._apiService.postStudent(student).subscribe(
      {
        next: (response) => {
          this.studentSaved.emit();
        },
        error: (err) => {
          console.log(err);
          this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to add student' });
        }
      }
    );
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

    // user change only
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

    this.studentForm.patchValue({
      municipalityId: null
    }, { emitEvent: false });
  }
}
