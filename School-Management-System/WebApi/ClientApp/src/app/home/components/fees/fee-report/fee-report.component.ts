import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { FilterSelection } from '../../student-filter/student-filter.component';
import { FeeReportViewModel } from '../model/feeReport.viewModel';
import { FeeReportRowViewModel } from '../model/feeReportRow.viewModel';
import { StudentFeeSummaryViewModel } from '../model/studentFeeSummary.viewModel';

@Component({
  selector: 'app-fee-report',
  standalone: false,
  templateUrl: './fee-report.component.html',
  styleUrl: './fee-report.component.scss'
})
export class FeeReportComponent implements OnInit {
  feeReport: FeeReportViewModel | null = null;
  hasLoadedReport: boolean = false;
  classSectionId: string = '';
  selectedStudentFeeSummary: StudentFeeSummaryViewModel | null = null;
  isDetailVisible: boolean = false;
  isDetailLoading: boolean = false;

  constructor(
    private _apiService: ApiService,
    private _messageService: MessageService
  ) { }

  ngOnInit(): void {
  }

  onLoadReport(filter: FilterSelection) {
    this.hasLoadedReport = false;
    this.feeReport = null;
    this.classSectionId = filter.classSectionId ?? '';
    this.closeDetail();

    if (!this.classSectionId) {
      return;
    }

    this._apiService.getFeeReport(this.classSectionId).subscribe({
      next: (response) => {
        this.feeReport = response;
        this.hasLoadedReport = true;
      },
      error: () => {
        this.hasLoadedReport = false;
        this._messageService.add({
          severity: 'error',
          summary: 'Report Load Failed',
          detail: 'Unable to load fee report for the selected class and section.'
        });
      }
    });
  }

  showDetail(student: FeeReportRowViewModel): void {
    if (!student.studentEnrollmentId || !this.classSectionId) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Detail Unavailable',
        detail: 'Load the report again before opening fee details.'
      });
      return;
    }

    this.isDetailVisible = true;
    this.isDetailLoading = true;
    this.selectedStudentFeeSummary = null;

    this._apiService.getStudentFeeSummary(student.studentEnrollmentId, this.classSectionId).subscribe({
      next: response => {
        this.selectedStudentFeeSummary = response;
        this.isDetailLoading = false;
      },
      error: () => {
        this.isDetailLoading = false;
        this._messageService.add({
          severity: 'error',
          summary: 'Detail Load Failed',
          detail: 'Unable to load detailed fee information for this student.'
        });
      }
    });
  }

  closeDetail(): void {
    this.isDetailVisible = false;
    this.isDetailLoading = false;
    this.selectedStudentFeeSummary = null;
  }
}
