import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem, PrimeNGConfig } from 'primeng/api';
import { PrimeIcons } from 'primeng/api';
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
  items: MenuItem[] | undefined;
  sidebarItems: MenuItem[] | undefined;
  avatarMenuItems: MenuItem[] | undefined;
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
  ) {

  }
  ngOnInit(): void {
    this.loadCurrentUser();
    this.items = this.filterMenuByPermissions([
      {
        label: 'Dashboard',
        icon: PrimeIcons.HOME,
        command: () => this.redirectToDashboard()
      },
      {
        label: 'Settings',
        icon: PrimeIcons.COG,
        items:[
          {label:'Users', icon:PrimeIcons.USER, routerLink: ['/home/security/user-roles'], data: { permissions: [PermissionNames.UserManage] }},
          {label:'Roles', icon:PrimeIcons.SHIELD, routerLink: ['/home/security/roles'], data: { permissions: [PermissionNames.RoleManage] }}
        ]
      }
    ]);
    this.sidebarItems = this.filterMenuByPermissions([
      {
        label: 'Master Entry',
        icon: PrimeIcons.USERS,
        items: [
          { label: 'Academic Year Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/add-academic-year'], data: { permissions: [PermissionNames.AcademicYearManage] } },
          { label: 'Class Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/add-class'], data: { permissions: [PermissionNames.ClassManage] } },
          { label: 'Section Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/add-section'], data: { permissions: [PermissionNames.ClassManage] } },
          { label: 'Course Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/add-course'], data: { permissions: [PermissionNames.CourseManage] } },
          { label: 'Fee Type Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/fee-type'], data: { permissions: [PermissionNames.FeeCreate] } }
        ]
      },
      {
        label: 'Class Management',
        icon: PrimeIcons.LIST,
        items: [
          { label: 'Manage Class', icon: PrimeIcons.BARS, routerLink: ['/home/class/manage-class'], data: { permissions: [PermissionNames.ClassManage] } }
        ]
      },
      {
        label: 'Course Management',
        icon: PrimeIcons.BOOK,
        items: [
          { label: 'Manage Courses', icon: PrimeIcons.BOOKMARK, routerLink: ['/home/course/manage-course'], data: { permissions: [PermissionNames.CourseManage] } },
        ]
      },
      {
        label: 'Student Section',
        icon: PrimeIcons.USER,
        items: [
          { label: 'All Students', icon: PrimeIcons.USERS, routerLink: ['/home/student/list-student'], data: { permissions: [PermissionNames.StudentView] } },
          { label: 'Add Student', icon: PrimeIcons.PLUS, routerLink: ['/home/student/add-student'], data: { permissions: [PermissionNames.StudentCreate] } },
          { label: 'Promotion Preview', icon: PrimeIcons.ARROW_RIGHT_ARROW_LEFT, routerLink: ['/home/student/promotion-preview'], data: { permissions: [PermissionNames.StudentUpdate] } },
        ]
      },
      {
        label: 'Teacher Section',
        icon: PrimeIcons.USER_EDIT,
        items: [
          { label: 'All Teachers', icon: PrimeIcons.USERS, routerLink: ['/home/teacher/list-teacher'], data: { permissions: [PermissionNames.TeacherView] } },
          { label: 'Subject Marks Entry', icon: PrimeIcons.PENCIL, routerLink: ['/home/teacher/marks-entry'], data: { permissions: [PermissionNames.ExamMarksEntry] } }
        ]
      },
      {
        label: 'Exams',
        icon: PrimeIcons.CALENDAR,
        items: [
          { label: 'Student Exam Setup', icon: PrimeIcons.COG, routerLink: ['/home/exam/student-setup'], data: { permissions: [PermissionNames.ExamMarksEntry] } },
          { label: 'Exam Marks Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/exam/exam-marks-entry'], data: { permissions: [PermissionNames.ExamMarksEntry] } },
          { label: 'Results', icon: PrimeIcons.CHART_BAR, routerLink: ['/home/exam/exam-results'], data: { permissions: [PermissionNames.ResultView] } },
          // { label: 'Results', icon: PrimeIcons.CHART_BAR, routerLink: ['/home/exam/preview-result'] }
        ]
      },
      {
        label: 'Certificates',
        icon: PrimeIcons.CREDIT_CARD,
        items: [
          { label: 'Certificate', icon: PrimeIcons.ID_CARD, routerLink: ['/home/certificate/certificate'], data: { permissions: [PermissionNames.StudentView] } },
        ]
      },
      {
        label: 'Attendance',
        icon: PrimeIcons.CHECK_SQUARE,
        items: [
          { label: 'Student Attendance', icon: PrimeIcons.USERS, routerLink: ['/attendance/students'] },
          { label: 'Teacher Attendance', icon: PrimeIcons.USER, routerLink: ['/attendance/teachers'] }
        ]
      },
      {
        label: 'Fees',
        icon: PrimeIcons.MONEY_BILL,
        items: [
          { label: 'Manage Fee Type', icon: PrimeIcons.CREDIT_CARD, routerLink: ['/home/fees/manage-fees'], data: { permissions: [PermissionNames.FeeCreate] } },
          { label: 'Fee Payment', icon: PrimeIcons.MONEY_BILL, routerLink: ['/home/fees/fee-payment'], data: { permissions: [PermissionNames.FeeCreate] } },
          { label: 'Fee Reports', icon: PrimeIcons.FILE, routerLink: ['/home/fees/reports'], data: { permissions: [PermissionNames.FeeView] } }
        ]
      },
      {
        label: 'Security',
        icon: PrimeIcons.SHIELD,
        items: [
          { label: 'Role Management', icon: PrimeIcons.SHIELD, routerLink: ['/home/security/roles'], data: { permissions: [PermissionNames.RoleManage] } },
          { label: 'User Roles', icon: PrimeIcons.USER, routerLink: ['/home/security/user-roles'], data: { permissions: [PermissionNames.UserManage] } }
        ]
      }
    ]);

    this.avatarMenuItems = this.filterMenuByPermissions([
      {
        label: 'Dashboard',
        icon: PrimeIcons.HOME,
        command: () => this.redirectToDashboard()
      },
      {
        label: 'Academic Year Entry',
        icon: PrimeIcons.CALENDAR,
        command: () => this._router.navigateByUrl('/home/master-entry/add-academic-year'),
        data: { permissions: [PermissionNames.AcademicYearManage] }
      },
      {
        label: 'Manage Courses',
        icon: PrimeIcons.BOOKMARK,
        command: () => this._router.navigateByUrl('/home/course/manage-course'),
        data: { permissions: [PermissionNames.CourseManage] }
      },
      {
        separator: true
      },
      {
        label: 'Logout',
        icon: PrimeIcons.SIGN_OUT,
        command: () => this.logout()
      }
    ]);
  }

  loadCurrentUser(): void {
    const decodedToken = this._authService.decodeToken();
    this.currentUserName = decodedToken?.name || 'User';
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
        next: (academicYear) => {
          this.currentAcademicYearName = academicYear?.yearName ?? '';
        }
      });
    }
  }

  onAvatarImageError(): void {
    this.userAvatarImage = '';
  }

  redirectToDashboard() {
    this._router.navigateByUrl('/home/dashboard');
  }

  logout(): void {
    this._authService.logout();
  }

  onAvatarActionClick(item: MenuItem): void {
    if (item.command) {
      item.command({ item, originalEvent: new Event('click') });
    }
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
