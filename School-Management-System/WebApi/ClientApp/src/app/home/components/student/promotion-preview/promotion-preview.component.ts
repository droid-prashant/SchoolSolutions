import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { FilterSelection } from '../../student-filter/student-filter.component';
import { PromotionCandidateViewModel } from '../shared/models/viewModels/promotionCandidate.viewModel';
import { LookupService } from '../../../../shared/common/lookup.service';
import { AuthService } from '../../../../shared/auth.service';
import { AcademicViewModel } from '../../master-entry/model/viewModels/academicYear.ViewModel';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { PromotionExecutionResultViewModel } from '../shared/models/viewModels/promotionExecutionResult.viewModel';

@Component({
  selector: 'app-promotion-preview',
  standalone: false,
  templateUrl: './promotion-preview.component.html',
  styleUrl: './promotion-preview.component.scss'
})
export class PromotionPreviewComponent implements OnInit {
  candidates: PromotionCandidateViewModel[] = [];
  selectedCandidates: PromotionCandidateViewModel[] = [];
  hasLoadedCandidates: boolean = false;
  isPromoting: boolean = false;
  visibilityFilter: 'all' | 'promotable' | 'notPromotable' = 'all';
  currentAcademicYearId: string = '';
  currentAcademicYearName: string = '';
  targetAcademicYearId: string | null = null;
  targetAcademicYearOptions: AcademicViewModel[] = [];
  classOptions: ClassRoomViewModel[] = [];
  sourceSectionOptions: SectionViewModel[] = [];
  targetSectionOptions: SectionViewModel[] = [];
  selectedSourceClassId: string | null = null;
  selectedSourceClassSectionId: string | null = null;
  selectedExamType: number = 4;
  manualTargetClassId: string | null = null;
  manualTargetClassSectionId: string | null = null;
  lastFilter: FilterSelection | null = null;
  tableFirst: number = 0;

  constructor(
    private _apiService: ApiService,
    private _lookupService: LookupService,
    private _authService: AuthService,
    private _messageService: MessageService,
    private _confirmationService: ConfirmationService
  ) { }

  ngOnInit(): void {
    this.currentAcademicYearId = this._authService.getCurrentAcademicYearId();
    this.loadAcademicYears();
    this.loadClasses();
  }

  loadCandidates(): void {
    if (!this.selectedSourceClassSectionId) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Filters Required',
        detail: 'Select class and section to load promotion candidates.'
      });
      return;
    }

    this.onLoadCandidates({
      classId: this.selectedSourceClassId ?? undefined,
      classSectionId: this.selectedSourceClassSectionId,
      examType: this.selectedExamType
    });
  }

  onLoadCandidates(filter: FilterSelection, preserveSelection: boolean = false) {
    const selectedEnrollmentIds = preserveSelection
      ? new Set(this.selectedCandidates.map(x => x.studentEnrollmentId))
      : new Set<string>();

    this.lastFilter = filter;
    this.candidates = [];
    this.selectedCandidates = [];
    this.hasLoadedCandidates = false;
    this.visibilityFilter = 'all';
    this.tableFirst = 0;
    this.resetManualTarget();

    if (!filter.classSectionId || !filter.examType) {
      return;
    }

    this._apiService.getPromotionCandidates(filter.classSectionId, filter.examType, this.targetAcademicYearId).subscribe({
      next: (response) => {
        this.candidates = response;
        this.restoreSelectedCandidates(selectedEnrollmentIds);
        this.hasLoadedCandidates = true;
      },
      error: (err) => {
        this.hasLoadedCandidates = true;
        this._messageService.add({
          severity: 'error',
          summary: 'Preview Load Failed',
          detail: err?.error?.message ?? 'Failed to load promotion candidates.'
        });
      }
    });
  }

  promoteSelected(): void {
    if (this.selectedCandidates.length === 0) {
      this._messageService.add({
        severity: 'warn',
        summary: 'No Students Selected',
        detail: 'Select at least one student to continue.'
      });
      return;
    }

    if (this.manualTargetClassSectionId) {
      this.executeManualPromotion();
      return;
    }

    if (this.selectedCandidates.some(x => x.isAlreadyPromoted)) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Invalid Selection',
        detail: 'Selected student already exists in the target academic year.'
      });
      return;
    }

    if (this.selectedCandidates.some(x => !x.isPromotable)) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Invalid Selection',
        detail: 'Normal promotion accepts only promotable students.'
      });
      return;
    }

    this.executePromotion(false);
  }

  get promotableCandidates(): PromotionCandidateViewModel[] {
    return this.candidates.filter(x => x.isPromotable && !x.isAlreadyPromoted);
  }

  get notPromotableCandidates(): PromotionCandidateViewModel[] {
    return this.candidates.filter(x => !x.isPromotable && !x.isAlreadyPromoted);
  }

  get promotedCandidates(): PromotionCandidateViewModel[] {
    return this.candidates.filter(x => x.isAlreadyPromoted);
  }

  get visibleCandidates(): PromotionCandidateViewModel[] {
    switch (this.visibilityFilter) {
      case 'promotable':
        return this.promotableCandidates;
      case 'notPromotable':
        return this.notPromotableCandidates;
      default:
        return this.candidates;
    }
  }

  setVisibilityFilter(filter: 'all' | 'promotable' | 'notPromotable'): void {
    this.visibilityFilter = filter;
    this.tableFirst = 0;
    const visibleIds = new Set(this.visibleCandidates.map(x => x.studentEnrollmentId));
    this.selectedCandidates = this.selectedCandidates.filter(x => visibleIds.has(x.studentEnrollmentId) && !x.isAlreadyPromoted);
  }

  onTablePage(event: { first?: number }): void {
    this.tableFirst = event.first ?? 0;
  }

  onSelectionChange(selection: PromotionCandidateViewModel[] | PromotionCandidateViewModel | null): void {
    const selectedCandidates = Array.isArray(selection) ? selection : selection ? [selection] : [];
    this.selectedCandidates = this.getSelectableCandidates(selectedCandidates);
  }

  sustainSelected(): void {
    if (this.selectedCandidates.length === 0) {
      this._messageService.add({
        severity: 'warn',
        summary: 'No Students Selected',
        detail: 'Select at least one non-promotable student to sustain.'
      });
      return;
    }

    if (this.selectedCandidates.some(x => x.isPromotable)) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Invalid Selection',
        detail: 'Sustain is meant for non-promotable students only.'
      });
      return;
    }

    this.executeSustain(false);
  }

  onManualTargetClassChange(classId: string | null): void {
    this.manualTargetClassId = classId;
    this.manualTargetClassSectionId = null;

    const selectedClass = this.classOptions.find(x => x.id === classId);
    this.targetSectionOptions = selectedClass?.sections ?? [];
  }

  onTargetAcademicYearChange(): void {
    if (this.hasLoadedCandidates && this.lastFilter?.classSectionId && this.lastFilter.examType) {
      this.onLoadCandidates(this.lastFilter, true);
    }
  }

  onSourceClassChange(classId: string | null): void {
    this.selectedSourceClassId = classId;
    this.selectedSourceClassSectionId = null;
    const selectedClass = this.classOptions.find(x => x.id === classId);
    this.sourceSectionOptions = selectedClass?.sections ?? [];
    this.clearPreview();
  }

  onSourceSectionChange(): void {
    this.clearPreview();
  }

  getStatusSeverity(status: string): 'success' | 'warning' | 'danger' | 'info' {
    switch (status) {
      case 'Passed':
        return 'success';
      case 'Promoted':
        return 'info';
      case 'No Result':
        return 'info';
      case 'NG':
      case 'NQ':
      case 'Failed':
        return 'danger';
      default:
        return 'warning';
    }
  }

  private loadAcademicYears(): void {
    this._lookupService.getAcademicYears().subscribe({
      next: (years) => {
        this.targetAcademicYearOptions = years.filter(x => x.id !== this.currentAcademicYearId);
        this.currentAcademicYearName = years.find(x => x.id === this.currentAcademicYearId)?.yearName ?? '';
      },
      error: () => {
        this._messageService.add({
          severity: 'error',
          summary: 'Academic Year Error',
          detail: 'Failed to load academic year options for promotion.'
        });
      }
    });
  }

  private loadClasses(): void {
    this._lookupService.getClassRooms().subscribe({
      next: (classes) => {
        this.classOptions = classes;
      },
      error: () => {
        this._messageService.add({
          severity: 'error',
          summary: 'Class Load Error',
          detail: 'Failed to load class and section options.'
        });
      }
    });
  }

  private executePromotion(promoteAllEligible: boolean): void {
    if (!this.lastFilter?.classSectionId || !this.lastFilter.examType) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Load Preview First',
        detail: 'Load promotion candidates before running promotion.'
      });
      return;
    }

    if (!this.targetAcademicYearId) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Target Year Required',
        detail: 'Select the target academic year for promotion.'
      });
      return;
    }

    const targetAcademicYearName = this.targetAcademicYearOptions.find(x => x.id === this.targetAcademicYearId)?.yearName ?? 'the selected year';
    const studentCount = promoteAllEligible ? this.promotableCandidates.length : this.selectedCandidates.length;
    const confirmationMessage = `Promote ${studentCount} student${studentCount === 1 ? '' : 's'} to ${targetAcademicYearName}?`;

    this._confirmationService.confirm({
      header: 'Confirm Promotion',
      message: confirmationMessage,
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-primary',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this.isPromoting = true;
        this._apiService.promoteStudents({
          classSectionId: this.lastFilter!.classSectionId!,
          examType: this.lastFilter!.examType!,
          targetAcademicYearId: this.targetAcademicYearId!,
          targetClassSectionId: null,
          promoteAllEligible,
          studentEnrollmentIds: promoteAllEligible ? [] : this.selectedCandidates.map(x => x.studentEnrollmentId)
        }).subscribe({
          next: (response) => {
            this._messageService.add({
              severity: 'success',
              summary: 'Promotion Completed',
              detail: `${response.promotedCount} student${response.promotedCount === 1 ? '' : 's'} promoted to ${response.targetAcademicYearName}. ${this.getPromotionSkipSummary(response)}`
            });
            this.selectedCandidates = [];
            if (this.lastFilter) {
              this.onLoadCandidates(this.lastFilter);
            }
            this.isPromoting = false;
          },
          error: (err) => {
            this._messageService.add({
              severity: 'error',
              summary: 'Promotion Failed',
              detail: err?.error?.message ?? 'Failed to complete normal promotion.'
            });
            this.isPromoting = false;
          }
        });
      }
    });
  }

  private executeSustain(sustainAllNonPromotable: boolean): void {
    if (!this.lastFilter?.classSectionId || !this.lastFilter.examType) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Load Preview First',
        detail: 'Load promotion candidates before sustaining students.'
      });
      return;
    }

    if (!this.targetAcademicYearId) {
      this._messageService.add({
        severity: 'warn',
        summary: 'Target Year Required',
        detail: 'Select the target academic year for sustain.'
      });
      return;
    }

    const targetAcademicYearName = this.targetAcademicYearOptions.find(x => x.id === this.targetAcademicYearId)?.yearName ?? 'the selected year';
    const studentCount = sustainAllNonPromotable ? this.notPromotableCandidates.length : this.selectedCandidates.length;

    this._confirmationService.confirm({
      header: 'Confirm Sustain',
      message: `Sustain ${studentCount} student${studentCount === 1 ? '' : 's'} in the same class-section for ${targetAcademicYearName}?`,
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-primary',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this.isPromoting = true;
        this._apiService.sustainStudents({
          classSectionId: this.lastFilter!.classSectionId!,
          examType: this.lastFilter!.examType!,
          targetAcademicYearId: this.targetAcademicYearId!,
          targetClassSectionId: this.lastFilter!.classSectionId!,
          promoteAllEligible: sustainAllNonPromotable,
          studentEnrollmentIds: sustainAllNonPromotable ? [] : this.selectedCandidates.map(x => x.studentEnrollmentId)
        }).subscribe({
          next: (response) => {
            this._messageService.add({
              severity: 'success',
              summary: 'Sustain Completed',
              detail: `${response.promotedCount} student${response.promotedCount === 1 ? '' : 's'} sustained in ${response.targetAcademicYearName}. ${this.getPromotionSkipSummary(response)}`
            });
            this.selectedCandidates = [];
            if (this.lastFilter) {
              this.onLoadCandidates(this.lastFilter);
            }
            this.isPromoting = false;
          },
          error: (err) => {
            this._messageService.add({
              severity: 'error',
              summary: 'Sustain Failed',
              detail: err?.error?.message ?? 'Failed to sustain students for the new academic year.'
            });
            this.isPromoting = false;
          }
        });
      }
    });
  }

  private executeManualPromotion(): void {
    if (!this.lastFilter?.classSectionId || !this.lastFilter.examType || !this.targetAcademicYearId || !this.manualTargetClassSectionId) {
      return;
    }

    const targetAcademicYearName = this.targetAcademicYearOptions.find(x => x.id === this.targetAcademicYearId)?.yearName ?? 'the selected year';
    const targetSectionName = this.targetSectionOptions.find(x => x.classSectionId === this.manualTargetClassSectionId)?.name ?? '';
    const targetClassName = this.classOptions.find(x => x.id === this.manualTargetClassId)?.name ?? 'selected class';

    this._confirmationService.confirm({
      header: 'Confirm Manual Promotion',
      message: `Manually promote ${this.selectedCandidates.length} student${this.selectedCandidates.length === 1 ? '' : 's'} to ${targetClassName}${targetSectionName ? ` - ${targetSectionName}` : ''} for ${targetAcademicYearName}?`,
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-primary',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this.isPromoting = true;
        this._apiService.manuallyPromoteStudents({
          classSectionId: this.lastFilter!.classSectionId!,
          examType: this.lastFilter!.examType!,
          targetAcademicYearId: this.targetAcademicYearId!,
          targetClassSectionId: this.manualTargetClassSectionId!,
          promoteAllEligible: false,
          studentEnrollmentIds: this.selectedCandidates.map(x => x.studentEnrollmentId)
        }).subscribe({
          next: (response) => {
            this._messageService.add({
              severity: 'success',
              summary: 'Manual Promotion Completed',
              detail: `${response.promotedCount} student${response.promotedCount === 1 ? '' : 's'} manually promoted to ${response.targetAcademicYearName}. ${this.getPromotionSkipSummary(response)}`
            });
            this.selectedCandidates = [];
            if (this.lastFilter) {
              this.onLoadCandidates(this.lastFilter);
            }
            this.isPromoting = false;
          },
          error: (err) => {
            this._messageService.add({
              severity: 'error',
              summary: 'Manual Promotion Failed',
              detail: err?.error?.message ?? 'Failed to manually promote the selected students.'
            });
            this.isPromoting = false;
          }
        });
      }
    });
  }

  private resetManualTarget(): void {
    this.manualTargetClassId = null;
    this.manualTargetClassSectionId = null;
    this.targetSectionOptions = [];
  }

  private clearPreview(): void {
    this.candidates = [];
    this.selectedCandidates = [];
    this.hasLoadedCandidates = false;
    this.visibilityFilter = 'all';
    this.tableFirst = 0;
  }

  private restoreSelectedCandidates(selectedEnrollmentIds: Set<string>): void {
    if (selectedEnrollmentIds.size === 0) {
      return;
    }

    this.selectedCandidates = this.getSelectableCandidates(
      this.candidates.filter(candidate => selectedEnrollmentIds.has(candidate.studentEnrollmentId))
    );
  }

  private getSelectableCandidates(selection: PromotionCandidateViewModel[]): PromotionCandidateViewModel[] {
    if (selection.length === 0) {
      return [];
    }

    const selectedEnrollmentIds = new Set(selection.map(x => x.studentEnrollmentId));
    const visibleIds = new Set(this.visibleCandidates.map(x => x.studentEnrollmentId));
    return this.candidates.filter(candidate =>
      selectedEnrollmentIds.has(candidate.studentEnrollmentId) &&
      visibleIds.has(candidate.studentEnrollmentId) &&
      !candidate.isAlreadyPromoted
    );
  }

  private getPromotionSkipSummary(response: PromotionExecutionResultViewModel): string {
    if (response.skippedCount === 0) {
      return 'No students skipped.';
    }

    const reasons: string[] = [];
    if (response.alreadyEnrolledCount > 0) {
      reasons.push(`${response.alreadyEnrolledCount} already enrolled`);
    }
    if (response.notEligibleCount > 0) {
      reasons.push(`${response.notEligibleCount} not eligible`);
    }
    if (response.missingTargetCount > 0) {
      reasons.push(`${response.missingTargetCount} missing target`);
    }

    return `${response.skippedCount} skipped${reasons.length ? ` (${reasons.join(', ')})` : ''}.`;
  }
}
