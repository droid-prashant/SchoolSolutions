import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { DateConverterService } from '../../../../shared/calender/date-convertor.service';
import { LookupService } from '../../../../shared/common/lookup.service';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { NoticeApiService } from '../shared/notice-api.service';
import { NoticeAudienceOption, NoticeDto, NoticeViewModel } from '../shared/notice.models';

@Component({
  selector: 'app-manage-notices',
  standalone: false,
  templateUrl: './manage-notices.component.html',
  styleUrl: './manage-notices.component.scss'
})
export class ManageNoticesComponent implements OnInit {
  noticeForm: FormGroup;
  notices: NoticeViewModel[] = [];
  classRooms: ClassRoomViewModel[] = [];
  sections: SectionViewModel[] = [];
  filteredSections: SectionViewModel[] = [];
  students: StudentViewModel[] = [];
  targetAudiences: NoticeAudienceOption[] = [
    { label: 'All Guardians', value: 'AllGuardians' },
    { label: 'Class Wise', value: 'ClassWise' },
    { label: 'Section Wise', value: 'SectionWise' },
    { label: 'Student Wise', value: 'StudentWise' }
  ];
  audienceFilterOptions: NoticeAudienceOption[] = [
    { label: 'All Audiences', value: '' },
    ...this.targetAudiences
  ];
  noticeSearchTerm = '';
  selectedAudienceFilter = '';

  isSaving = false;
  isLoading = false;
  editingNoticeId = '';

  constructor(
    private fb: FormBuilder,
    private noticeApiService: NoticeApiService,
    private apiService: ApiService,
    private lookupService: LookupService,
    private dateConverter: DateConverterService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {
    const today = new Date();
    this.noticeForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.required],
      noticeDate: [this.dateConverter.formatAd(today), Validators.required],
      noticeDateNp: [this.dateConverter.adToBs(today), Validators.required],
      targetAudience: ['AllGuardians', Validators.required],
      classId: [null],
      sectionId: [null],
      studentIds: [[]]
    });
  }

  ngOnInit(): void {
    this.loadLookups();
    this.loadNotices();
  }

  get isEditing(): boolean {
    return !!this.editingNoticeId;
  }

  get selectedTargetAudience(): string {
    return this.noticeForm.get('targetAudience')?.value;
  }

  get filteredNotices(): NoticeViewModel[] {
    const searchTerm = this.noticeSearchTerm.trim().toLowerCase();
    return this.notices.filter(notice => {
      const matchesAudience = !this.selectedAudienceFilter || notice.targetAudience === this.selectedAudienceFilter;
      if (!matchesAudience) {
        return false;
      }

      if (!searchTerm) {
        return true;
      }

      return this.buildNoticeSearchText(notice).includes(searchTerm);
    });
  }

  showClassSelector(): boolean {
    return this.selectedTargetAudience === 'ClassWise' ||
      this.selectedTargetAudience === 'SectionWise' ||
      this.selectedTargetAudience === 'StudentWise';
  }

  showSectionSelector(): boolean {
    return this.selectedTargetAudience === 'SectionWise' ||
      (this.selectedTargetAudience === 'StudentWise' && !!this.noticeForm.get('classId')?.value);
  }

  showStudentSelector(): boolean {
    return this.selectedTargetAudience === 'StudentWise';
  }

  onNoticeDateChange(event: { bs: string; ad: string }): void {
    if (!event.bs || !event.ad) {
      return;
    }

    this.noticeForm.patchValue({
      noticeDate: event.ad,
      noticeDateNp: event.bs
    });
  }

  onTargetAudienceChange(): void {
    const targetAudience = this.selectedTargetAudience;
    const classControl = this.noticeForm.get('classId');
    const sectionControl = this.noticeForm.get('sectionId');
    const studentControl = this.noticeForm.get('studentIds');

    classControl?.clearValidators();
    sectionControl?.clearValidators();
    studentControl?.clearValidators();

    if (targetAudience === 'ClassWise') {
      classControl?.setValidators([Validators.required]);
      sectionControl?.setValue(null);
      studentControl?.setValue([]);
    }

    if (targetAudience === 'SectionWise') {
      classControl?.setValidators([Validators.required]);
      sectionControl?.setValidators([Validators.required]);
      studentControl?.setValue([]);
    }

    if (targetAudience === 'StudentWise') {
      studentControl?.setValidators([Validators.required]);
    }

    if (targetAudience === 'AllGuardians') {
      classControl?.setValue(null);
      sectionControl?.setValue(null);
      studentControl?.setValue([]);
    }

    classControl?.updateValueAndValidity();
    sectionControl?.updateValueAndValidity();
    studentControl?.updateValueAndValidity();
    this.refreshSectionsForClass();
    this.loadStudentsForSelection();
  }

  onClassChange(): void {
    this.noticeForm.patchValue({
      sectionId: null,
      studentIds: []
    });
    this.refreshSectionsForClass();
    this.loadStudentsForSelection();
  }

  onSectionChange(): void {
    this.noticeForm.patchValue({ studentIds: [] });
    this.loadStudentsForSelection();
  }

  saveNotice(): void {
    this.onTargetAudienceChange();
    this.noticeForm.markAllAsTouched();
    if (this.noticeForm.invalid) {
      return;
    }

    this.isSaving = true;
    const request = this.buildNoticeDto();
    const save$ = this.isEditing
      ? this.noticeApiService.updateNotice(this.editingNoticeId, request)
      : this.noticeApiService.createNotice(request);

    save$.subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: this.isEditing ? 'Notice updated.' : 'Notice saved.'
        });
        this.resetForm();
        this.loadNotices();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to save notice.' });
      },
      complete: () => {
        this.isSaving = false;
      }
    });
  }

  editNotice(notice: NoticeViewModel): void {
    if (notice.isPublished) {
      return;
    }

    this.editingNoticeId = notice.id;
    this.noticeForm.patchValue({
      title: notice.title,
      description: notice.description,
      noticeDate: notice.noticeDate,
      noticeDateNp: notice.noticeDateNp,
      targetAudience: notice.targetAudience,
      classId: notice.classId ?? null,
      sectionId: notice.sectionId ?? null,
      studentIds: notice.studentIds ?? []
    });
    this.onTargetAudienceChange();
  }

  publishNotice(notice: NoticeViewModel): void {
    this.confirmationService.confirm({
      header: 'Publish Notice',
      message: `Publish "${notice.title}" and send it to guardians?`,
      icon: 'pi pi-send',
      acceptLabel: 'Publish',
      rejectLabel: 'Cancel',
      accept: () => {
        this.noticeApiService.publishNotice(notice.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Published', detail: 'Notice sent to guardians.' });
            this.loadNotices();
          },
          error: () => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to publish notice.' });
          }
        });
      }
    });
  }

  resetForm(): void {
    const today = new Date();
    this.editingNoticeId = '';
    this.noticeForm.reset({
      title: '',
      description: '',
      noticeDate: this.dateConverter.formatAd(today),
      noticeDateNp: this.dateConverter.adToBs(today),
      targetAudience: 'AllGuardians',
      classId: null,
      sectionId: null,
      studentIds: []
    });
    this.filteredSections = [];
    this.students = [];
    this.onTargetAudienceChange();
  }

  loadNotices(): void {
    this.isLoading = true;
    this.noticeApiService.getNotices().subscribe({
      next: notices => {
        this.notices = notices;
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load notices.' });
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  clearGridFilters(): void {
    this.noticeSearchTerm = '';
    this.selectedAudienceFilter = '';
  }

  private loadLookups(): void {
    this.lookupService.getClassRooms().subscribe({
      next: classRooms => {
        this.classRooms = classRooms;
        this.refreshSectionsForClass();
      }
    });

    this.lookupService.getAllSections().subscribe({
      next: sections => {
        this.sections = sections;
      }
    });
  }

  private buildNoticeDto(): NoticeDto {
    const value = this.noticeForm.value;
    return {
      title: value.title?.trim(),
      description: value.description?.trim(),
      noticeDate: value.noticeDate,
      noticeDateNp: value.noticeDateNp,
      targetAudience: value.targetAudience,
      classId: value.classId || null,
      sectionId: value.sectionId || null,
      studentIds: value.studentIds || []
    };
  }

  private refreshSectionsForClass(): void {
    const classId = this.noticeForm.get('classId')?.value;
    if (!classId) {
      this.filteredSections = this.selectedTargetAudience === 'StudentWise' ? [] : this.sections;
      return;
    }

    const selectedClass = this.classRooms.find(x => String(x.id) === String(classId));
    this.filteredSections = selectedClass?.sections ?? [];
  }

  private loadStudentsForSelection(): void {
    if (this.selectedTargetAudience !== 'StudentWise') {
      this.students = [];
      return;
    }

    const classId = this.noticeForm.get('classId')?.value;
    const sectionId = this.noticeForm.get('sectionId')?.value;
    const classSectionId = this.getSelectedClassSectionId(sectionId);
    const request$ = classSectionId
      ? this.apiService.getStudentsByClassSectionId(classSectionId, null, true)
      : classId
        ? this.apiService.getStudentsByClass(classId, true)
        : this.apiService.getStudents(true);

    request$.subscribe({
      next: students => {
        this.students = (students ?? []).map(student => ({
          ...student,
          displayName: this.buildStudentDisplayName(student)
        }));
      },
      error: () => {
        this.students = [];
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load students.' });
      }
    });
  }

  private getSelectedClassSectionId(sectionId: string | null): string | null {
    if (!sectionId) {
      return null;
    }

    return this.filteredSections.find(x => String(x.sectionId) === String(sectionId))?.classSectionId ?? null;
  }

  private buildStudentDisplayName(student: StudentViewModel): string {
    const name = `${student.firstName || ''} ${student.lastName || ''}`.trim();
    const classSection = [student.classRoomName, student.sectionName].filter(x => !!x).join(' - ');
    return classSection ? `${name} (${classSection})` : name;
  }

  private buildNoticeSearchText(notice: NoticeViewModel): string {
    return [
      notice.title,
      notice.description,
      notice.targetAudience,
      notice.noticeDateNp,
      notice.className,
      notice.sectionName,
      ...(notice.studentNames ?? [])
    ]
      .filter(x => !!x)
      .join(' ')
      .toLowerCase();
  }
}
