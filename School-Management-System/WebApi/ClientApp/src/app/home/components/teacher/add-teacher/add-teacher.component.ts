import { Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { HttpErrorResponse } from '@angular/common/http';
import { ApiService } from '../../../../shared/api.service';
import { extractApiErrorMessage } from '../../../../shared/http-error.util';
import { TeacherDto } from '../shared/models/dtos/teacher.dto';
import { TeacherViewModel } from '../shared/models/viewModels/teacher.viewModel';
import { ProvinceViewModel } from '../../../../shared/common/models/master/master.ViewModel';
import { DistrictViewModel } from '../../../../shared/common/models/master/district.ViewModel';
import { MunicipalityViewModel } from '../../../../shared/common/models/master/municipality.ViewModel';

@Component({
  selector: 'app-add-teacher',
  standalone: false,
  templateUrl: './add-teacher.component.html',
  styleUrl: './add-teacher.component.scss'
})
export class AddTeacherComponent implements OnInit, OnChanges {
  @Input() isEditMode = false;
  @Input() teacherToEdit: TeacherViewModel | null = null;
  @Output() teacherSaved = new EventEmitter<void>();

  teacherForm: FormGroup;
  genders = [
    { label: 'Male', value: 1 },
    { label: 'Female', value: 2 },
    { label: 'Other', value: 3 }
  ];
  provinceDetails: ProvinceViewModel[] = [];
  selectedDistricts: DistrictViewModel[] = [];
  selectedMunicipalities: MunicipalityViewModel[] = [];
  isSaving = false;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private messageService: MessageService
  ) {
    this.teacherForm = this.fb.group({
      id: [null],
      employeeCode: [''],
      firstName: ['', Validators.required],
      middleName: [''],
      lastName: ['', Validators.required],
      gender: [null, Validators.required],
      age: [null],
      dateOfBirthNp: [''],
      dateOfBirthEn: [''],
      contactNumber: ['', Validators.required],
      alternateContactNumber: [''],
      email: ['', Validators.email],
      provinceId: [null],
      districtId: [null],
      municipalityId: [null],
      wardNo: [null],
      fatherName: [''],
      motherName: [''],
      designation: [''],
      joiningDateNp: [''],
      joiningDateEn: [''],
      createUser: [true],
      userName: [''],
      password: [''],
      qualifications: this.fb.array([]),
      experiences: this.fb.array([])
    });
  }

  get qualifications(): FormArray {
    return this.teacherForm.get('qualifications') as FormArray;
  }

  get experiences(): FormArray {
    return this.teacherForm.get('experiences') as FormArray;
  }

  ngOnInit(): void {
    this.loadLookups();
    this.teacherForm.get('createUser')?.valueChanges.subscribe(createUser => {
      this.updateUserValidators(createUser);
    });
    this.updateUserValidators(this.teacherForm.get('createUser')?.value);
    this.teacherForm.get('provinceId')?.valueChanges.subscribe(value => {
      this.onProvinceChange(value);
    });
    this.teacherForm.get('districtId')?.valueChanges.subscribe(value => {
      this.onDistrictChange(value);
    });

    if (!this.isEditMode) {
      this.addQualification();
    }
  }

  ngOnChanges(): void {
    if (this.isEditMode && this.teacherToEdit) {
      this.populateForm();
    }
  }

  loadLookups(): void {
    this.apiService.getProvinceDetails().subscribe(provinces => {
      this.provinceDetails = provinces;
      if (this.isEditMode && this.teacherToEdit?.provinceId) {
        this.selectedDistricts = provinces.find(x => x.id === this.teacherToEdit?.provinceId)?.districts ?? [];
        this.selectedMunicipalities = this.selectedDistricts.find(x => x.id === this.teacherToEdit?.districtId)?.municipalities ?? [];
      }
    });
  }

  updateUserValidators(createUser: boolean): void {
    const userName = this.teacherForm.get('userName');
    const password = this.teacherForm.get('password');
    const needsUserCredentials = createUser && (!this.isEditMode || !this.teacherToEdit?.userId);
    if (needsUserCredentials) {
      userName?.setValidators([Validators.required]);
      password?.setValidators([Validators.required, Validators.minLength(6)]);
    } else {
      userName?.clearValidators();
      password?.clearValidators();
    }
    userName?.updateValueAndValidity({ emitEvent: false });
    password?.updateValueAndValidity({ emitEvent: false });
  }

  populateForm(): void {
    if (!this.teacherToEdit) {
      return;
    }

    this.qualifications.clear();
    this.experiences.clear();

    this.teacherForm.patchValue({
      id: this.teacherToEdit.id,
      employeeCode: this.teacherToEdit.employeeCode ?? '',
      firstName: this.teacherToEdit.firstName,
      middleName: this.teacherToEdit.middleName ?? '',
      lastName: this.teacherToEdit.lastName,
      gender: this.teacherToEdit.gender,
      age: this.teacherToEdit.age ?? null,
      dateOfBirthNp: this.teacherToEdit.dateOfBirthNp ?? '',
      dateOfBirthEn: this.teacherToEdit.dateOfBirthEn ?? '',
      contactNumber: this.teacherToEdit.contactNumber,
      alternateContactNumber: this.teacherToEdit.alternateContactNumber ?? '',
      email: this.teacherToEdit.email ?? '',
      provinceId: this.teacherToEdit.provinceId ?? null,
      districtId: this.teacherToEdit.districtId ?? null,
      municipalityId: this.teacherToEdit.municipalityId ?? null,
      wardNo: this.teacherToEdit.wardNo ?? null,
      fatherName: this.teacherToEdit.fatherName ?? '',
      motherName: this.teacherToEdit.motherName ?? '',
      designation: this.teacherToEdit.designation ?? '',
      joiningDateNp: this.teacherToEdit.joiningDateNp ?? '',
      joiningDateEn: this.teacherToEdit.joiningDateEn ?? '',
      createUser: !this.teacherToEdit.userId
    }, { emitEvent: false });

    this.teacherToEdit.qualifications?.forEach(qualification => this.qualifications.push(this.createQualificationGroup(qualification)));
    this.teacherToEdit.experiences?.forEach(experience => this.experiences.push(this.createExperienceGroup(experience)));

    if (!this.qualifications.length) {
      this.addQualification();
    }
    this.updateUserValidators(this.teacherForm.get('createUser')?.value);
  }

  addQualification(): void {
    this.qualifications.push(this.createQualificationGroup());
  }

  removeQualification(index: number): void {
    this.qualifications.removeAt(index);
  }

  addExperience(): void {
    this.experiences.push(this.createExperienceGroup());
  }

  removeExperience(index: number): void {
    this.experiences.removeAt(index);
  }

  createQualificationGroup(value: any = {}): FormGroup {
    return this.fb.group({
      id: [value.id ?? null],
      degreeName: [value.degreeName ?? '', Validators.required],
      institutionName: [value.institutionName ?? '', Validators.required],
      boardOrUniversity: [value.boardOrUniversity ?? ''],
      passedYear: [value.passedYear ?? ''],
      gradeOrPercentage: [value.gradeOrPercentage ?? ''],
      majorSubject: [value.majorSubject ?? ''],
      remarks: [value.remarks ?? '']
    });
  }

  createExperienceGroup(value: any = {}): FormGroup {
    return this.fb.group({
      id: [value.id ?? null],
      organizationName: [value.organizationName ?? '', Validators.required],
      designation: [value.designation ?? '', Validators.required],
      subjectOrDepartment: [value.subjectOrDepartment ?? ''],
      startDateNp: [value.startDateNp ?? '', Validators.required],
      startDateEn: [value.startDateEn ?? '', Validators.required],
      endDateNp: [value.endDateNp ?? ''],
      endDateEn: [value.endDateEn ?? ''],
      isCurrent: [value.isCurrent ?? false],
      remarks: [value.remarks ?? '']
    });
  }

  onProvinceChange(provinceId: number | null): void {
    this.selectedDistricts = [];
    this.selectedMunicipalities = [];
    if (provinceId) {
      this.selectedDistricts = this.provinceDetails.find(x => x.id === provinceId)?.districts ?? [];
    }

    const currentDistrictId = this.teacherForm.get('districtId')?.value;
    if (currentDistrictId && !this.selectedDistricts.some(x => x.id === currentDistrictId)) {
      this.teacherForm.get('districtId')?.setValue(null, { emitEvent: false });
      this.teacherForm.get('municipalityId')?.setValue(null, { emitEvent: false });
    }
  }

  onDistrictChange(districtId: number | null): void {
    this.selectedMunicipalities = [];
    if (districtId) {
      this.selectedMunicipalities = this.selectedDistricts.find(x => x.id === districtId)?.municipalities ?? [];
    }

    const currentMunicipalityId = this.teacherForm.get('municipalityId')?.value;
    if (currentMunicipalityId && !this.selectedMunicipalities.some(x => x.id === currentMunicipalityId)) {
      this.teacherForm.get('municipalityId')?.setValue(null, { emitEvent: false });
    }
  }

  onDateOfBirthChange(event: { bs: string; ad: string }): void {
    this.teacherForm.patchValue({
      dateOfBirthNp: event.bs,
      dateOfBirthEn: event.ad
    });
  }

  onJoiningDateChange(event: { bs: string; ad: string }): void {
    this.teacherForm.patchValue({
      joiningDateNp: event.bs,
      joiningDateEn: event.ad
    });
  }

  onExperienceStartDateChange(index: number, event: { bs: string; ad: string }): void {
    this.experiences.at(index).patchValue({
      startDateNp: event.bs,
      startDateEn: event.ad
    });
  }

  onExperienceEndDateChange(index: number, event: { bs: string; ad: string }): void {
    this.experiences.at(index).patchValue({
      endDateNp: event.bs,
      endDateEn: event.ad
    });
  }

  saveTeacher(): void {
    if (this.teacherForm.invalid || this.isSaving) {
      this.teacherForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const teacher = this.prepareTeacherDto();
    const request = this.isEditMode ? this.apiService.updateTeacher(teacher) : this.apiService.postTeacher(teacher);

    request.subscribe({
      next: () => this.teacherSaved.emit(),
      error: err => {
        this.isSaving = false;
        const detail = err instanceof HttpErrorResponse
          ? extractApiErrorMessage(err)
          : 'Failed to save teacher';
        this.messageService.add({ severity: 'error', summary: 'Error', detail });
      },
      complete: () => this.isSaving = false
    });
  }

  prepareTeacherDto(): TeacherDto {
    const value = this.teacherForm.value;
    return {
      ...value,
      qualifications: value.qualifications ?? [],
      experiences: value.experiences ?? [],
      assignments: []
    };
  }
}
