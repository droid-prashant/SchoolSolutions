import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { AuthService } from '../../../../shared/auth.service';
import { LookupService } from '../../../../shared/common/lookup.service';
import { AcademicViewModel } from '../../master-entry/model/viewModels/academicYear.ViewModel';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { CourseViewModel } from '../../course/shared/models/course.viewModel';
import { TeacherAssignmentCopyDto, TeacherClassSectionDto } from '../shared/models/dtos/teacher.dto';
import { TeacherClassSectionViewModel, TeacherViewModel } from '../shared/models/viewModels/teacher.viewModel';

@Component({
  selector: 'app-teacher-assignments',
  standalone: false,
  templateUrl: './teacher-assignments.component.html',
  styleUrl: './teacher-assignments.component.scss'
})
export class TeacherAssignmentsComponent implements OnInit {
  @Input() teacherId = '';
  @Input() embedded = false;
  teacher: TeacherViewModel | null = null;
  assignments: TeacherClassSectionViewModel[] = [];
  previousYearAssignments: TeacherClassSectionViewModel[] = [];
  academicYears: AcademicViewModel[] = [];
  classRooms: ClassRoomViewModel[] = [];
  courses: CourseViewModel[] = [];
  selectedAcademicYearId: string | null = null;
  currentAcademicYearName = '';
  previousAcademicYearId: string | null = null;
  previousAcademicYearName = '';
  selectedSections: SectionViewModel[] = [];
  assignmentForm: FormGroup;
  isLoading = false;
  isSaving = false;
  isCopying = false;
  isEditMode = false;
  editingAssignmentId: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService,
    private authService: AuthService,
    private lookupService: LookupService,
    private messageService: MessageService,
    private fb: FormBuilder
  ) {
    this.assignmentForm = this.fb.group({
      classRoomId: [null, Validators.required],
      sectionId: [null, Validators.required],
      classSectionId: [null, Validators.required],
      courseId: [null, Validators.required],
      isClassTeacher: [false],
      remarks: ['']
    });
  }

  ngOnInit(): void {
    const routeTeacherId = this.route.snapshot.paramMap.get('teacherId') ?? '';
    this.teacherId = this.teacherId || routeTeacherId;
    if (!this.teacherId) {
      this.messageService.add({
        severity: 'error',
        summary: 'Teacher Not Found',
        detail: 'Teacher id is missing in the route.'
      });
      return;
    }

    this.selectedAcademicYearId = this.authService.getCurrentAcademicYearId();
    if (!this.selectedAcademicYearId) {
      this.messageService.add({
        severity: 'error',
        summary: 'Academic Year Missing',
        detail: 'Current session academic year is not available.'
      });
      return;
    }

    this.loadLookups();
    this.loadTeacher();
    this.assignmentForm.get('classRoomId')?.valueChanges.subscribe(classRoomId => this.onClassRoomChange(classRoomId));
    this.assignmentForm.get('sectionId')?.valueChanges.subscribe(sectionId => this.onSectionChange(sectionId));
  }

  loadLookups(): void {
    this.lookupService.getAcademicYears().subscribe(years => {
      this.academicYears = years;
      this.currentAcademicYearName = years.find(x => x.id === this.selectedAcademicYearId)?.yearName ?? 'Current Session';
      this.previousAcademicYearId = this.resolvePreviousAcademicYearId(years, this.selectedAcademicYearId);
      this.previousAcademicYearName = years.find(x => x.id === this.previousAcademicYearId)?.yearName ?? '';
      this.loadPreviousYearAssignments();
    });
    this.lookupService.getClassRooms().subscribe(classes => this.classRooms = classes);
    this.lookupService.getCourses().subscribe(courses => this.courses = courses);
    this.loadAssignments();
  }

  loadTeacher(): void {
    this.apiService.getTeacherById(this.teacherId).subscribe({
      next: teacher => this.teacher = teacher,
      error: err => {
        this.messageService.add({
          severity: 'error',
          summary: 'Load Failed',
          detail: err?.error?.message ?? 'Failed to load teacher profile.'
        });
      }
    });
  }

  loadAssignments(): void {
    if (!this.teacherId) {
      return;
    }

    this.isLoading = true;
    this.apiService.getTeacherAssignments(this.teacherId, this.selectedAcademicYearId).subscribe({
      next: assignments => {
        this.assignments = assignments;
        this.isLoading = false;
      },
      error: err => {
        this.isLoading = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Load Failed',
          detail: err?.error?.message ?? 'Failed to load teacher assignments.'
        });
      }
    });
  }

  loadPreviousYearAssignments(): void {
    if (!this.teacherId || !this.previousAcademicYearId) {
      this.previousYearAssignments = [];
      return;
    }

    this.apiService.getTeacherAssignments(this.teacherId, this.previousAcademicYearId).subscribe({
      next: assignments => {
        this.previousYearAssignments = assignments.filter(x => x.isActive);
        this.loadTeacher();
      },
      error: () => {
        this.previousYearAssignments = [];
      }
    });
  }

  editAssignment(assignment: TeacherClassSectionViewModel): void {
    this.isEditMode = true;
    this.editingAssignmentId = assignment.id;
    const selectedClass = this.classRooms.find(x => x.id === assignment.classRoomId);
    this.selectedSections = selectedClass?.sections ?? [];
    this.assignmentForm.patchValue({
      classRoomId: assignment.classRoomId,
      sectionId: assignment.sectionId,
      classSectionId: assignment.classSectionId,
      courseId: assignment.courseId,
      isClassTeacher: assignment.isClassTeacher,
      remarks: assignment.remarks ?? ''
    }, { emitEvent: false });
  }

  saveAssignment(): void {
    if (this.assignmentForm.invalid || this.isSaving) {
      this.assignmentForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const assignment = this.prepareAssignmentDto();
    const request = this.isEditMode
      ? this.apiService.updateTeacherAssignment(this.teacherId, assignment)
      : this.apiService.addTeacherAssignment(this.teacherId, assignment);

    request.subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: this.isEditMode ? 'Teacher assignment updated successfully.' : 'Teacher assignment added successfully.'
        });
        this.resetAssignmentForm();
        this.loadAssignments();
        this.loadTeacher();
      },
      error: err => {
        this.messageService.add({
          severity: 'error',
          summary: 'Save Failed',
          detail: err?.error?.message ?? 'Failed to save teacher assignment.'
        });
      },
      complete: () => this.isSaving = false
    });
  }

  deleteAssignment(assignment: TeacherClassSectionViewModel): void {
    this.apiService.deleteTeacherAssignment(assignment.id).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Assignment Removed',
          detail: 'Teacher assignment removed successfully.'
        });
        if (this.editingAssignmentId === assignment.id) {
          this.resetAssignmentForm();
        }
        this.loadAssignments();
        this.loadTeacher();
      },
      error: err => {
        this.messageService.add({
          severity: 'error',
          summary: 'Delete Failed',
          detail: err?.error?.message ?? 'Failed to remove teacher assignment.'
        });
      }
    });
  }

  cancelEdit(): void {
    this.resetAssignmentForm();
  }

  copyPreviousYearAssignments(): void {
    if (!this.teacherId || !this.previousAcademicYearId || !this.selectedAcademicYearId || this.isCopying) {
      return;
    }

    const payload: TeacherAssignmentCopyDto = {
      sourceAcademicYearId: this.previousAcademicYearId,
      targetAcademicYearId: this.selectedAcademicYearId
    };

    this.isCopying = true;
    this.apiService.copyTeacherAssignments(this.teacherId, payload).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Assignments Copied',
          detail: `Assignments from ${this.previousAcademicYearName} were copied to ${this.currentAcademicYearName}.`
        });
        this.loadAssignments();
        this.loadTeacher();
      },
      error: err => {
        this.messageService.add({
          severity: 'error',
          summary: 'Copy Failed',
          detail: err?.error?.message ?? 'Failed to copy previous year assignments.'
        });
      },
      complete: () => this.isCopying = false
    });
  }

  get teacherDisplayName(): string {
    if (!this.teacher) {
      return 'Teacher Assignments';
    }

    return `${this.teacher.fullName} · ${this.teacher.employeeCode || 'No code'}`;
  }

  private onClassRoomChange(classRoomId: string | null): void {
    const selectedClass = this.classRooms.find(x => x.id === classRoomId);
    this.selectedSections = selectedClass?.sections ?? [];
    this.assignmentForm.patchValue({
      sectionId: null,
      classSectionId: null
    }, { emitEvent: false });
  }

  private onSectionChange(sectionId: string | null): void {
    const selectedSection = this.selectedSections.find(x => x.sectionId === sectionId);
    this.assignmentForm.patchValue({
      classSectionId: selectedSection?.classSectionId ?? null
    }, { emitEvent: false });
  }

  private prepareAssignmentDto(): TeacherClassSectionDto {
    const value = this.assignmentForm.value;
    return {
      id: this.editingAssignmentId ?? undefined,
      academicYearId: this.selectedAcademicYearId ?? '',
      classSectionId: value.classSectionId,
      courseId: value.courseId,
      isClassTeacher: value.isClassTeacher,
      remarks: value.remarks ?? ''
    };
  }

  private resetAssignmentForm(): void {
    this.isEditMode = false;
    this.editingAssignmentId = null;
    this.selectedSections = [];
    this.assignmentForm.reset({
      classRoomId: null,
      sectionId: null,
      classSectionId: null,
      courseId: null,
      isClassTeacher: false,
      remarks: ''
    });
  }

  private resolvePreviousAcademicYearId(years: AcademicViewModel[], currentAcademicYearId: string | null): string | null {
    if (!currentAcademicYearId || years.length === 0) {
      return null;
    }

    const orderedYears = [...years].sort((left, right) => this.compareAcademicYears(left.yearName, right.yearName));
    const currentIndex = orderedYears.findIndex(x => x.id === currentAcademicYearId);
    if (currentIndex <= 0) {
      return null;
    }

    return orderedYears[currentIndex - 1]?.id ?? null;
  }

  private compareAcademicYears(left: string, right: string): number {
    const leftNumber = Number(left);
    const rightNumber = Number(right);

    if (!Number.isNaN(leftNumber) && !Number.isNaN(rightNumber)) {
      return leftNumber - rightNumber;
    }

    return left.localeCompare(right);
  }
}
