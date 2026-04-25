import { Component, EventEmitter, Input, OnChanges, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { LookupService } from '../../../../shared/common/lookup.service';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { CourseViewModel } from '../../course/shared/models/course.viewModel';
import { AcademicViewModel } from '../../master-entry/model/viewModels/academicYear.ViewModel';
import { TeacherDto } from '../shared/models/dtos/teacher.dto';
import { TeacherDocumentViewModel, TeacherViewModel } from '../shared/models/viewModels/teacher.viewModel';
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
  @Input() academicYearId: string | null = null;
  @Output() teacherSaved = new EventEmitter<void>();

  teacherForm: FormGroup;
  genders = [
    { label: 'Male', value: 1 },
    { label: 'Female', value: 2 },
    { label: 'Other', value: 3 }
  ];
  classRooms: ClassRoomViewModel[] = [];
  courses: CourseViewModel[] = [];
  academicYears: AcademicViewModel[] = [];
  provinceDetails: ProvinceViewModel[] = [];
  selectedDistricts: DistrictViewModel[] = [];
  selectedMunicipalities: MunicipalityViewModel[] = [];
  isSaving = false;
  documentType = '';
  documentTitle = '';
  selectedDocument: File | null = null;
  isUploadingDocument = false;
  documentTypes = [
    { label: 'Citizenship', value: 'Citizenship' },
    { label: 'CV / Resume', value: 'CV' },
    { label: 'Qualification Certificate', value: 'Qualification Certificate' },
    { label: 'Experience Letter', value: 'Experience Letter' },
    { label: 'Appointment Letter', value: 'Appointment Letter' },
    { label: 'PAN / Tax Document', value: 'PAN / Tax Document' },
    { label: 'Other', value: 'Other' }
  ];

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private lookupService: LookupService,
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
      experiences: this.fb.array([]),
      assignments: this.fb.array([])
    });
  }

  get qualifications(): FormArray {
    return this.teacherForm.get('qualifications') as FormArray;
  }

  get experiences(): FormArray {
    return this.teacherForm.get('experiences') as FormArray;
  }

  get assignments(): FormArray {
    return this.teacherForm.get('assignments') as FormArray;
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
      this.addAssignment();
    }
  }

  ngOnChanges(): void {
    if (this.isEditMode && this.teacherToEdit) {
      this.populateForm();
    }
  }

  loadLookups(): void {
    this.lookupService.getClassRooms().subscribe(classes => this.classRooms = classes);
    this.lookupService.getCourses().subscribe(courses => this.courses = courses);
    this.apiService.getAcademicYear().subscribe(years => this.academicYears = years);
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
    this.assignments.clear();

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
    this.teacherToEdit.assignments?.forEach(assignment => this.assignments.push(this.createAssignmentGroup({
      ...assignment,
      classRoomId: assignment.classRoomId,
      sectionId: assignment.sectionId
    })));

    if (!this.qualifications.length) {
      this.addQualification();
    }
    if (!this.assignments.length) {
      this.addAssignment();
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

  addAssignment(): void {
    this.assignments.push(this.createAssignmentGroup({
      academicYearId: this.academicYearId ?? this.academicYears.find(x => x.isActive)?.id
    }));
  }

  removeAssignment(index: number): void {
    this.assignments.removeAt(index);
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

  createAssignmentGroup(value: any = {}): FormGroup {
    return this.fb.group({
      id: [value.id ?? null],
      academicYearId: [value.academicYearId ?? this.academicYearId ?? null, Validators.required],
      classRoomId: [value.classRoomId ?? null, Validators.required],
      sectionId: [value.sectionId ?? null, Validators.required],
      classSectionId: [value.classSectionId ?? null, Validators.required],
      courseId: [value.courseId ?? null, Validators.required],
      isClassTeacher: [value.isClassTeacher ?? false],
      remarks: [value.remarks ?? '']
    });
  }

  getSections(index: number): SectionViewModel[] {
    const classRoomId = this.assignments.at(index).get('classRoomId')?.value;
    return this.classRooms.find(x => x.id === classRoomId)?.sections ?? [];
  }

  onClassRoomChange(index: number): void {
    this.assignments.at(index).patchValue({
      sectionId: null,
      classSectionId: null
    });
  }

  onSectionChange(index: number): void {
    const sectionId = this.assignments.at(index).get('sectionId')?.value;
    const section = this.getSections(index).find(x => x.sectionId === sectionId);
    this.assignments.at(index).get('classSectionId')?.setValue(section?.classSectionId ?? null);
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

    if (this.hasDuplicateAssignments()) {
      this.messageService.add({ severity: 'warn', summary: 'Duplicate Assignment', detail: 'Same academic year, class, section, and course is selected more than once.' });
      return;
    }

    this.isSaving = true;
    const teacher = this.prepareTeacherDto();
    const request = this.isEditMode ? this.apiService.updateTeacher(teacher) : this.apiService.postTeacher(teacher);

    request.subscribe({
      next: () => this.teacherSaved.emit(),
      error: err => {
        this.isSaving = false;
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err?.error?.message ?? 'Failed to save teacher' });
      },
      complete: () => this.isSaving = false
    });
  }

  onDocumentSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedDocument = input.files?.[0] ?? null;
  }

  uploadDocument(fileInput: HTMLInputElement): void {
    if (this.isUploadingDocument) {
      return;
    }

    if (!this.teacherToEdit?.id || !this.selectedDocument || !this.documentType.trim() || !this.documentTitle.trim()) {
      this.messageService.add({ severity: 'warn', summary: 'Missing Document Detail', detail: 'Select file, type, and title before upload.' });
      return;
    }

    this.isUploadingDocument = true;
    const formData = new FormData();
    formData.append('teacherId', this.teacherToEdit.id);
    formData.append('documentType', this.documentType);
    formData.append('documentTitle', this.documentTitle);
    formData.append('file', this.selectedDocument);

    this.apiService.uploadTeacherDocument(formData).subscribe({
      next: document => {
        if (this.teacherToEdit) {
          this.teacherToEdit.documents = [...(this.teacherToEdit.documents ?? []), document];
        }
        this.documentType = '';
        this.documentTitle = '';
        this.selectedDocument = null;
        fileInput.value = '';
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Document uploaded successfully' });
      },
      error: err => this.messageService.add({ severity: 'error', summary: 'Error', detail: err?.error?.message ?? 'Failed to upload document' }),
      complete: () => this.isUploadingDocument = false
    });
  }

  viewDocument(document: TeacherDocumentViewModel): void {
    const url = this.getDocumentUrl(document);
    if (url) {
      window.open(url, '_blank', 'noopener');
    }
  }

  getDocumentUrl(document: TeacherDocumentViewModel): string {
    if (!document?.id) {
      return '';
    }

    const baseUrl = this.apiService.baseUrl.endsWith('/') ? this.apiService.baseUrl : `${this.apiService.baseUrl}/`;
    return `${baseUrl}api/Teacher/ViewTeacherDocument?documentId=${encodeURIComponent(document.id)}`;
  }

  formatFileSize(fileSize: number): string {
    if (!fileSize) {
      return '-';
    }

    if (fileSize < 1024 * 1024) {
      return `${Math.ceil(fileSize / 1024)} KB`;
    }

    return `${(fileSize / (1024 * 1024)).toFixed(1)} MB`;
  }

  deleteDocument(documentId: string): void {
    this.apiService.deleteTeacherDocument(documentId).subscribe({
      next: () => {
        if (this.teacherToEdit) {
          this.teacherToEdit.documents = this.teacherToEdit.documents.filter(x => x.id !== documentId);
        }
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Document removed successfully' });
      },
      error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to remove document' })
    });
  }

  prepareTeacherDto(): TeacherDto {
    const value = this.teacherForm.value;
    return {
      ...value,
      qualifications: value.qualifications ?? [],
      experiences: value.experiences ?? [],
      assignments: (value.assignments ?? []).map((assignment: any) => ({
        id: assignment.id,
        academicYearId: assignment.academicYearId,
        classSectionId: assignment.classSectionId,
        courseId: assignment.courseId,
        isClassTeacher: assignment.isClassTeacher,
        remarks: assignment.remarks
      }))
    };
  }

  hasDuplicateAssignments(): boolean {
    const keys = new Set<string>();
    for (const assignment of this.assignments.value) {
      const key = `${assignment.academicYearId}-${assignment.classSectionId}-${assignment.courseId}`;
      if (keys.has(key)) {
        return true;
      }
      keys.add(key);
    }
    return false;
  }
}
