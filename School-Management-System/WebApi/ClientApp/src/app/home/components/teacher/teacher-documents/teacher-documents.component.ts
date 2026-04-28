import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { ApiService } from '../../../../shared/api.service';
import { TeacherDocumentViewModel, TeacherViewModel } from '../shared/models/viewModels/teacher.viewModel';

@Component({
  selector: 'app-teacher-documents',
  standalone: false,
  templateUrl: './teacher-documents.component.html',
  styleUrl: './teacher-documents.component.scss'
})
export class TeacherDocumentsComponent implements OnInit {
  @ViewChild('teacherDocumentTable') teacherDocumentTable?: Table;
  @ViewChild('documentFileInput') documentFileInput?: ElementRef<HTMLInputElement>;
  @Input() teacherId = '';
  @Input() embedded = false;

  teacher: TeacherViewModel | null = null;
  isLoading = false;
  isUploading = false;
  selectedDocument: File | null = null;
  documentType = '';
  documentTitle = '';
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
          detail: err?.error?.message ?? 'Failed to load teacher documents.'
        });
      }
    });
  }

  onDocumentSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedDocument = input.files?.[0] ?? null;
  }

  uploadDocument(): void {
    if (this.isUploading) {
      return;
    }

    if (!this.teacherId || !this.selectedDocument || !this.documentType.trim() || !this.documentTitle.trim()) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Missing Document Detail',
        detail: 'Choose the file, document type, and title before uploading.'
      });
      return;
    }

    const formData = new FormData();
    formData.append('teacherId', this.teacherId);
    formData.append('documentType', this.documentType);
    formData.append('documentTitle', this.documentTitle.trim());
    formData.append('file', this.selectedDocument);

    this.isUploading = true;
    this.apiService.uploadTeacherDocument(formData).subscribe({
      next: document => {
        if (this.teacher) {
          this.teacher.documents = [...(this.teacher.documents ?? []), document];
        }
        this.resetUploadForm();
        this.messageService.add({
          severity: 'success',
          summary: 'Document Uploaded',
          detail: 'Teacher document uploaded successfully.'
        });
      },
      error: err => {
        this.messageService.add({
          severity: 'error',
          summary: 'Upload Failed',
          detail: err?.error?.message ?? 'Failed to upload teacher document.'
        });
      },
      complete: () => this.isUploading = false
    });
  }

  deleteDocument(documentId: string): void {
    this.apiService.deleteTeacherDocument(documentId).subscribe({
      next: () => {
        if (this.teacher) {
          this.teacher.documents = this.teacher.documents.filter(x => x.id !== documentId);
        }
        this.messageService.add({
          severity: 'success',
          summary: 'Document Removed',
          detail: 'Teacher document removed successfully.'
        });
      },
      error: err => {
        this.messageService.add({
          severity: 'error',
          summary: 'Delete Failed',
          detail: err?.error?.message ?? 'Failed to remove teacher document.'
        });
      }
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

  get teacherDisplayName(): string {
    if (!this.teacher) {
      return 'Teacher Documents';
    }

    return `${this.teacher.fullName} · ${this.teacher.employeeCode || 'No code'}`;
  }

  private resetUploadForm(): void {
    this.documentType = '';
    this.documentTitle = '';
    this.selectedDocument = null;
    if (this.documentFileInput?.nativeElement) {
      this.documentFileInput.nativeElement.value = '';
    }
  }
}
