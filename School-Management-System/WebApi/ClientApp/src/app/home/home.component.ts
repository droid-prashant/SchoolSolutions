import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem, PrimeNGConfig } from 'primeng/api';
import { PrimeIcons } from 'primeng/api';
import { AuthService } from '../shared/auth.service';


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
  userInitials = 'U';
  userAvatarImage = 'assets/Om Pushpanjali logo.png';

  constructor(
    private _router: Router,
    private _authService: AuthService
  ) {

  }
  ngOnInit(): void {
    this.loadCurrentUser();
    this.items = [
      {
        label: 'Dashboard',
        icon: PrimeIcons.HOME,
        command: () => this.redirectToDashboard()
      },
      {
        label: 'Settings',
        icon: PrimeIcons.COG,
        items:[
          {label:'Users', icon:PrimeIcons.USER},
          {label:'Roles', icon:PrimeIcons.SHIELD}
        ]
      }
    ]
    this.sidebarItems = [
      {
        label: 'Master Entry',
        icon: PrimeIcons.USERS,
        items: [
          { label: 'Academic Year Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/add-academic-year'] },
          { label: 'Class Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/add-class'] },
          { label: 'Section Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/add-section'] },
          { label: 'Course Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/add-course'] },
          { label: 'Fee Type Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/fee-type'] }
        ]
      },
      {
        label: 'Class Management',
        icon: PrimeIcons.LIST,
        items: [
          { label: 'Manage Class', icon: PrimeIcons.BARS, routerLink: ['/home/class/manage-class'] }
        ]
      },
      {
        label: 'Course Management',
        icon: PrimeIcons.BOOK,
        items: [
          { label: 'Manage Courses', icon: PrimeIcons.BOOKMARK, routerLink: ['/home/course/manage-course'] },
        ]
      },
      {
        label: 'Student Section',
        icon: PrimeIcons.USER,
        items: [
          { label: 'All Students', icon: PrimeIcons.USERS, routerLink: ['/home/student/list-student'] },
        ]
      },
      {
        label: 'Teacher Section',
        icon: PrimeIcons.USER_EDIT,
        items: [
          { label: 'All Teachers', icon: PrimeIcons.USERS, routerLink: ['/home/teachers'] },
          { label: 'Add Teacher', icon: PrimeIcons.PLUS, routerLink: ['/home/teachers/add'] }
        ]
      },
      {
        label: 'Exams',
        icon: PrimeIcons.CALENDAR,
        items: [
          { label: 'Exam Schedule', icon: PrimeIcons.CLOCK, routerLink: ['/exam/schedule'] },
          { label: 'Student Exam Setup', icon: PrimeIcons.COG, routerLink: ['/home/exam/student-setup'] },
          { label: 'Exam Marks Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/exam/exam-marks-entry'] },
          { label: 'Results', icon: PrimeIcons.CHART_BAR, routerLink: ['/home/exam/exam-results'] },
          // { label: 'Results', icon: PrimeIcons.CHART_BAR, routerLink: ['/home/exam/preview-result'] }
        ]
      },
      {
        label: 'Certificates',
        icon: PrimeIcons.CREDIT_CARD,
        items: [
          { label: 'Certificate', icon: PrimeIcons.ID_CARD, routerLink: ['/home/certificate/certificate'] },
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
          { label: 'Manage Fee Type', icon: PrimeIcons.CREDIT_CARD, routerLink: ['/home/fees/manage-fees'] },
          { label: 'Fee Payment', icon: PrimeIcons.MONEY_BILL, routerLink: ['/home/fees/fee-payment'] },
          { label: 'Fee Reports', icon: PrimeIcons.FILE, routerLink: ['/fees/reports'] }
        ]
      }
    ];

    this.avatarMenuItems = [
      {
        label: 'Dashboard',
        icon: PrimeIcons.HOME,
        command: () => this.redirectToDashboard()
      },
      {
        label: 'Academic Year Entry',
        icon: PrimeIcons.CALENDAR,
        command: () => this._router.navigateByUrl('/home/master-entry/add-academic-year')
      },
      {
        label: 'Manage Courses',
        icon: PrimeIcons.BOOKMARK,
        command: () => this._router.navigateByUrl('/home/course/manage-course')
      },
      {
        separator: true
      },
      {
        label: 'Logout',
        icon: PrimeIcons.SIGN_OUT,
        command: () => this.logout()
      }
    ];
  }

  loadCurrentUser(): void {
    const decodedToken = this._authService.decodeToken();
    this.currentUserName = decodedToken?.name || 'User';
    this.currentUserEmail = decodedToken?.email || '';
    this.currentUserRole = decodedToken?.permission || 'Staff';

    this.userInitials = this.currentUserName
      .split(' ')
      .filter(x => !!x)
      .slice(0, 2)
      .map(x => x.charAt(0).toUpperCase())
      .join('') || 'U';
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
}
