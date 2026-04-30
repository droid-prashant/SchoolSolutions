import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { ExamTerminal } from '../../exam/shared/models/examTerminal.dto';
import { SubjectMarkDto } from '../../exam/shared/models/examMarksEntry.dto';
import { TeacherMarksAssignmentViewModel, TeacherSubjectStudentMarksViewModel } from '../shared/models/viewModels/teacher-marks.viewModel';

@Component({
  selector: 'app-teacher-marks-entry',
  standalone: false,
  templateUrl: './teacher-marks-entry.component.html',
  styleUrl: './teacher-marks-entry.component.scss'
})
export class TeacherMarksEntryComponent implements OnInit {
  examTerminals: ExamTerminal[] = [
    { id: 1, terminalName: 'First Terminal' },
    { id: 2, terminalName: 'Second Terminal' },
    { id: 3, terminalName: 'Third Terminal' },
    { id: 4, terminalName: 'Final Terminal' }
  ];
  assignments: TeacherMarksAssignmentViewModel[] = [];
  students: TeacherSubjectStudentMarksViewModel[] = [];
  selectedAssignment: TeacherMarksAssignmentViewModel | null = null;
  selectedStudent: TeacherSubjectStudentMarksViewModel | null = null;
  isMarksEntryVisible = false;
  isSubmitted = false;
  isLoading = false;
  marksForm: FormGroup;

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder,
    private messageService: MessageService
  ) {
    this.marksForm = this.fb.group({
      studentId: [null, Validators.required],
      examType: [null, Validators.required],
      attendance: [null, [Validators.required, Validators.min(0), Validators.max(365)]],
      totalSchoolDays: [null, [Validators.required, Validators.min(0), Validators.max(365)]],
      classCourseId: [null, Validators.required],
      obtainedTheoryMarks: [null],
      obtainedPracticalMarks: [null]
    });
  }

  ngOnInit(): void {
    this.loadAssignments();
  }

  getControl(controlName: string): AbstractControl | null {
    return this.marksForm.get(controlName);
  }

  isInvalid(controlName: string): boolean {
    const control = this.getControl(controlName);
    return !!(control && control.invalid && (control.touched || this.isSubmitted));
  }

  loadAssignments(): void {
    this.apiService.getTeacherMarksAssignments().subscribe({
      next: assignments => this.assignments = assignments,
      error: err => this.showError(err?.error?.message ?? 'Failed to load assigned subjects.')
    });
  }

  onCriteriaChange(): void {
    this.students = [];
    if (this.selectedAssignment && this.getControl('examType')?.value) {
      this.loadStudents();
    }
  }

  loadStudents(keyword = ''): void {
    if (!this.selectedAssignment) {
      return;
    }

    const examType = this.getControl('examType')?.value;
    if (!examType) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Exam Terminal Required',
        detail: 'Please select exam terminal first.'
      });
      return;
    }

    this.isLoading = true;
    this.apiService.getTeacherSubjectStudentMarks(
      this.selectedAssignment.classSectionId,
      this.selectedAssignment.classCourseId,
      examType,
      keyword
    ).subscribe({
      next: students => {
        this.students = students;
        this.isLoading = false;
      },
      error: err => {
        this.isLoading = false;
        this.showError(err?.error?.message ?? 'Failed to load students.');
      }
    });
  }

  showDialog(student: TeacherSubjectStudentMarksViewModel): void {
    if (!this.selectedAssignment) {
      return;
    }

    this.selectedStudent = student;
    this.isSubmitted = false;
    this.marksForm.reset({
      studentId: student.studentEnrollmentId,
      examType: this.getControl('examType')?.value,
      attendance: student.attendance,
      totalSchoolDays: student.totalSchoolDays,
      classCourseId: this.selectedAssignment.classCourseId,
      obtainedTheoryMarks: student.marks?.obtainedTheoryMarks ?? null,
      obtainedPracticalMarks: student.marks?.obtainedPracticalMarks ?? null
    });
    this.applyMarkValidators();
    this.isMarksEntryVisible = true;
  }

  save(): void {
    this.isSubmitted = true;
    this.marksForm.markAllAsTouched();
    if (this.marksForm.invalid || !this.selectedAssignment) {
      this.messageService.add({
        severity: 'error',
        summary: 'Validation Error',
        detail: 'Please correct the highlighted fields.'
      });
      return;
    }

    const value = this.marksForm.getRawValue();
    const payload: SubjectMarkDto = {
      studentId: value.studentId,
      examType: value.examType,
      attendance: value.attendance,
      totalSchoolDays: value.totalSchoolDays,
      studentMarksLists: [{
        classCourseId: value.classCourseId,
        isTheoryRequired: this.selectedAssignment.isTheoryRequired,
        isPracticalRequired: this.selectedAssignment.isPracticalRequired,
        theoryCredit: this.selectedAssignment.theoryCreditHour,
        practicalCredit: this.selectedAssignment.practicalCreditHour,
        theoryFullMarks: this.selectedAssignment.theoryFullMarks,
        practicalFullMarks: this.selectedAssignment.practicalFullMarks,
        obtainedTheoryMarks: this.selectedAssignment.isTheoryRequired ? value.obtainedTheoryMarks : null,
        obtainedPracticalMarks: this.selectedAssignment.isPracticalRequired ? value.obtainedPracticalMarks : null
      }]
    };

    this.apiService.upsertTeacherSubjectMarks(payload).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Marks saved successfully.'
        });
        this.closeDialog();
        this.loadStudents();
      },
      error: err => this.showError(err?.error?.message ?? 'Failed to save marks.')
    });
  }

  deleteMarks(student: TeacherSubjectStudentMarksViewModel): void {
    if (!this.selectedAssignment || !student.hasMarksEntry) {
      return;
    }

    const examType = this.getControl('examType')?.value;
    if (!window.confirm(`Delete ${this.selectedAssignment.courseName} marks for ${student.studentName}?`)) {
      return;
    }

    this.apiService.deleteTeacherSubjectMarks(student.studentEnrollmentId, examType, this.selectedAssignment.classCourseId).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Deleted',
          detail: 'Marks entry deleted successfully.'
        });
        this.loadStudents();
      },
      error: err => this.showError(err?.error?.message ?? 'Failed to delete marks.')
    });
  }

  closeDialog(): void {
    this.isMarksEntryVisible = false;
    this.selectedStudent = null;
    this.isSubmitted = false;
  }

  get assignmentLabel(): string {
    if (!this.selectedAssignment) {
      return 'Select an assigned subject';
    }

    return `${this.selectedAssignment.classRoomName} - ${this.selectedAssignment.sectionName} / ${this.selectedAssignment.courseName}`;
  }

  private applyMarkValidators(): void {
    const theory = this.getControl('obtainedTheoryMarks');
    const practical = this.getControl('obtainedPracticalMarks');
    const theoryValidators = this.selectedAssignment?.isTheoryRequired
      ? [Validators.required, Validators.min(0), Validators.max(this.selectedAssignment.theoryFullMarks ?? 0)]
      : [];
    const practicalValidators = this.selectedAssignment?.isPracticalRequired
      ? [Validators.required, Validators.min(0), Validators.max(this.selectedAssignment.practicalFullMarks ?? 0)]
      : [];

    theory?.setValidators(theoryValidators);
    practical?.setValidators(practicalValidators);
    theory?.updateValueAndValidity();
    practical?.updateValueAndValidity();
  }

  private showError(detail: string): void {
    this.messageService.add({
      severity: 'error',
      summary: 'Error',
      detail
    });
  }
}
