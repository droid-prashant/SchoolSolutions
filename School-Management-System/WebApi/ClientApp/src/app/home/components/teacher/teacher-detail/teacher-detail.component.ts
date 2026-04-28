import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { TeacherDocumentViewModel, TeacherViewModel } from '../shared/models/viewModels/teacher.viewModel';

@Component({
  selector: 'app-teacher-detail',
  standalone: false,
  templateUrl: './teacher-detail.component.html',
  styleUrl: './teacher-detail.component.scss'
})
export class TeacherDetailComponent implements OnInit {
  @Input() teacherId = '';
  @Input() embedded = false;
  teacher: TeacherViewModel | null = null;
  isLoading = false;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService,
    private messageService: MessageService
  ) { }

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

    this.loadTeacher();
  }

  loadTeacher(): void {
    this.isLoading = true;
    this.apiService.getTeacherById(this.teacherId).subscribe({
      next: teacher => {
        this.teacher = teacher;
        this.isLoading = false;
      },
      error: err => {
        this.isLoading = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Load Failed',
          detail: err?.error?.message ?? 'Failed to load teacher detail.'
        });
      }
    });
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

  printTeacherDetail(): void {
    if (!this.teacher) {
      return;
    }

    const teacher = this.teacher;
    const assignments = teacher.assignments?.length
      ? teacher.assignments.map(x => `<tr><td>${this.escapeHtml(x.academicYearName)}</td><td>${this.escapeHtml(x.classRoomName)}-${this.escapeHtml(x.sectionName)}</td><td>${this.escapeHtml(x.courseName)}</td><td>${x.isClassTeacher ? 'Yes' : 'No'}</td></tr>`).join('')
      : '<tr><td colspan="4">No assignments</td></tr>';
    const qualifications = teacher.qualifications?.length
      ? teacher.qualifications.map(x => `<tr><td>${this.escapeHtml(x.degreeName)}</td><td>${this.escapeHtml(x.institutionName)}</td><td>${this.escapeHtml(x.boardOrUniversity || '-')}</td><td>${this.escapeHtml(x.passedYear || '-')}</td></tr>`).join('')
      : '<tr><td colspan="4">No qualifications</td></tr>';
    const experiences = teacher.experiences?.length
      ? teacher.experiences.map(x => `<tr><td>${this.escapeHtml(x.organizationName)}</td><td>${this.escapeHtml(x.designation)}</td><td>${this.escapeHtml(x.startDateNp || '-')}</td><td>${this.escapeHtml(x.endDateNp || '-')}</td></tr>`).join('')
      : '<tr><td colspan="4">No experience records</td></tr>';
    const documents = teacher.documents?.length
      ? teacher.documents.map(x => `<tr><td>${this.escapeHtml(x.documentType)}</td><td>${this.escapeHtml(x.documentTitle)}</td><td>${this.escapeHtml(x.originalFileName)}</td><td>${this.escapeHtml(this.formatFileSize(x.fileSize))}</td></tr>`).join('')
      : '<tr><td colspan="4">No documents uploaded</td></tr>';

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
          <h2>Documents</h2><table><thead><tr><th>Type</th><th>Title</th><th>File</th><th>Size</th></tr></thead><tbody>${documents}</tbody></table>
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
