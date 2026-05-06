import { Component, OnInit } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { StudentViewModel } from '../shared/models/viewModels/student.viewModel';
import { FilterSelection } from '../../student-filter/student-filter.component';
import { LookupService } from '../../../../shared/common/lookup.service';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';

@Component({
  selector: 'app-list-student',
  standalone: false,
  templateUrl: './list-student.component.html',
  styleUrl: './list-student.component.scss'
})
export class ListStudentComponent implements OnInit {
  students: StudentViewModel[] = [];
  filteredStudents: StudentViewModel[] = [];
  sections: SectionViewModel[] = [];
  isAddDialogVisible: boolean = false;
  isEditDialogVisible: boolean = false;
  selectedStudent: StudentViewModel | null = null;
  currentFilter: FilterSelection = {};
  searchText: string = '';
  studentView: 'active' | 'dropped' = 'active';
  isLoadingStudents: boolean = false;

  constructor(
    private _apiService: ApiService,
    private _messageService: MessageService,
    private _lookupService: LookupService,
    private _confirmationService: ConfirmationService
  ) {
  }
  ngOnInit(): void {
  }

  editStudent(student: StudentViewModel) {
    this.selectedStudent = { ...student };
    this.isEditDialogVisible = true;
  }

  openAddStudentDialog() {
    this.isAddDialogVisible = true;
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

  onSearchChange() {
    this.applyFilters();
  }

  applyFilters() {
    let filtered = this.students;

    // Apply filter criteria
    if (this.currentFilter.classId) {
      filtered = filtered.filter(s => s.classRoomId === this.currentFilter.classId);
    }
    if (this.currentFilter.sectionId) {
      filtered = filtered.filter(s => s.sectionId === this.currentFilter.sectionId);
    }

    // Apply search text
    if (this.searchText.trim()) {
      const searchLower = this.searchText.toLowerCase();
      filtered = filtered.filter(student =>
        student.firstName?.toLowerCase().includes(searchLower) ||
        student.lastName?.toLowerCase().includes(searchLower) ||
        student.classRoomName?.toLowerCase().includes(searchLower) ||
        student.sectionName?.toLowerCase().includes(searchLower) ||
        student.address?.toLowerCase().includes(searchLower) ||
        student.fatherName?.toLowerCase().includes(searchLower) ||
        student.motherName?.toLowerCase().includes(searchLower)
      );
    }

    this.filteredStudents = filtered;
  }

  onLoadStudents(filter: FilterSelection) {
    this.currentFilter = filter;
    this.loadCurrentStudents();
  }

  setStudentView(view: 'active' | 'dropped') {
    if (this.studentView === view) {
      return;
    }

    this.studentView = view;
    this.searchText = '';
    this.loadCurrentStudents();
  }

  loadCurrentStudents() {
    if (this.currentFilter && this.currentFilter.classSectionId) {
      this.listStudentByClassSection(this.currentFilter.classSectionId);
    }
    else if (this.currentFilter && this.currentFilter.classId) {
      this.listStudentByClass(this.currentFilter.classId);
    }
    else {
      this.listStudent();
    }
  }

  private get isActiveView(): boolean {
    return this.studentView === 'active';
  }

  listStudent() {
    this.isLoadingStudents = true;
    this._apiService.getStudents(this.isActiveView).subscribe(
      {
        next: (response) => {
          this.students = response;
          this.applyFilters();
        },
        error: () => this.showLoadError(),
        complete: () => this.isLoadingStudents = false
      }
    )
  }

  listStudentByClassSection(classSectionId: string) {
    this.isLoadingStudents = true;
    this._apiService.getStudentsByClassSectionId(classSectionId, null, this.isActiveView).subscribe(
      {
        next: (response) => {
          this.students = response;
          this.applyFilters();
        },
        error: () => this.showLoadError(),
        complete: () => this.isLoadingStudents = false
      }
    );
  }

  listStudentByClass(classId: string) {
    this.isLoadingStudents = true;
    this._apiService.getStudentsByClass(classId, this.isActiveView).subscribe(
      {
        next: (response) => {
          this.students = response;
          this.applyFilters();
        },
        error: () => this.showLoadError(),
        complete: () => this.isLoadingStudents = false
      }
    );
  }

  updateStudentStatus(student: StudentViewModel, isActive: boolean) {
    const action = isActive ? 'revive' : 'drop';
    const studentName = `${student.firstName} ${student.lastName}`.trim();
    this._confirmationService.confirm({
      message: `Are you sure you want to ${action} ${studentName}?`,
      header: isActive ? 'Revive Student' : 'Drop Student',
      icon: isActive ? 'pi pi-check-circle' : 'pi pi-exclamation-triangle',
      acceptLabel: isActive ? 'Revive' : 'Drop',
      rejectLabel: 'Cancel',
      accept: () => {
        this._apiService.updateStudentEnrollmentStatus(student.studentEnrollmentId, { isActive }).subscribe({
          next: () => {
            this._messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: `Student ${isActive ? 'revived' : 'dropped'} successfully`
            });
            this.loadCurrentStudents();
          },
          error: () => this._messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: `Failed to ${action} student`
          })
        });
      }
    });
  }

  getViewTitle(): string {
    return this.studentView === 'active' ? 'Active Students' : 'Dropped Students';
  }

  getEmptyMessage(): string {
    return this.studentView === 'active' ? 'No active students found' : 'No dropped students found';
  }

  displayClassName(student: StudentViewModel): string {
    return student.classRoomName || this.getClassName(student.classRoomId);
  }

  displaySectionName(student: StudentViewModel): string {
    return student.sectionName || this.getSectionName(student.sectionId);
  }

  private showLoadError() {
    this.isLoadingStudents = false;
    this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load students' });
  }

  getClassName(classRoomId: string): string {
    let classRoom: string = 'Unknown';
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

  onStudentSaved() {
    if (this.isEditDialogVisible) {
      this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Student updated successfully' });
    } else if (this.isAddDialogVisible) {
      this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Student added successfully' });
    }
    this.onLoadStudents(this.currentFilter);
    this.isEditDialogVisible = false;
    this.isAddDialogVisible = false;
    this.selectedStudent = null;
  }
}
