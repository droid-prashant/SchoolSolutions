import { Component, OnInit, ViewChild } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { ApiService } from '../../../../shared/api.service';
import { AcademicViewModel } from '../../master-entry/model/viewModels/academicYear.ViewModel';
import { TeacherViewModel } from '../shared/models/viewModels/teacher.viewModel';

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
  isViewDialogVisible = false;
  selectedTeacher: TeacherViewModel | null = null;
  detailTeacher: TeacherViewModel | null = null;

  constructor(private apiService: ApiService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.loadAcademicYears();
  }

  loadAcademicYears(): void {
    this.apiService.getAcademicYear().subscribe({
      next: years => {
        this.academicYears = years;
        this.selectedAcademicYearId = years.find(x => x.isActive)?.id ?? years[0]?.id ?? null;
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

  editTeacher(teacher: TeacherViewModel): void {
    this.selectedTeacher = { ...teacher };
    this.isEditDialogVisible = true;
  }

  viewTeacher(teacher: TeacherViewModel): void {
    this.detailTeacher = teacher;
    this.isViewDialogVisible = true;
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
    this.detailTeacher = null;
    this.loadTeachers();
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
      return 'Not assigned';
    }

    return assignments
      .slice(0, 3)
      .map(x => `${x.classRoomName}-${x.sectionName}: ${x.courseName}${x.isClassTeacher ? ' (Class Teacher)' : ''}`)
      .join(', ');
  }

  getAddressSummary(teacher: TeacherViewModel | null): string {
    if (!teacher) {
      return '-';
    }

    const parts = [
      teacher.provinceId ? `Province ${teacher.provinceId}` : '',
      teacher.districtId ? `District ${teacher.districtId}` : '',
      teacher.municipalityId ? `Municipality ${teacher.municipalityId}` : '',
      teacher.wardNo ? `Ward ${teacher.wardNo}` : ''
    ].filter(Boolean);

    return parts.length ? parts.join(', ') : '-';
  }

  printTeacherDetail(): void {
    if (!this.detailTeacher) {
      return;
    }

    const teacher = this.detailTeacher;
    const assignments = teacher.assignments?.length
      ? teacher.assignments.map(x => `<tr><td>${this.escapeHtml(x.academicYearName)}</td><td>${this.escapeHtml(x.classRoomName)}-${this.escapeHtml(x.sectionName)}</td><td>${this.escapeHtml(x.courseName)}</td><td>${x.isClassTeacher ? 'Yes' : 'No'}</td></tr>`).join('')
      : '<tr><td colspan="4">No assignments</td></tr>';
    const qualifications = teacher.qualifications?.length
      ? teacher.qualifications.map(x => `<tr><td>${this.escapeHtml(x.degreeName)}</td><td>${this.escapeHtml(x.institutionName)}</td><td>${this.escapeHtml(x.boardOrUniversity || '-')}</td><td>${this.escapeHtml(x.passedYear || '-')}</td></tr>`).join('')
      : '<tr><td colspan="4">No qualifications</td></tr>';
    const experiences = teacher.experiences?.length
      ? teacher.experiences.map(x => `<tr><td>${this.escapeHtml(x.organizationName)}</td><td>${this.escapeHtml(x.designation)}</td><td>${this.escapeHtml(x.startDateNp || '-')}</td><td>${this.escapeHtml(x.endDateNp || '-')}</td></tr>`).join('')
      : '<tr><td colspan="4">No experience records</td></tr>';

    const printWindow = window.open('', '_blank', 'width=900,height=700');
    if (!printWindow) {
      return;
    }

    printWindow.document.write(`
      <html>
        <head>
          <title>Teacher Detail - ${this.escapeHtml(teacher.fullName)}</title>
          <style>
            body { font-family: Arial, sans-serif; color: #1f2937; padding: 24px; }
            h1 { margin: 0 0 4px; font-size: 24px; }
            h2 { margin: 24px 0 10px; font-size: 16px; color: #17324d; }
            .muted { color: #64748b; margin-bottom: 18px; }
            .grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 10px 24px; }
            .item span { display: block; color: #64748b; font-size: 12px; }
            .item strong { font-size: 14px; }
            table { width: 100%; border-collapse: collapse; margin-top: 8px; }
            th, td { border: 1px solid #dbe5f0; padding: 8px; text-align: left; font-size: 13px; }
            th { background: #f1f6fd; }
          </style>
        </head>
        <body>
          <h1>${this.escapeHtml(teacher.fullName)}</h1>
          <div class="muted">${this.escapeHtml(teacher.employeeCode || '-')} | ${this.escapeHtml(teacher.designation || 'Teacher')}</div>
          <div class="grid">
            <div class="item"><span>Contact</span><strong>${this.escapeHtml(teacher.contactNumber || '-')}</strong></div>
            <div class="item"><span>Email</span><strong>${this.escapeHtml(teacher.email || '-')}</strong></div>
            <div class="item"><span>Gender</span><strong>${this.getGender(teacher.gender)}</strong></div>
            <div class="item"><span>Status</span><strong>${teacher.isActive ? 'Active' : 'Inactive'}</strong></div>
            <div class="item"><span>Date of Birth</span><strong>${this.escapeHtml(teacher.dateOfBirthNp || '-')}</strong></div>
            <div class="item"><span>Joining Date</span><strong>${this.escapeHtml(teacher.joiningDateNp || '-')}</strong></div>
            <div class="item"><span>Address</span><strong>${this.escapeHtml(this.getAddressSummary(teacher))}</strong></div>
            <div class="item"><span>Login User</span><strong>${teacher.userId ? 'Created' : 'Not created'}</strong></div>
          </div>
          <h2>Assignments</h2><table><thead><tr><th>Academic Year</th><th>Class / Section</th><th>Course</th><th>Class Teacher</th></tr></thead><tbody>${assignments}</tbody></table>
          <h2>Qualifications</h2><table><thead><tr><th>Degree</th><th>Institution</th><th>Board/University</th><th>Passed Year</th></tr></thead><tbody>${qualifications}</tbody></table>
          <h2>Experience</h2><table><thead><tr><th>Organization</th><th>Designation</th><th>Start Date</th><th>End Date</th></tr></thead><tbody>${experiences}</tbody></table>
        </body>
      </html>
    `);
    printWindow.document.close();
    printWindow.focus();
    printWindow.print();
  }

  private escapeHtml(value: string): string {
    const entities: Record<string, string> = {
      '&': '&amp;',
      '<': '&lt;',
      '>': '&gt;',
      '"': '&quot;',
      "'": '&#039;'
    };

    return value.replace(/[&<>"']/g, char => entities[char] ?? char);
  }
}
