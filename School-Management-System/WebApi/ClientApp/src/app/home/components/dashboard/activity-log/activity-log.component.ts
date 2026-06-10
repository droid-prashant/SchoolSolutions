import { Component, OnDestroy, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PaginatorModule } from 'primeng/paginator';
import { finalize, Subject, takeUntil } from 'rxjs';

import { ApiService } from '../../../../shared/api.service';
import { SharedModule } from '../../../../shared/shared.module';
import { DashboardActivityViewModel } from '../model/dashboardOverview.viewModel';

@Component({
  selector: 'app-activity-log',
  standalone: true,
  imports: [SharedModule, RouterModule, PaginatorModule],
  templateUrl: './activity-log.component.html',
  styleUrl: './activity-log.component.scss'
})
export class ActivityLogComponent implements OnInit, OnDestroy {
  readonly activityTypes = [
    { label: 'All Activities', value: '' },
    { label: 'Student', value: 'Student' },
    { label: 'Attendance', value: 'Attendance' },
    { label: 'Fee', value: 'Fee' },
    { label: 'Notice', value: 'Notice' },
    { label: 'Exam', value: 'Exam' }
  ];

  activities: DashboardActivityViewModel[] = [];
  selectedType = '';
  fromDate: Date | null = null;
  toDate: Date | null = null;
  loading = false;
  errorMessage = '';
  totalRecords = 0;
  page = 1;
  pageSize = 20;

  private readonly destroy$ = new Subject<void>();

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.loadActivities();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  applyFilters(): void {
    this.page = 1;
    this.loadActivities();
  }

  resetFilters(): void {
    this.selectedType = '';
    this.fromDate = null;
    this.toDate = null;
    this.page = 1;
    this.loadActivities();
  }

  onPageChange(event: any): void {
    this.pageSize = event.rows;
    this.page = Math.floor(event.first / event.rows) + 1;
    this.loadActivities();
  }

  trackByIndex(index: number): number {
    return index;
  }

  loadActivities(): void {
    this.loading = true;
    this.errorMessage = '';

    this.apiService.getDashboardActivities({
      page: this.page,
      pageSize: this.pageSize,
      type: this.selectedType || undefined,
      fromDate: this.formatDateParam(this.fromDate),
      toDate: this.formatDateParam(this.toDate)
    })
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.loading = false)
      )
      .subscribe({
        next: result => {
          this.activities = result.items;
          this.totalRecords = result.totalCount;
          this.page = result.page;
          this.pageSize = result.pageSize;
        },
        error: () => {
          this.activities = [];
          this.totalRecords = 0;
          this.errorMessage = 'Activity log could not be loaded right now.';
        }
      });
  }

  getTone(activity: DashboardActivityViewModel): string {
    switch ((activity.severity || '').toLowerCase()) {
      case 'success':
        return 'green';
      case 'warning':
        return 'orange';
      case 'danger':
        return 'red';
      case 'info':
        return 'blue';
      default:
        return 'purple';
    }
  }

  getRelativeTime(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
      return '-';
    }

    const diffMinutes = Math.max(0, Math.floor((Date.now() - date.getTime()) / 60000));
    if (diffMinutes < 60) {
      return `${diffMinutes || 1} min ago`;
    }

    const diffHours = Math.floor(diffMinutes / 60);
    if (diffHours < 24) {
      return `${diffHours} hour${diffHours === 1 ? '' : 's'} ago`;
    }

    const diffDays = Math.floor(diffHours / 24);
    return `${diffDays} day${diffDays === 1 ? '' : 's'} ago`;
  }

  private formatDateParam(value: Date | null): string | undefined {
    if (!value) {
      return undefined;
    }

    const year = value.getFullYear();
    const month = `${value.getMonth() + 1}`.padStart(2, '0');
    const day = `${value.getDate()}`.padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
