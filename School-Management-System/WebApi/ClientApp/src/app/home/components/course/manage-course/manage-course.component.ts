import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { CourseViewModel } from '../shared/models/course.viewModel';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClassCreditCourseViewModel } from '../shared/models/classCourse.viewModel';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { ClassCourseDto } from '../shared/models/classCourse.dto';
import { ConfirmationService, MessageService } from 'primeng/api';
import { LookupService } from '../../../../shared/common/lookup.service';
import { FilterSelection } from '../../student-filter/student-filter.component';

@Component({
  selector: 'app-manage-course',
  standalone: false,
  templateUrl: './manage-course.component.html',
  styleUrl: './manage-course.component.scss'
})
export class ManageCourseComponent implements OnInit {
  courses: CourseViewModel[] = [];
  masterCourses: CourseViewModel[] = [];
  classCourses: ClassCreditCourseViewModel[] = [];
  classCourseForm: FormGroup;
  classRooms: ClassRoomViewModel[] = [];
  classCourseId: string = '';
  currentFilter: FilterSelection = {};

  isNewRow = false;
  isUpdateRow = false;
  isSubmitted = false;

  constructor(
    private _apiService: ApiService,
    private _lookupService: LookupService,
    private _formBuilder: FormBuilder,
    private _messageService: MessageService,
    private _confirmationService: ConfirmationService
  ) {
    this.classCourseForm = this._formBuilder.group({
      classRoomId: ['', Validators.required],
      courseId: ['', Validators.required],

      isTheoryRequired: [true],
      theoryCreditHour: ['', Validators.required],
      theoryFullMarks: ['', Validators.required],

      isPracticalRequired: [true],
      practicalCreditHour: ['', Validators.required],
      practicalFullMarks: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.listCourses();
    this.listClass();
    this.registerCheckboxHandlers();
  }

  registerCheckboxHandlers(): void {
    this.classCourseForm.get('isTheoryRequired')?.valueChanges.subscribe((checked: boolean) => {
      this.toggleCourseFields('theory', !checked);
    });

    this.classCourseForm.get('isPracticalRequired')?.valueChanges.subscribe((checked: boolean) => {
      this.toggleCourseFields('practical', !checked);
    });
  }

  toggleCourseFields(type: 'theory' | 'practical', disabled: boolean): void {
    const creditControl = this.classCourseForm.get(`${type}CreditHour`);
    const marksControl = this.classCourseForm.get(`${type}FullMarks`);

    if (disabled) {
      creditControl?.setValue(null);
      marksControl?.setValue(null);
      creditControl?.clearValidators();
      marksControl?.clearValidators();
      creditControl?.disable();
      marksControl?.disable();
    } else {
      creditControl?.enable();
      marksControl?.enable();
      creditControl?.setValidators([Validators.required]);
      marksControl?.setValidators([Validators.required]);
    }

    creditControl?.updateValueAndValidity();
    marksControl?.updateValueAndValidity();
  }

  private hasConfiguredValue(value: number | string | null | undefined): boolean {
    if (value === null || value === undefined || value === '') {
      return false;
    }

    return Number(value) > 0;
  }

  private resolveTheoryRequired(classCourse: ClassCreditCourseViewModel): boolean {
    const hasTheoryValues =
      this.hasConfiguredValue(classCourse.theoryCreditHour) || this.hasConfiguredValue(classCourse.theoryFullMarks);

    if (classCourse.isTheoryRequired === false && hasTheoryValues) {
      return true;
    }

    if (classCourse.isTheoryRequired !== null && classCourse.isTheoryRequired !== undefined) {
      return classCourse.isTheoryRequired;
    }

    return hasTheoryValues;
  }

  private resolvePracticalRequired(classCourse: ClassCreditCourseViewModel): boolean {
    const hasPracticalValues =
      this.hasConfiguredValue(classCourse.practicalCreditHour) || this.hasConfiguredValue(classCourse.practicalFullMarks);

    if (classCourse.isPracticalRequired === false && hasPracticalValues) {
      return true;
    }

    if (classCourse.isPracticalRequired !== null && classCourse.isPracticalRequired !== undefined) {
      return classCourse.isPracticalRequired;
    }

    return hasPracticalValues;
  }

  buildClassCourseDto(): ClassCourseDto {
    const formValue = this.classCourseForm.getRawValue();
    const isTheoryRequired = formValue.isTheoryRequired;
    const isPracticalRequired = formValue.isPracticalRequired;

    return {
      classCourseId: this.classCourseId,
      classRoomId: formValue.classRoomId,
      courseId: formValue.courseId,
      isTheoryRequired,
      isPracticalRequired,
      theoryCreditHour: isTheoryRequired ? formValue.theoryCreditHour : null,
      theoryFullMarks: isTheoryRequired ? formValue.theoryFullMarks : null,
      practicalCreditHour: isPracticalRequired ? formValue.practicalCreditHour : null,
      practicalFullMarks: isPracticalRequired ? formValue.practicalFullMarks : null
    };
  }

  onClassChange(event: any): void {
    const classRoomId = event.value;
    if (classRoomId) {
      this.classCourseForm.get('classRoomId')?.setValue(classRoomId);
      this.listClassCourseByClassId(classRoomId);
    }
  }

  listClass(): void {
    this._lookupService.getClassRooms().subscribe({
      next: (response) => {
        this.classRooms = response;
      },
      error: () => {
        this._messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load classes.'
        });
      }
    });
  }

  listAllClassCourse(): void {
    this._apiService.getAllClassCourse().subscribe({
      next: (response) => {
        this.classCourses = response;
      },
      error: () => {
        this._messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load class courses.'
        });
      }
    });
  }

  listClassCourseByClassId(classRoomId: string): void {
    this._apiService.getClassCourseByClassId(classRoomId).subscribe({
      next: (response) => {
        this.classCourses = response;
        this.filterCourse();
      },
      error: () => {
        this._messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load class courses by class.'
        });
      }
    });
  }

  listCourses(): void {
    this._lookupService.getCourses().subscribe({
      next: (response) => {
        this.courses = response;
        this.masterCourses = response;
      },
      error: () => {
        this._messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load courses.'
        });
      }
    });
  }

  addNewRow(): void {
    const selectedClassId = this.classCourseForm.get('classRoomId')?.value;

    if (!selectedClassId) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Warning',
        detail: 'Please select a class first.'
      });
      return;
    }

    this.isSubmitted = false;
    this.isUpdateRow = false;
    this.isNewRow = true;
    this.resetFormKeepingClass(selectedClassId);
    this.filterCourse();
  }

  onRowSubmit(): void {
    this.isSubmitted = true;

    if (this.classCourseForm.invalid) {
      this.classCourseForm.markAllAsTouched();
      this._messageService.add({
        severity: 'warn',
        summary: 'Validation',
        detail: 'Please fill all required fields.'
      });
      return;
    }

    const classCourseDto = this.buildClassCourseDto();

    if (this.isUpdateRow && !this.isNewRow) {
      this.onRowUpdate(classCourseDto);
    } else {
      this.onRowSave(classCourseDto);
    }
  }

  onRowUpdate(classCourseDto: ClassCourseDto): void {
    classCourseDto.classCourseId = this.classCourseId;

    this._apiService.putClassCourse(classCourseDto).subscribe({
      next: () => {
        this._messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Successfully updated class course.'
        });

        const classId = this.classCourseForm.get('classRoomId')?.value;
        this.listClassCourseByClassId(classId);
        this.reset();
      },
      error: () => {
        this._messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to update class course.'
        });
      }
    });
  }

  onRowSave(classCourseDto: ClassCourseDto): void {
    this._apiService.postClassCourse(classCourseDto).subscribe({
      next: () => {
        this._messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Successfully configured class course.'
        });

        const classId = this.classCourseForm.get('classRoomId')?.value;
        this.listClassCourseByClassId(classId);
        this.reset();
      },
      error: () => {
        this._messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to configure class course.'
        });
      }
    });
  }

  onRowEdit(classCourse: ClassCreditCourseViewModel): void {
    this.courses = [...this.masterCourses];
    this.classCourseId = classCourse.classCreditCourseId;
    const isTheoryRequired = this.resolveTheoryRequired(classCourse);
    const isPracticalRequired = this.resolvePracticalRequired(classCourse);

    this.isSubmitted = false;
    this.isNewRow = false;
    this.isUpdateRow = true;

    this.classCourseForm.reset();
    this.patchClassCourseForm(classCourse, isTheoryRequired, isPracticalRequired);
    this.filterCourseForEdit(classCourse.courseId);
  }

  getCourseName(): string {
    return this.classRooms.find(x => x.id === this.classCourseForm.get('classRoomId')?.value)?.name || '-';
  }

  patchClassCourseForm(classCourse: ClassCreditCourseViewModel, isTheoryRequired: boolean, isPracticalRequired: boolean): void {
    this.classCourseForm.patchValue({
      classRoomId: classCourse.classRoomId,
      courseId: classCourse.courseId,
      isTheoryRequired,
      theoryCreditHour: classCourse.theoryCreditHour,
      isPracticalRequired,
      practicalCreditHour: classCourse.practicalCreditHour,
      theoryFullMarks: classCourse.theoryFullMarks,
      practicalFullMarks: classCourse.practicalFullMarks
    });

    this.toggleCourseFields('theory', !isTheoryRequired);
    this.toggleCourseFields('practical', !isPracticalRequired);
  }

  onRowCancel(): void {
    this.reset();
  }

  onRowDelete(classCourse: ClassCreditCourseViewModel): void {
    this._confirmationService.confirm({
      header: 'Delete Class Course',
      message: `Are you sure you want to delete the mapping for "${classCourse.courseName}"?`,
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this._apiService.deleteClassCourse(classCourse.classCreditCourseId).subscribe({
          next: () => {
            this._messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Successfully deleted class course.'
            });

            if (this.classCourseId === classCourse.classCreditCourseId) {
              this.reset();
            }

            const classId = this.classCourseForm.get('classRoomId')?.value || classCourse.classRoomId;
            if (classId) {
              this.listClassCourseByClassId(classId);
            } else {
              this.listAllClassCourse();
              this.filterCourse();
            }
          },
          error: () => {
            this._messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to delete class course.'
            });
          }
        });
      }
    });
  }

  onLoadStudents(filter: FilterSelection): void {
    this.currentFilter = filter;
    const classId = filter.classId;

    this.reset();

    if (!classId) {
      this.classCourses = [];
      this.courses = [...this.masterCourses];
      this.classCourseForm.get('classRoomId')?.setValue('');
      return;
    }

    this.classCourseForm.get('classRoomId')?.setValue(classId);
    this.listClassCourseByClassId(classId);
  }

  filterCourse(): void {
    if (this.classCourses.length > 0) {
      const classCourseIds = this.classCourses.map(x => x.courseId);
      this.courses = this.masterCourses.filter(x => !classCourseIds.includes(x.id));
    } else {
      this.courses = [...this.masterCourses];
    }
  }

  filterCourseForEdit(currentCourseId: string): void {
    const existingCourseIds = this.classCourses
      .map(x => x.courseId)
      .filter(id => id !== currentCourseId);

    this.courses = this.masterCourses.filter(x => !existingCourseIds.includes(x.id));
  }

  reset(): void {
    const classId = this.classCourseForm.get('classRoomId')?.value;
    this.resetFormKeepingClass(classId);
    this.isNewRow = false;
    this.isUpdateRow = false;
    this.isSubmitted = false;
  }

  resetFormKeepingClass(classId: string): void {
    this.classCourseForm.reset({
      classRoomId: classId || '',
      courseId: '',
      isTheoryRequired: true,
      theoryCreditHour: '',
      theoryFullMarks: '',
      isPracticalRequired: true,
      practicalCreditHour: '',
      practicalFullMarks: ''
    });

    this.classCourseForm.get('theoryCreditHour')?.enable();
    this.classCourseForm.get('theoryFullMarks')?.enable();
    this.classCourseForm.get('practicalCreditHour')?.enable();
    this.classCourseForm.get('practicalFullMarks')?.enable();

    this.classCourseForm.get('theoryCreditHour')?.setValidators([Validators.required]);
    this.classCourseForm.get('theoryFullMarks')?.setValidators([Validators.required]);
    this.classCourseForm.get('practicalCreditHour')?.setValidators([Validators.required]);
    this.classCourseForm.get('practicalFullMarks')?.setValidators([Validators.required]);

    this.classCourseForm.get('theoryCreditHour')?.updateValueAndValidity();
    this.classCourseForm.get('theoryFullMarks')?.updateValueAndValidity();
    this.classCourseForm.get('practicalCreditHour')?.updateValueAndValidity();
    this.classCourseForm.get('practicalFullMarks')?.updateValueAndValidity();
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.classCourseForm.get(fieldName);
    return !!(field && field.invalid && (field.touched || this.isSubmitted));
  }
}
