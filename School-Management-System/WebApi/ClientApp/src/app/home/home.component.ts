import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { AuthService } from '../shared/auth.service';
import { LookupService } from '../shared/common/lookup.service';
import { PermissionNames } from '../shared/common/constants/permission-names';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  sidebarItems: MenuItem[] = [];
  avatarMenuItems: MenuItem[] = [];
  isSidebarCollapsed = false;
  schoolName = 'Om Pushpanjali English School';
  currentUserName = 'User';
  currentUserEmail = '';
  currentUserRole = '';
  currentAcademicYearName = '';
  userInitials = 'U';
  userAvatarImage = 'assets/Om Pushpanjali logo.png';

  constructor(
    private _router: Router,
    private _authService: AuthService,
    private _lookupService: LookupService
  ) { }

  ngOnInit(): void {
    this.loadCurrentUser();
    this.buildSidebar();
    this.buildAvatarMenu();
  }

  loadCurrentUser(): void {
    const decodedToken = this._authService.decodeToken();
    this.currentUserName = decodedToken?.name || decodedToken?.username || 'User';
    this.currentUserEmail = decodedToken?.email || '';
    this.currentUserRole = this._authService.getRoles().join(', ') || 'Staff';
    const academicYearId = this._authService.getCurrentAcademicYearId();

    this.userInitials = this.currentUserName
      .split(' ')
      .filter(x => !!x)
      .slice(0, 2)
      .map(x => x.charAt(0).toUpperCase())
      .join('') || 'U';

    if (academicYearId) {
      this._lookupService.getAcademicYearById(academicYearId).subscribe({
        next: academicYear => {
          this.currentAcademicYearName = academicYear?.yearName ?? '';
        }
      });
    }
  }

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }

  onAvatarImageError(): void {
    this.userAvatarImage = '';
  }

  logout(): void {
    this._authService.logout();
  }

  onAvatarActionClick(item: MenuItem): void {
    if (item.command) {
      item.command({ item, originalEvent: new Event('click') });
    }
  }

  getRouterLink(item: MenuItem): string | any[] | null {
    return item['routerLink'] as string | any[] | null;
  }

  isGroupActive(item: MenuItem): boolean {
    return (item.items ?? []).some(child => this.isItemActive(child));
  }

  isItemActive(item: MenuItem): boolean {
    const routerLink = this.getRouterLink(item);
    if (!routerLink) {
      return false;
    }

    const commands = Array.isArray(routerLink) ? routerLink : [routerLink];
    const urlTree = this._router.createUrlTree(commands);
    return this._router.isActive(urlTree, {
      paths: 'subset',
      queryParams: 'ignored',
      fragment: 'ignored',
      matrixParams: 'ignored'
    });
  }

  private buildSidebar(): void {
    this.sidebarItems = this.filterMenuByPermissions([
      {
        label: 'Dashboard',
        icon: 'pi pi-home',
        routerLink: ['/home/dashboard']
      },
      {
        label: 'Setup',
        icon: 'pi pi-cog',
        items: [
          { label: 'Academic Year', icon: 'pi pi-calendar-plus', routerLink: ['/home/master-entry/add-academic-year'], data: { permissions: [PermissionNames.AcademicYearManage] } }
        ]
      },
      {
        label: 'Classes',
        icon: 'pi pi-sitemap',
        items: [
          { label: 'Class Entry', icon: 'pi pi-plus-circle', routerLink: ['/home/master-entry/add-class'], data: { permissions: [PermissionNames.ClassManage] } },
          { label: 'Section Entry', icon: 'pi pi-tags', routerLink: ['/home/master-entry/add-section'], data: { permissions: [PermissionNames.ClassManage] } },
          { label: 'Manage Classes', icon: 'pi pi-list-check', routerLink: ['/home/class/manage-class'], data: { permissions: [PermissionNames.ClassManage] } }
        ]
      },
      {
        label: 'Subjects & Courses',
        icon: 'pi pi-book',
        items: [
          { label: 'Course Entry', icon: 'pi pi-bookmark', routerLink: ['/home/master-entry/add-course'], data: { permissions: [PermissionNames.CourseManage] } },
          { label: 'Manage Courses', icon: 'pi pi-table', routerLink: ['/home/course/manage-course'], data: { permissions: [PermissionNames.CourseManage] } }
        ]
      },
      {
        label: 'Students',
        icon: 'pi pi-users',
        items: [
          { label: 'All Students', icon: 'pi pi-users', routerLink: ['/home/student/list-student'], data: { permissions: [PermissionNames.StudentView] } },
          { label: 'Add Student', icon: 'pi pi-user-plus', routerLink: ['/home/student/add-student'], data: { permissions: [PermissionNames.StudentCreate] } },
          { label: 'Promotion Preview', icon: 'pi pi-arrow-right-arrow-left', routerLink: ['/home/student/promotion-preview'], data: { permissions: [PermissionNames.StudentUpdate] } }
        ]
      },
      {
        label: 'Teachers',
        icon: 'pi pi-user-edit',
        items: [
          { label: 'All Teachers', icon: 'pi pi-id-card', routerLink: ['/home/teacher/list-teacher'], data: { permissions: [PermissionNames.TeacherView] } },
          { label: 'Subject Marks Entry', icon: 'pi pi-pencil', routerLink: ['/home/teacher/marks-entry'], data: { permissions: [PermissionNames.SubjectMarksEntry] } }
        ]
      },
      {
        label: 'Attendance',
        icon: 'pi pi-calendar-clock',
        items: [
          {
            label: 'Student Entry',
            icon: 'pi pi-user-check',
            routerLink: ['/home/attendance/student-entry'],
            data: { permissions: [PermissionNames.StudentAttendanceView, PermissionNames.StudentAttendanceTake, PermissionNames.StudentAttendanceEdit] }
          },
          {
            label: 'Student Reports',
            icon: 'pi pi-chart-bar',
            routerLink: ['/home/attendance/student-report'],
            data: { permissions: [PermissionNames.StudentAttendanceReport] }
          },
          {
            label: 'Teacher Entry',
            icon: 'pi pi-briefcase',
            routerLink: ['/home/attendance/teacher-entry'],
            data: { permissions: [PermissionNames.TeacherAttendanceView, PermissionNames.TeacherAttendanceTake, PermissionNames.TeacherAttendanceEdit, PermissionNames.TeacherAttendanceCheckInOut] }
          },
          {
            label: 'Teacher Reports',
            icon: 'pi pi-chart-line',
            routerLink: ['/home/attendance/teacher-report'],
            data: { permissions: [PermissionNames.TeacherAttendanceReport] }
          }
        ]
      },
      {
        label: 'Exams & Results',
        icon: 'pi pi-chart-bar',
        items: [
          { label: 'Student Exam Setup', icon: 'pi pi-sliders-h', routerLink: ['/home/exam/student-setup'], data: { permissions: [PermissionNames.ExamMarksEntry] } },
          { label: 'Exam Marks Entry', icon: 'pi pi-file-edit', routerLink: ['/home/exam/exam-marks-entry'], data: { permissions: [PermissionNames.ExamMarksEntry] } },
          { label: 'Results', icon: 'pi pi-chart-line', routerLink: ['/home/exam/exam-results'], data: { permissions: [PermissionNames.ResultView] } }
        ]
      },
      {
        label: 'Certificates',
        icon: 'pi pi-verified',
        items: [
          { label: 'Certificates', icon: 'pi pi-id-card', routerLink: ['/home/certificate/certificate'], data: { permissions: [PermissionNames.StudentView] } }
        ]
      },
      {
        label: 'Fees & Payments',
        icon: 'pi pi-wallet',
        items: [
          { label: 'Manage Fee Type', icon: 'pi pi-credit-card', routerLink: ['/home/fees/manage-fees'], data: { permissions: [PermissionNames.FeeCreate] } },
          { label: 'Fee Payment', icon: 'pi pi-money-bill', routerLink: ['/home/fees/fee-payment'], data: { permissions: [PermissionNames.FeeCreate] } },
          { label: 'Fee Reports', icon: 'pi pi-file', routerLink: ['/home/fees/reports'], data: { permissions: [PermissionNames.FeeView] } }
        ]
      },
      {
        label: 'Users & Permissions',
        icon: 'pi pi-shield',
        items: [
          { label: 'User Roles', icon: 'pi pi-user', routerLink: ['/home/security/user-roles'], data: { permissions: [PermissionNames.UserManage] } },
          { label: 'Role Management', icon: 'pi pi-lock', routerLink: ['/home/security/roles'], data: { permissions: [PermissionNames.RoleManage] } }
        ]
      }
    ]);
  }

  private buildAvatarMenu(): void {
    this.avatarMenuItems = [
      {
        label: 'Logout',
        icon: 'pi pi-sign-out',
        command: () => this.logout()
      }
    ];
  }

  private filterMenuByPermissions(items: MenuItem[]): MenuItem[] {
    return items
      .map(item => {
        const hadChildren = !!item.items?.length;
        const children = hadChildren ? this.filterMenuByPermissions(item.items ?? []) : undefined;
        const permissions = item['data']?.['permissions'] as string[] | undefined;
        const allowed = !permissions || this._authService.hasAnyPermission(permissions);

        if (!allowed && (!children || children.length === 0)) {
          return null;
        }

        if (hadChildren && (!children || children.length === 0)) {
          return null;
        }

        return {
          ...item,
          items: children
        } as MenuItem;
      })
      .filter((item): item is MenuItem => !!item);
  }
}
