import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormGroup,
  Validators
} from '@angular/forms';
import { MessageService } from 'primeng/api';

import { ApiService } from '../../../../shared/api.service';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { ExamTerminal } from '../shared/models/examTerminal.dto';
import { SubjectMarkDto } from '../shared/models/examMarksEntry.dto';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { ClassCreditCourseViewModel } from '../../course/shared/models/classCourse.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { FilterSelection } from '../../student-filter/student-filter.component';
import { LookupService } from '../../../../shared/common/lookup.service';

@Component({
  selector: 'app-exam-marks-entry',
  standalone: false,
  templateUrl: './exam-marks-entry.component.html',
  styleUrl: './exam-marks-entry.component.scss'
})
export class ExamMarksEntryComponent implements OnInit {
  classRooms: ClassRoomViewModel[] = [];
  students: StudentViewModel[] = [];
  courses: ClassCreditCourseViewModel[] = [];
  sections: SectionViewModel[] = [];
  examTerminals: ExamTerminal[] = [];

  classId: string = '';
  studentName: string = '';
  currentFilter: FilterSelection = {};

  submitButtonLabel: string = 'Save';
  isClassSectionSelected: boolean = false;
  isStudentMarksUpdate: boolean = false;
  isSubmitted: boolean = false;
  isClassSelected: boolean = false;
  isMarksEntryVisible: boolean = false;

  studentMarks: FormGroup;

  constructor(
    private _apiService: ApiService,
    private _formBuilder: FormBuilder,
    private _messageService: MessageService,
    private _lookupService: LookupService
  ) {
    this.examTerminals = [
      { id: 1, terminalName: 'First Terminal' },
      { id: 2, terminalName: 'Second Terminal' },
      { id: 3, terminalName: 'Third Terminal' },
      { id: 4, terminalName: 'Final Terminal' }
    ];

    this.studentMarks = this._formBuilder.group({
      studentId: [null, Validators.required],
      examType: [null, Validators.required],
      attendance: [null, [Validators.required, Validators.min(0), Validators.max(365)]],
      totalSchoolDays: [null, [Validators.required, Validators.min(0), Validators.max(365)]],
      studentMarksLists: this._formBuilder.array([], Validators.required)
    });
  }

  ngOnInit(): void {
    // this.getClassRooms();
  }

  get studentMarksLists(): FormArray {
    return this.studentMarks.get('studentMarksLists') as FormArray;
  }

  getFormGroup(index: number): FormGroup {
    return this.studentMarksLists.at(index) as FormGroup;
  }

  getControl(controlName: string): AbstractControl | null {
    return this.studentMarks.get(controlName);
  }

  getRowControl(rowIndex: number, controlName: string): AbstractControl | null {
    return this.getFormGroup(rowIndex).get(controlName);
  }

  isInvalid(controlName: string): boolean {
    const control = this.getControl(controlName);
    return !!(control && control.invalid && (control.touched || this.isSubmitted));
  }

  isRowControlInvalid(rowIndex: number, controlName: string): boolean {
    const control = this.getRowControl(rowIndex, controlName);
    return !!(control && control.invalid && (control.touched || this.isSubmitted));
  }

  createStudentMarksFormArray(): void {
    const marksArray = this.studentMarksLists;
    marksArray.clear();

    this.courses.forEach(course => {
      const isTheoryRequired = course.isTheoryRequired;
      const isPracticalRequired = course.isPracticalRequired;

      marksArray.push(
        this._formBuilder.group({
          classCourseId: [course.classCreditCourseId, Validators.required],
          isTheoryRequired: [isTheoryRequired],
          isPracticalRequired: [isPracticalRequired],
          theoryCredit: [course.theoryCreditHour],
          practicalCredit: [course.practicalCreditHour],
          theoryFullMarks: [course.theoryFullMarks],
          practicalFullMarks: [course.practicalFullMarks],
          obtainedTheoryMarks: [
            isTheoryRequired ? null : null,
            isTheoryRequired && course.theoryFullMarks != null
              ? [
                  Validators.required,
                  Validators.min(0),
                  Validators.max(course.theoryFullMarks)
                ]
              : []
          ],
          obtainedPracticalMarks: [
            isPracticalRequired ? null : null,
            isPracticalRequired && course.practicalFullMarks != null
              ? [
                  Validators.required,
                  Validators.min(0),
                  Validators.max(course.practicalFullMarks)
                ]
              : []
          ]
        })
      );
    });

    marksArray.controls.forEach((control, index) => {
      const course = this.courses[index];
      if (!course.isTheoryRequired) {
        control.get('obtainedTheoryMarks')?.disable({ emitEvent: false });
        control.get('obtainedTheoryMarks')?.setValue(null, { emitEvent: false });
      }

      if (!course.isPracticalRequired) {
        control.get('obtainedPracticalMarks')?.disable({ emitEvent: false });
        control.get('obtainedPracticalMarks')?.setValue(null, { emitEvent: false });
      }
    });
  }

  onLoadStudents(filter: FilterSelection): void {
    this.currentFilter = filter;

    if (filter && filter.classSectionId && filter.classId) {
      this.listSubject(filter.classId);
      this.listStudentByClassSection(filter.classSectionId);
    }
  }

  listStudentByClassSection(classSectionId: string): void {
    this._apiService.getStudentsByClassSectionId(classSectionId).subscribe({
      next: response => {
        this.students = response.map((student: any) => ({
          ...student,
          fullName: `${student.firstName ?? ''} ${student.lastName ?? ''}`.trim()
        }));
      },
      error: err => console.log(err)
    });
  }

  onClassSectionChange(event: any): void {
    const classSectionId = event.value;
    if (classSectionId) {
      this.isClassSectionSelected = true;
      const selectedClassSection = this.sections.filter(
        x => x.classSectionId === classSectionId
      );

      if (selectedClassSection) {
        this.getStudentByClassSection(classSectionId);
      }
    }
  }

  getStudentByClassSection(classSectionId: string): void {
    this._apiService.getStudentsByClassSectionId(classSectionId).subscribe({
      next: response => {
        this.students = response.map((student: any) => ({
          ...student,
          fullName: `${student.firstName ?? ''} ${student.lastName ?? ''}`.trim()
        }));
      },
      error: err => console.log(err)
    });
  }

  onClassSectionChanged(event: any): void { }

  onExamTerminalChange(): void { }

  getClassRooms(): void {
    this._apiService.getClassRooms().subscribe({
      next: response => {
        this.classRooms = response;
      },
      error: err => console.log(err),
      complete: () => console.log('Request is complete')
    });
  }

  listSubject(classId: string): void {
    this._apiService.getClassCourseByClassId(classId).subscribe({
      next: response => {
        this.courses = response;
        this.createStudentMarksFormArray();
      },
      error: error => {
        console.log(error);
      }
    });
  }

  showDialog(student: StudentViewModel): void {
    this.studentName = student.firstName + ' ' + student.lastName;
    if (!this.getControl('examType')?.value) {
      this.isSubmitted = true;
      this.getControl('examType')?.markAsTouched();
      this._messageService.add({
        severity: 'warn',
        summary: 'Exam Terminal Required',
        detail: 'Please select exam terminal before entering marks.'
      });
      return;
    }

    if (student) {
      this.isSubmitted = false;
      this.submitButtonLabel = 'Save';
      this.isStudentMarksUpdate = false;

      const studentEnrollmentId = student.studentEnrollmentId;
      this.resetDialogFormOnly();
      this.getControl('studentId')?.setValue(studentEnrollmentId);
      this.isMarksEntryVisible = true;
      this.getStudentMarks(studentEnrollmentId);
    }
  }

  onDialogClose(): void {
    this.isStudentMarksUpdate = false;
    this.isMarksEntryVisible = false;
    this.submitButtonLabel = 'Save';
    this.resetDialogFormOnly();
  }

  getStudentMarks(studentEnrollmentId: string): void {
    const examType = this.getControl('examType')?.value;

    if (!examType) {
      return;
    }

    this._apiService.getStudentMarks(studentEnrollmentId, examType).subscribe({
      next: response => {
        if (response) {
          this.getControl('studentId')?.setValue(response.studentId);
          this.getControl('examType')?.setValue(response.examType);
          this.getControl('attendance')?.setValue(response.attendance);
          this.getControl('totalSchoolDays')?.setValue(response.totalSchoolDays);
          this.patchStudentMarksLists(response.studentMarksLists);
        }
      },
      error: error => {
        console.error(error);
      }
    });
  }

  patchStudentMarksLists(studentMarksLists: any[]): void {
    const marksArray = this.studentMarksLists;

    if (!studentMarksLists || !studentMarksLists.length) {
      return;
    }

    this.submitButtonLabel = 'Update';
    this.isStudentMarksUpdate = true;

    studentMarksLists.forEach(item => {
      const index = marksArray.controls.findIndex(
        control => control.get('classCourseId')?.value === item.classCourseId
      );

      if (index !== -1) {
        const group = marksArray.at(index) as FormGroup;

        group.patchValue({
          classCourseId: item.classCourseId,
          isTheoryRequired: item.isTheoryRequired,
          isPracticalRequired: item.isPracticalRequired,
          theoryCredit: item.theoryCredit,
          practicalCredit: item.practicalCredit,
          theoryFullMarks: item.theoryFullMarks,
          practicalFullMarks: item.practicalFullMarks,
          obtainedTheoryMarks: item.obtainedTheoryMarks,
          obtainedPracticalMarks: item.obtainedPracticalMarks
        });
      }
    });
  }

  save(): void {
    this.isSubmitted = true;
    this.studentMarks.markAllAsTouched();

    if (this.studentMarks.invalid) {
      const invalidCount = this.countInvalidControls(this.studentMarks);
      this.showValidationToast(invalidCount);
      this.scrollToFirstInvalidControl();
      return;
    }

    if (this.isStudentMarksUpdate) {
      this.updateStudentMarks();
    } else {
      this.saveStudentMarks();
    }
  }

  isTheoryDisabled(rowIndex: number): boolean {
    return !this.courses[rowIndex]?.isTheoryRequired;
  }

  isPracticalDisabled(rowIndex: number): boolean {
    return !this.courses[rowIndex]?.isPracticalRequired;
  }

  updateStudentMarks(): void {
    if (this.studentMarks.invalid) {
      return;
    }

    const studentMarks: SubjectMarkDto = this.studentMarks.value;

    this._apiService.updateStudentMarks(studentMarks).subscribe({
      next: response => {
        this._messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Successfully updated marks'
        });
        this.onDialogClose();
      },
      error: err => {
        this._messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to update marks'
        });
      }
    });
  }

  saveStudentMarks(): void {
    if (this.studentMarks.invalid) {
      return;
    }

    const studentMarks: SubjectMarkDto = this.studentMarks.value;

    this._apiService.postStudentMarks(studentMarks).subscribe({
      next: response => {
        this._messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Successfully added marks'
        });
        this.onDialogClose();
      },
      error: err => {
        this._messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to add marks'
        });
      }
    });
  }

  resetDialogFormOnly(): void {
    this.isSubmitted = false;

    const studentId = this.getControl('studentId')?.value;
    const examType = this.getControl('examType')?.value;

    this.studentMarks.reset({
      studentId: studentId ?? null,
      examType: examType ?? null,
      attendance: null,
      totalSchoolDays: null
    });

    this.studentMarksLists.clear();
    this.createStudentMarksFormArray();

    this.studentMarks.markAsPristine();
    this.studentMarks.markAsUntouched();
    this.studentMarks.updateValueAndValidity();
  }

  countInvalidControls(control: AbstractControl): number {
    let count = 0;

    if (control instanceof FormGroup) {
      Object.keys(control.controls).forEach(key => {
        count += this.countInvalidControls(control.controls[key]);
      });
    } else if (control instanceof FormArray) {
      control.controls.forEach(ctrl => {
        count += this.countInvalidControls(ctrl);
      });
    } else {
      if (control.invalid) {
        count++;
      }
    }

    return count;
  }

  showValidationToast(invalidCount: number): void {
    this._messageService.add({
      severity: 'error',
      summary: 'Validation Error',
      detail: `${invalidCount} field${invalidCount > 1 ? 's are' : ' is'} invalid. Please correct the highlighted fields.`
    });
  }

  scrollToFirstInvalidControl(): void {
    setTimeout(() => {
      const dialogContent = document.querySelector('.p-dialog-content') || document;

      const firstInvalidElement = dialogContent.querySelector(
        '.ng-invalid[formControlName], .p-dropdown.ng-invalid, input.ng-invalid, textarea.ng-invalid'
      ) as HTMLElement | null;

      if (firstInvalidElement) {
        firstInvalidElement.scrollIntoView({
          behavior: 'smooth',
          block: 'center'
        });

        firstInvalidElement.focus?.();
      }
    }, 100);
  }

  getClassName(classRoomId: string): string {
    let classRoom = 'Unknown';

    this._lookupService.getClassRooms().subscribe(classes => {
      classRoom = classes.find(s => s.id === classRoomId)?.name || 'Unknown';
      this.sections = classes.find(s => s.id === classRoomId)?.sections || [];
    });

    return classRoom;
  }

  getSectionName(sectionId: string): string {
    const section = this.sections.find(s => s.sectionId === sectionId);
    return section ? section.name : 'Unknown';
  }
}
