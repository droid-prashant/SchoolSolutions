import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { finalize, Subject, takeUntil } from 'rxjs';

import { ApiService } from '../../../shared/api.service';
import { AuthService } from '../../../shared/auth.service';
import { PermissionNames } from '../../../shared/common/constants/permission-names';
import { ProvinceViewModel } from '../../../shared/common/models/master/master.ViewModel';
import { SharedModule } from '../../../shared/shared.module';
import { MasterApiService } from '../../../shared/master-api.service';
import { ClassRoomViewModel } from '../class-room/shared/models/viewModels/classRoom.viewModel';
import { DateConverterService, BsDateParts } from '../../../shared/calender/date-convertor.service';
import { NpDatepickerComponent } from '../../../shared/calender/np-datepicker/np-datepicker.component';
import { DashboardActivityViewModel, DashboardNoticeViewModel, DashboardOverviewQuery, DashboardOverviewViewModel } from './model/dashboardOverview.viewModel';

type DashboardTone = 'blue' | 'green' | 'orange' | 'pink' | 'teal' | 'purple' | 'yellow' | 'indigo';
type DashboardPeriodKey = 'today' | 'this-week' | 'this-month' | 'this-academic-year' | 'custom';

interface DashboardKpiCard {
  label: string;
  value: string;
  subtext?: string;
  trend?: string;
  icon: string;
  tone: DashboardTone;
}

interface AttendanceLegendItem {
  label: string;
  value: number;
  percent: number;
  color: string;
}

interface DashboardQuickAction {
  label: string;
  icon: string;
  route: string;
  permissions: string[];
  tone: DashboardTone;
}

interface UpcomingExamItem {
  day: string;
  month: string;
  title: string;
  scope: string;
  badge: string;
  tone: 'green' | 'orange' | 'blue';
}

interface ActivityItem {
  title: string;
  description: string;
  time: string;
  tone: DashboardTone;
}

interface NoticeItem {
  title: string;
  badge: string;
  date: string;
  important: boolean;
}

interface DashboardPeriodOption {
  label: string;
  value: DashboardPeriodKey;
}

interface DashboardResolvedPeriod {
  key: DashboardPeriodKey;
  label: string;
  fromDate: string;
  toDate: string;
  fromDateNp: string;
  toDateNp: string;
  monthBuckets: string[];
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [SharedModule, RouterModule, FormsModule, NpDatepickerComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit, OnDestroy {
  readonly permissions = PermissionNames;
  readonly periodOptions: DashboardPeriodOption[] = [
    { label: 'Today', value: 'today' },
    { label: 'This Week', value: 'this-week' },
    { label: 'This Month', value: 'this-month' },
    { label: 'This Academic Year', value: 'this-academic-year' },
    { label: 'Custom Range', value: 'custom' }
  ];

  readonly quickActions: DashboardQuickAction[] = [
    {
      label: 'Add Student',
      icon: 'pi pi-user-plus',
      route: '/home/student/add-student',
      permissions: [PermissionNames.StudentCreate],
      tone: 'blue'
    },
    {
      label: 'Take Attendance',
      icon: 'pi pi-calendar-plus',
      route: '/home/attendance/student-entry',
      permissions: [PermissionNames.StudentAttendanceView, PermissionNames.StudentAttendanceTake, PermissionNames.StudentAttendanceEdit],
      tone: 'green'
    },
    {
      label: 'Add Fee Payment',
      icon: 'pi pi-money-bill',
      route: '/home/fees/fee-payment',
      permissions: [PermissionNames.FeeCreate],
      tone: 'pink'
    },
    {
      label: 'Publish Notice',
      icon: 'pi pi-megaphone',
      route: '/home/notices/manage',
      permissions: [PermissionNames.NoticeManage],
      tone: 'orange'
    },
    {
      label: 'Add Marks',
      icon: 'pi pi-file-edit',
      route: '/home/exam/exam-marks-entry',
      permissions: [PermissionNames.ExamMarksEntry],
      tone: 'purple'
    },
    {
      label: 'View Report Card',
      icon: 'pi pi-chart-line',
      route: '/home/exam/exam-results',
      permissions: [PermissionNames.ResultView],
      tone: 'teal'
    },
    {
      label: 'Add Teacher',
      icon: 'pi pi-user-plus',
      route: '/home/teacher/list-teacher',
      permissions: [PermissionNames.TeacherView, PermissionNames.TeacherManage],
      tone: 'yellow'
    },
    {
      label: 'Manage Classes',
      icon: 'pi pi-users',
      route: '/home/class/manage-class',
      permissions: [PermissionNames.ClassManage],
      tone: 'indigo'
    },
    {
      label: 'Manage Courses',
      icon: 'pi pi-book',
      route: '/home/course/manage-course',
      permissions: [PermissionNames.CourseManage],
      tone: 'pink'
    },
    {
      label: 'Create Exam',
      icon: 'pi pi-clipboard',
      route: '/home/exam/student-setup',
      permissions: [PermissionNames.ExamMarksEntry],
      tone: 'green'
    }
  ];

  readonly upcomingExams: UpcomingExamItem[] = [
    {
      day: '20',
      month: 'Jestha',
      title: 'First Terminal Examination',
      scope: 'Class 10',
      badge: 'In 5 days',
      tone: 'green'
    },
    {
      day: '25',
      month: 'Jestha',
      title: 'Unit Test - II',
      scope: 'Class 6 to 9',
      badge: 'In 10 days',
      tone: 'orange'
    },
    {
      day: '05',
      month: 'Ashar',
      title: 'Second Terminal Examination',
      scope: 'Class 11 & 12',
      badge: 'In 21 days',
      tone: 'blue'
    }
  ];

  overview: DashboardOverviewViewModel | null = null;
  kpiCards: DashboardKpiCard[] = [];
  attendanceLegend: AttendanceLegendItem[] = [];
  activityItems: ActivityItem[] = [];
  noticeItems: NoticeItem[] = [];
  loading = true;
  errorMessage = '';
  hasAnyInsight = false;
  displayName = 'Admin Developer';
  selectedPeriodKey: DashboardPeriodKey = 'this-month';
  periodLabel = 'This Month';
  periodSummary = '';
  periodTrendLabel = 'in this month';
  customFromBs = '';
  customToBs = '';
  private customFromAd = '';
  private customToAd = '';

  attendanceChartData: any;
  attendanceChartOptions: any;
  feeTrendChartData: any;
  feeTrendChartOptions: any;
  classDistributionChartData: any;
  classDistributionChartOptions: any;

  provinceDetails: ProvinceViewModel[] = [];
  classRooms: ClassRoomViewModel[] = [];

  private readonly destroy$ = new Subject<void>();
  private readonly numberFormatter = new Intl.NumberFormat('en-IN');
  private readonly bsMonthNames = [
    'Baisakh',
    'Jestha',
    'Asar',
    'Shrawan',
    'Bhadra',
    'Ashwin',
    'Kartik',
    'Mangsir',
    'Poush',
    'Magh',
    'Falgun',
    'Chaitra'
  ];

  constructor(
    private apiService: ApiService,
    private masterApiService: MasterApiService,
    private authService: AuthService,
    private dateConverter: DateConverterService
  ) { }

  ngOnInit(): void {
    this.displayName = this.resolveDisplayName();
    this.loadDashboard();
    this.warmLookupCaches();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadDashboard(): void {
    this.loading = true;
    this.errorMessage = '';
    const period = this.resolveDashboardPeriod();
    this.periodLabel = period.label;
    this.periodSummary = this.formatPeriodSummary(period);
    this.periodTrendLabel = this.getPeriodTrendLabel(period.key);

    this.apiService.getDashboardOverview(this.toDashboardQuery(period))
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.loading = false)
      )
      .subscribe({
        next: overview => {
          this.overview = overview;
          this.displayName = this.resolveDisplayName(overview);
          this.buildDashboardView(overview);
        },
        error: () => {
          this.overview = null;
          this.resetDashboardView();
          this.errorMessage = 'Dashboard data could not be loaded right now. Please refresh or try again later.';
        }
      });
  }

  trackByIndex(index: number): number {
    return index;
  }

  onPeriodChange(): void {
    if (this.selectedPeriodKey === 'custom') {
      this.ensureCustomRange();
    }

    this.loadDashboard();
  }

  onCustomFromDateChange(value: { bs: string; ad: string }): void {
    this.customFromBs = value.bs;
    this.customFromAd = value.ad;
  }

  onCustomToDateChange(value: { bs: string; ad: string }): void {
    this.customToBs = value.bs;
    this.customToAd = value.ad;
  }

  applyCustomPeriod(): void {
    this.ensureCustomRange();
    this.loadDashboard();
  }

  private buildDashboardView(overview: DashboardOverviewViewModel): void {
    const summary = overview.summary;
    const attendanceTotal = overview.attendance.totalMarked ||
      summary.presentToday + summary.absentToday + summary.lateToday + summary.leaveToday + summary.halfDayToday;
    const cards: DashboardKpiCard[] = [];

    if (overview.permissions.canViewStudents) {
      cards.push({
        label: 'Total Students',
        value: this.formatNumber(summary.totalStudents),
        subtext: `Active: ${this.formatNumber(summary.activeStudents)}`,
        trend: `${this.formatNumber(summary.newAdmissionsThisMonth)} ${this.periodTrendLabel}`,
        icon: 'pi pi-user-plus',
        tone: 'blue'
      });
    }

    if (overview.permissions.canViewTeachers) {
      cards.push({
        label: 'Total Teachers',
        value: this.formatNumber(summary.totalTeachers),
        subtext: 'Active faculty',
        icon: 'pi pi-user',
        tone: 'purple'
      });
    }

    if (overview.permissions.canViewClasses) {
      cards.push({
        label: 'Total Classes',
        value: this.formatNumber(summary.totalClasses),
        subtext: 'Configured class levels',
        icon: 'pi pi-book',
        tone: 'green'
      });
    }

    if (overview.permissions.canViewCourses) {
      cards.push({
        label: 'Total Courses',
        value: this.formatNumber(summary.totalCourses),
        subtext: 'Active course catalog',
        icon: 'pi pi-clone',
        tone: 'orange'
      });
    }

    if (overview.permissions.canViewAttendance) {
      cards.push({
        label: 'Today\'s Attendance',
        value: `${this.formatNumber(attendanceTotal)} / ${this.formatNumber(summary.activeStudents)}`,
        subtext: `Present: ${this.formatNumber(summary.presentToday)}`,
        trend: `${this.formatPercent(overview.attendance.attendancePercentage)}`,
        icon: 'pi pi-user-plus',
        tone: 'teal'
      });
    }

    if (overview.permissions.canViewFees) {
      cards.push({
        label: 'Pending Fees',
        value: this.formatCurrency(summary.pendingFees),
        subtext: `From ${this.formatNumber(summary.pendingFeeStudents)} Students`,
        icon: 'pi pi-money-bill',
        tone: 'pink'
      });
    }

    this.kpiCards = cards;
    this.attendanceLegend = this.buildAttendanceLegend(overview);
    this.activityItems = overview.activities.slice(0, 5).map(activity => this.mapActivity(activity));
    this.noticeItems = overview.notices.recent.slice(0, 4).map(notice => this.mapNotice(notice));
    this.initializeCharts(overview);

    this.hasAnyInsight = this.kpiCards.length > 0 ||
      this.activityItems.length > 0 ||
      this.noticeItems.length > 0 ||
      this.upcomingExams.length > 0;
  }

  private initializeCharts(overview: DashboardOverviewViewModel): void {
    this.attendanceChartData = this.attendanceLegend.some(x => x.value > 0) ? {
      labels: this.attendanceLegend.map(x => x.label),
      datasets: [
        {
          data: this.attendanceLegend.map(x => x.value),
          backgroundColor: this.attendanceLegend.map(x => x.color),
          borderColor: '#ffffff',
          borderWidth: 4,
          hoverOffset: 6
        }
      ]
    } : null;

    this.attendanceChartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      cutout: '66%',
      plugins: {
        legend: {
          display: false
        }
      }
    };

    const feeMonths = overview.fees.monthlyCollection.slice(-6);
    this.feeTrendChartData = feeMonths.length > 0 ? {
      labels: feeMonths.map(x => x.month),
      datasets: [
        {
          label: 'Collected',
          data: feeMonths.map(x => x.amount),
          borderColor: '#16a34a',
          backgroundColor: 'rgba(34, 197, 94, 0.12)',
          pointBackgroundColor: '#16a34a',
          pointBorderColor: '#ffffff',
          pointRadius: 3,
          fill: true,
          tension: 0.35
        },
        {
          label: 'Pending',
          data: feeMonths.map(() => overview.fees.pendingAmount),
          borderColor: '#fb7185',
          backgroundColor: 'rgba(251, 113, 133, 0.1)',
          pointBackgroundColor: '#fb7185',
          pointBorderColor: '#ffffff',
          pointRadius: 3,
          fill: true,
          tension: 0.35
        }
      ]
    } : null;

    this.feeTrendChartOptions = this.getLineOptions();

    const sectionDistribution = overview.students.sectionWiseDistribution ?? [];
    const sectionColors = ['#2563eb', '#22c55e', '#f59e0b', '#ef4444', '#8b5cf6', '#14b8a6', '#ec4899', '#64748b'];

    if (sectionDistribution.length > 0) {
      const classLabels = Array.from(new Set(sectionDistribution.map(x => x.className)));
      const sectionNames = Array.from(new Set(sectionDistribution.map(x => x.sectionName)));

      this.classDistributionChartData = {
        labels: classLabels,
        datasets: sectionNames.map((sectionName, index) => ({
          label: sectionName,
          backgroundColor: sectionColors[index % sectionColors.length],
          borderRadius: 4,
          borderSkipped: false,
          maxBarThickness: 24,
          data: classLabels.map(className =>
            sectionDistribution.find(x => x.className === className && x.sectionName === sectionName)?.studentCount ?? 0)
        }))
      };

      this.classDistributionChartOptions = this.getBarOptions(true, true);
    } else {
      const chartValues = overview.students.classWiseDistribution.map(x => x.studentCount);
      this.classDistributionChartData = chartValues.length > 0 ? {
        labels: overview.students.classWiseDistribution.map(x => x.className),
        datasets: [
          {
            label: 'Students',
            backgroundColor: '#2f6fec',
            hoverBackgroundColor: '#1d4ed8',
            borderRadius: 4,
            maxBarThickness: 20,
            data: chartValues
          }
        ]
      } : null;

      this.classDistributionChartOptions = this.getBarOptions(false, false);
    }
  }

  private buildAttendanceLegend(overview: DashboardOverviewViewModel): AttendanceLegendItem[] {
    const attendance = overview.attendance;
    const total = attendance.totalMarked || attendance.present + attendance.absent + attendance.late + attendance.leave + attendance.halfDay;
    const items: AttendanceLegendItem[] = [
      { label: 'Present', value: attendance.present, percent: this.percentOf(attendance.present, total), color: '#22c55e' },
      { label: 'Absent', value: attendance.absent, percent: this.percentOf(attendance.absent, total), color: '#ef4444' }
    ];

    if (attendance.late > 0) {
      items.push({ label: 'Late', value: attendance.late, percent: this.percentOf(attendance.late, total), color: '#f97316' });
    }

    items.push(
      { label: 'Leave', value: attendance.leave, percent: this.percentOf(attendance.leave, total), color: '#f59e0b' },
      { label: 'Half Day', value: attendance.halfDay, percent: this.percentOf(attendance.halfDay, total), color: '#2563eb' }
    );

    return items;
  }

  private mapActivity(activity: DashboardActivityViewModel): ActivityItem {
    return {
      title: activity.title,
      description: activity.description,
      time: this.relativeTime(activity.occurredAt),
      tone: this.mapSeverityTone(activity.severity)
    };
  }

  private mapNotice(notice: DashboardNoticeViewModel): NoticeItem {
    const important = /important|closed|admission/i.test(notice.title);

    return {
      title: notice.title,
      badge: important ? 'Important' : 'General',
      date: notice.noticeDateNp || this.formatDate(notice.noticeDate),
      important
    };
  }

  private resetDashboardView(): void {
    this.kpiCards = [];
    this.attendanceLegend = [];
    this.activityItems = [];
    this.noticeItems = [];
    this.hasAnyInsight = false;
    this.attendanceChartData = null;
    this.feeTrendChartData = null;
    this.classDistributionChartData = null;
  }

  private resolveDashboardPeriod(): DashboardResolvedPeriod {
    const todayAd = this.dateConverter.formatAd(new Date());
    const todayBs = this.dateConverter.adToBs(todayAd);
    const todayBsParts = this.parseBs(todayBs);

    switch (this.selectedPeriodKey) {
      case 'today':
        return this.createResolvedPeriod('today', `Today (${this.formatBsLabel(todayBs)})`, todayAd, todayAd, todayBs, todayBs);
      case 'this-week':
        return this.resolveCurrentWeek(todayAd);
      case 'this-academic-year':
        return this.resolveAcademicYearPeriod(todayBsParts);
      case 'custom':
        this.ensureCustomRange();
        return this.createResolvedPeriod('custom', 'Custom Range', this.customFromAd, this.customToAd, this.customFromBs, this.customToBs);
      case 'this-month':
      default:
        return this.resolveCurrentMonth(todayBsParts);
    }
  }

  private resolveCurrentWeek(todayAd: string): DashboardResolvedPeriod {
    const today = this.parseAdDate(todayAd);
    const weekStart = this.addAdDays(today, -today.getDay());
    const weekEnd = this.addAdDays(weekStart, 6);
    const fromDate = this.formatAdDateParts(weekStart);
    const toDate = this.formatAdDateParts(weekEnd);
    const fromDateNp = this.dateConverter.adToBs(fromDate);
    const toDateNp = this.dateConverter.adToBs(toDate);

    return this.createResolvedPeriod('this-week', 'This Week', fromDate, toDate, fromDateNp, toDateNp);
  }

  private resolveCurrentMonth(todayBsParts: BsDateParts): DashboardResolvedPeriod {
    const startBs = this.formatBsDate({ ...todayBsParts, day: 1 });
    const endBs = this.formatBsDate(this.getBsMonthEnd(todayBsParts.year, todayBsParts.month));
    const label = `${this.bsMonthNames[todayBsParts.month - 1]} ${todayBsParts.year}`;

    return this.createResolvedPeriod('this-month', label, this.dateConverter.bsToAd(startBs), this.dateConverter.bsToAd(endBs), startBs, endBs);
  }

  private resolveAcademicYearPeriod(todayBsParts: BsDateParts): DashboardResolvedPeriod {
    const fallbackStartBs = `${todayBsParts.year}-01-01`;
    const fallbackEndBs = this.formatBsDate(this.getBsMonthEnd(todayBsParts.year, 12));
    const fromDateNp = this.overview?.academicYearStartDateNp || fallbackStartBs;
    const toDateNp = this.overview?.academicYearEndDateNp || fallbackEndBs;
    const fromDate = this.overview?.academicYearStartDateEn || this.dateConverter.bsToAd(fromDateNp);
    const toDate = this.overview?.academicYearEndDateEn || this.dateConverter.bsToAd(toDateNp);
    const label = this.overview?.academicYearName ? `Academic Year ${this.overview.academicYearName}` : 'This Academic Year';

    return this.createResolvedPeriod('this-academic-year', label, fromDate, toDate, fromDateNp, toDateNp);
  }

  private createResolvedPeriod(
    key: DashboardPeriodKey,
    label: string,
    fromDate: string,
    toDate: string,
    fromDateNp: string,
    toDateNp: string
  ): DashboardResolvedPeriod {
    if (this.compareAdDates(fromDate, toDate) > 0) {
      [fromDate, toDate] = [toDate, fromDate];
      [fromDateNp, toDateNp] = [toDateNp, fromDateNp];
    }

    return {
      key,
      label,
      fromDate,
      toDate,
      fromDateNp,
      toDateNp,
      monthBuckets: this.buildNepaliMonthBuckets(this.parseBs(toDateNp))
    };
  }

  private toDashboardQuery(period: DashboardResolvedPeriod): DashboardOverviewQuery {
    return {
      periodKey: period.key,
      periodLabel: period.label,
      fromDate: period.fromDate,
      toDate: period.toDate,
      fromDateNp: period.fromDateNp,
      toDateNp: period.toDateNp,
      monthBuckets: period.monthBuckets
    };
  }

  private ensureCustomRange(): void {
    if (this.customFromBs && this.customToBs && this.customFromAd && this.customToAd) {
      return;
    }

    const currentMonth = this.resolveCurrentMonth(this.dateConverter.adToBsParts(new Date()));
    this.customFromBs = currentMonth.fromDateNp;
    this.customToBs = currentMonth.toDateNp;
    this.customFromAd = currentMonth.fromDate;
    this.customToAd = currentMonth.toDate;
  }

  private buildNepaliMonthBuckets(endBs: BsDateParts): string[] {
    return Array.from({ length: 6 }, (_, index) => {
      const month = this.addBsMonths(endBs.year, endBs.month, index - 5);
      const startBs = this.formatBsDate({ ...month, day: 1 });
      const endMonth = this.getBsMonthEnd(month.year, month.month);
      const endBsString = this.formatBsDate(endMonth);
      const label = `${this.bsMonthNames[month.month - 1]} ${month.year}`;

      return `${label}|${this.dateConverter.bsToAd(startBs)}|${this.dateConverter.bsToAd(endBsString)}`;
    });
  }

  private getBsMonthEnd(year: number, month: number): BsDateParts {
    const firstDayAd = this.dateConverter.bsToAdParts({ year, month, day: 1 });
    const nextMonth = this.addBsMonths(year, month, 1);
    const nextMonthAd = this.dateConverter.bsToAdParts({ year: nextMonth.year, month: nextMonth.month, day: 1 });
    const firstDay = new Date(firstDayAd.year, firstDayAd.month - 1, firstDayAd.day);
    const nextFirstDay = new Date(nextMonthAd.year, nextMonthAd.month - 1, nextMonthAd.day);
    const daysInMonth = Math.round((nextFirstDay.getTime() - firstDay.getTime()) / 86400000);

    return { year, month, day: daysInMonth >= 28 && daysInMonth <= 32 ? daysInMonth : 30 };
  }

  private addBsMonths(year: number, month: number, offset: number): BsDateParts {
    const zeroBasedMonth = year * 12 + (month - 1) + offset;
    return {
      year: Math.floor(zeroBasedMonth / 12),
      month: (zeroBasedMonth % 12) + 1,
      day: 1
    };
  }

  private parseBs(value: string): BsDateParts {
    const [year, month, day] = value.split('-').map(Number);
    return { year, month, day };
  }

  private parseAdDate(value: string): Date {
    const [year, month, day] = value.split('-').map(Number);
    return new Date(year, month - 1, day);
  }

  private addAdDays(value: Date, days: number): Date {
    const date = new Date(value);
    date.setDate(date.getDate() + days);
    return date;
  }

  private formatAdDateParts(value: Date): string {
    return `${value.getFullYear()}-${this.pad(value.getMonth() + 1)}-${this.pad(value.getDate())}`;
  }

  private formatBsDate(value: BsDateParts): string {
    return `${value.year}-${this.pad(value.month)}-${this.pad(value.day)}`;
  }

  private formatBsLabel(value: string): string {
    const bs = this.parseBs(value);
    return `${this.bsMonthNames[bs.month - 1]} ${bs.day}, ${bs.year}`;
  }

  private formatPeriodSummary(period: DashboardResolvedPeriod): string {
    if (period.fromDateNp === period.toDateNp) {
      return this.formatBsLabel(period.fromDateNp);
    }

    return `${this.formatBsLabel(period.fromDateNp)} - ${this.formatBsLabel(period.toDateNp)}`;
  }

  private getPeriodTrendLabel(key: DashboardPeriodKey): string {
    switch (key) {
      case 'today':
        return 'today';
      case 'this-week':
        return 'this week';
      case 'this-academic-year':
        return 'this academic year';
      case 'custom':
        return 'in selected range';
      case 'this-month':
      default:
        return 'this month';
    }
  }

  private compareAdDates(first: string, second: string): number {
    return first.localeCompare(second);
  }

  private pad(value: number): string {
    return `${value}`.padStart(2, '0');
  }

  private warmLookupCaches(): void {
    this.apiService.getProvinceDetails()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: response => {
          this.provinceDetails = response;
          localStorage.setItem('provinceDetails', JSON.stringify(response));
        }
      });

    this.masterApiService.getClassRooms()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: response => {
          this.classRooms = response;
          localStorage.setItem('classRoomDetails', JSON.stringify(response));
        }
      });
  }

  private resolveDisplayName(overview?: DashboardOverviewViewModel): string {
    const token = this.authService.decodeToken();
    return overview?.userName || token.name || token.username || 'Admin Developer';
  }

  private getLineOptions(): any {
    return {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'top',
          labels: {
            color: '#53657d',
            usePointStyle: true,
            boxWidth: 8
          }
        }
      },
      scales: {
        x: {
          grid: {
            color: 'rgba(148, 163, 184, 0.12)'
          },
          ticks: {
            color: '#697d98',
            font: { size: 11 }
          }
        },
        y: {
          beginAtZero: true,
          grid: {
            color: 'rgba(148, 163, 184, 0.18)'
          },
          ticks: {
            color: '#697d98',
            font: { size: 11 }
          }
        }
      }
    };
  }

  private getBarOptions(showLegend: boolean, stacked: boolean): any {
    return {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: showLegend,
          position: 'top',
          labels: {
            color: '#53657d',
            usePointStyle: true,
            boxWidth: 8
          }
        }
      },
      scales: {
        x: {
          stacked,
          grid: {
            display: false
          },
          ticks: {
            color: '#697d98',
            font: { size: 11 }
          }
        },
        y: {
          stacked,
          beginAtZero: true,
          grid: {
            color: 'rgba(148, 163, 184, 0.2)'
          },
          ticks: {
            color: '#697d98',
            font: { size: 11 }
          }
        }
      }
    };
  }

  private formatNumber(value: number | null | undefined): string {
    return this.numberFormatter.format(value ?? 0);
  }

  private formatCurrency(value: number | null | undefined): string {
    return `Rs. ${this.formatNumber(Math.round(value ?? 0))}`;
  }

  private formatPercent(value: number | null | undefined): string {
    return `${(value ?? 0).toFixed(2)}%`;
  }

  private percentOf(value: number, total: number): number {
    return total > 0 ? (value / total) * 100 : 0;
  }

  private relativeTime(value: string): string {
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

  private formatDate(value: string): string {
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? '-' : date.toISOString().slice(0, 10);
  }

  private mapSeverityTone(severity: string): DashboardTone {
    switch ((severity || '').toLowerCase()) {
      case 'success':
        return 'green';
      case 'warning':
        return 'orange';
      case 'danger':
        return 'pink';
      case 'info':
        return 'blue';
      default:
        return 'purple';
    }
  }
}
