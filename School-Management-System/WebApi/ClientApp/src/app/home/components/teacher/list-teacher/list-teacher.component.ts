import { Component, OnInit, ViewChild } from '@angular/core';
import { MenuItem, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { ApiService } from '../../../../shared/api.service';
import { AuthService } from '../../../../shared/auth.service';
import { AcademicViewModel } from '../../master-entry/model/viewModels/academicYear.ViewModel';
import { TeacherClassSectionViewModel, TeacherViewModel } from '../shared/models/viewModels/teacher.viewModel';

@Component({
  selector: 'app-list-teacher',
  standalone: false,
  templateUrl: './list-teacher.component.html',
  styleUrl: './list-teacher.component.scss'
})
export class ListTeacherComponent implements OnInit {
  @ViewChild('teacherTable') teacherTable?: Table;

  teachers: TeacherViewModel[] = [];
  filteredTeachers: TeacherViewModel[] = [];
  academicYears: AcademicViewModel[] = [];
  selectedAcademicYearId: string | null = null;
  includeInactive = false;
  searchText = '';
  isAddDialogVisible = false;
  isEditDialogVisible = false;
  isDetailDialogVisible = false;
  isAssignmentsDialogVisible = false;
  isDocumentsDialogVisible = false;
  isUserDialogVisible = false;
  selectedTeacher: TeacherViewModel | null = null;
  currentActionTeacher: TeacherViewModel | null = null;
  teacherActionItems: MenuItem[] = [];

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.loadAcademicYears();
  }

  loadAcademicYears(): void {
    this.apiService.getAcademicYear().subscribe({
      next: years => {
        this.academicYears = years;
        const sessionAcademicYearId = this.authService.getCurrentAcademicYearId();
        this.selectedAcademicYearId = years.find(x => x.id === sessionAcademicYearId)?.id
          ?? years.find(x => x.isActive)?.id
          ?? years[0]?.id
          ?? null;
        this.loadTeachers();
      },
      error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load academic years' })
    });
  }

  loadTeachers(): void {
    this.apiService.getTeachers(this.selectedAcademicYearId ?? undefined, this.includeInactive).subscribe({
      next: teachers => {
        this.teachers = teachers;
        this.applyFilters();
      },
      error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load teachers' })
    });
  }

  openAddTeacherDialog(): void {
    this.isAddDialogVisible = true;
  }

  openAssignments(teacher: TeacherViewModel): void {
    this.currentActionTeacher = teacher;
    this.isAssignmentsDialogVisible = true;
  }

  openDocuments(teacher: TeacherViewModel): void {
    this.currentActionTeacher = teacher;
    this.isDocumentsDialogVisible = true;
  }

  openTeacherUser(teacher: TeacherViewModel): void {
    this.currentActionTeacher = teacher;
    this.isUserDialogVisible = true;
  }

  editTeacher(teacher: TeacherViewModel): void {
    this.selectedTeacher = { ...teacher };
    this.isEditDialogVisible = true;
  }

  viewTeacher(teacher: TeacherViewModel): void {
    this.currentActionTeacher = teacher;
    this.isDetailDialogVisible = true;
  }

  toggleTeacherStatus(teacher: TeacherViewModel): void {
    const isActive = !teacher.isActive;
    this.apiService.updateTeacherStatus(teacher.id, {
      isActive,
      inactiveReason: isActive ? '' : 'Marked inactive from teacher list'
    }).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: `Teacher ${isActive ? 'activated' : 'deactivated'} successfully` });
        this.loadTeachers();
      },
      error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update teacher status' })
    });
  }

  onTeacherSaved(): void {
    this.messageService.add({
      severity: 'success',
      summary: 'Success',
      detail: this.isEditDialogVisible ? 'Teacher updated successfully' : 'Teacher added successfully'
    });
    this.isAddDialogVisible = false;
    this.isEditDialogVisible = false;
    this.selectedTeacher = null;
    this.loadTeachers();
  }

  closeActionDialog(dialogName: 'detail' | 'assignments' | 'documents' | 'user'): void {
    if (dialogName === 'detail') {
      this.isDetailDialogVisible = false;
    } else if (dialogName === 'assignments') {
      this.isAssignmentsDialogVisible = false;
    } else if (dialogName === 'documents') {
      this.isDocumentsDialogVisible = false;
    } else if (dialogName === 'user') {
      this.isUserDialogVisible = false;
    }
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  applyFilters(): void {
    const search = this.searchText.trim().toLowerCase();
    this.filteredTeachers = !search ? this.teachers : this.teachers.filter(teacher =>
      teacher.fullName?.toLowerCase().includes(search) ||
      teacher.employeeCode?.toLowerCase().includes(search) ||
      teacher.contactNumber?.toLowerCase().includes(search) ||
      teacher.email?.toLowerCase().includes(search) ||
      teacher.assignments?.some(assignment =>
        assignment.classRoomName?.toLowerCase().includes(search) ||
        assignment.sectionName?.toLowerCase().includes(search) ||
        assignment.courseName?.toLowerCase().includes(search)
      )
    );
  }

  exportTeachers(): void {
    this.teacherTable?.exportCSV();
  }

  openActionMenu(event: Event, menu: any, teacher: TeacherViewModel): void {
    this.currentActionTeacher = teacher;
    this.teacherActionItems = this.buildTeacherActionItems(teacher);
    menu.toggle(event);
  }

  private buildTeacherActionItems(teacher: TeacherViewModel): MenuItem[] {
    return [
      {
        label: 'Assignments',
        icon: 'pi pi-book',
        command: () => this.openAssignments(teacher)
      },
      {
        label: 'Documents',
        icon: 'pi pi-folder-open',
        command: () => this.openDocuments(teacher)
      },
      {
        label: 'User Access',
        icon: 'pi pi-user-edit',
        command: () => this.openTeacherUser(teacher)
      },
      {
        separator: true
      },
      {
        label: teacher.isActive ? 'Deactivate' : 'Activate',
        icon: teacher.isActive ? 'pi pi-ban' : 'pi pi-check',
        command: () => this.toggleTeacherStatus(teacher)
      }
    ];
  }

  getGender(genderId: number): string {
    switch (genderId) {
      case 1:
        return 'Male';
      case 2:
        return 'Female';
      case 3:
        return 'Other';
      default:
        return 'Unknown';
    }
  }

  getAssignmentSummary(teacher: TeacherViewModel): string {
    const assignments = teacher.assignments ?? [];
    if (!assignments.length) {
      return `Not assigned in ${this.getCurrentSessionYearLabel()}`;
    }

    return assignments
      .slice(0, 3)
      .map(x => `${x.classRoomName}-${x.sectionName}: ${x.courseName}${x.isClassTeacher ? ' (Class Teacher)' : ''}`)
      .join(', ');
  }

  getAssignmentPreview(teacher: TeacherViewModel): TeacherClassSectionViewModel[] {
    return (teacher.assignments ?? []).slice(0, 2);
  }

  getAssignmentOverflowCount(teacher: TeacherViewModel): number {
    return Math.max((teacher.assignments?.length ?? 0) - 2, 0);
  }

  getAssignmentChipLabel(assignment: TeacherClassSectionViewModel): string {
    return `${assignment.classRoomName}-${assignment.sectionName} · ${assignment.courseName}`;
  }
  getEmptyAssignmentLabel(): string {
    return `Not assigned in ${this.getCurrentSessionYearLabel()}`;
  }

  private getCurrentSessionYearLabel(): string {
    return this.academicYears.find(x => x.id === this.selectedAcademicYearId)?.yearName ?? 'current session';
  }
}
