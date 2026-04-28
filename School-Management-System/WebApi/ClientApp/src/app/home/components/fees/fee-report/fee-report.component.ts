import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { AuthService } from '../../../../shared/auth.service';
import { ApiService } from '../../../../shared/api.service';
import { LookupService } from '../../../../shared/common/lookup.service';
import { FilterSelection } from '../../student-filter/student-filter.component';
import { FeeReportViewModel } from '../model/feeReport.viewModel';

@Component({
  selector: 'app-fee-report',
  standalone: false,
  templateUrl: './fee-report.component.html',
  styleUrl: './fee-report.component.scss'
})
export class FeeReportComponent implements OnInit {
  currentAcademicYearName: string = '';
  feeReport: FeeReportViewModel | null = null;
  hasLoadedReport: boolean = false;

  constructor(
    private _apiService: ApiService,
    private _authService: AuthService,
    private _lookupService: LookupService,
    private _messageService: MessageService
  ) { }

  ngOnInit(): void {
    const academicYearId = this._authService.getCurrentAcademicYearId();
    if (academicYearId) {
      this._lookupService.getAcademicYearById(academicYearId).subscribe({
        next: (academicYear) => {
          this.currentAcademicYearName = academicYear?.yearName ?? '';
        }
      });
    }
  }

  onLoadReport(filter: FilterSelection) {
    this.hasLoadedReport = false;
    this.feeReport = null;

    if (!filter.classSectionId) {
      return;
    }

    this._apiService.getFeeReport(filter.classSectionId).subscribe({
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
}
