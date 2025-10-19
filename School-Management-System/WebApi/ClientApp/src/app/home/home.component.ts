import { Component, OnInit } from '@angular/core';
import { MenuItem, PrimeNGConfig } from 'primeng/api';
import { PrimeIcons } from 'primeng/api';


@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})

export class HomeComponent implements OnInit {
  items: MenuItem[] | undefined;
  sidebarItems: MenuItem[] | undefined;

  constructor() {

  }
  ngOnInit(): void {
    this.items = [
      {
        label: 'Dashboard',
        icon: PrimeIcons.HOME
      },
      {
        label: 'Features',
        icon: PrimeIcons.STAR
      },
      {
        label: 'Projects',
        icon: PrimeIcons.SEARCH,
      },
      {
        label: 'Contact',
        icon: PrimeIcons.ENVELOPE,
        badge: '3'
      }
    ];
    this.sidebarItems = [
      {
        label: 'Master Entry',
        icon: PrimeIcons.USERS,
        items: [
          { label: 'Class Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/add-class'] },
          { label: 'Section Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/add-section'] },
          { label: 'Course Entry', icon: PrimeIcons.PLUS, routerLink: ['/home/master-entry/add-course'] }
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
          { label: 'Add Student', icon: PrimeIcons.PLUS, routerLink: ['/home/student/add-student'] },
          { label: 'Promotions', icon: PrimeIcons.ARROW_RIGHT_ARROW_LEFT, routerLink: ['/students/promotions'] }
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
          { label: 'Character Certificate', icon: PrimeIcons.USERS, routerLink: ['/home/certificate/character-certificate'] },
          { label: 'Transfer Certificate', icon: PrimeIcons.USERS, routerLink: ['/home/certificate/transfer-certificate'] }
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
          { label: 'Fee Collection', icon: PrimeIcons.CREDIT_CARD, routerLink: ['/fees/collection'] },
          { label: 'Fee Reports', icon: PrimeIcons.FILE, routerLink: ['/fees/reports'] }
        ]
      },
      {
        label: 'Settings',
        icon: PrimeIcons.COG,
        routerLink: ['/settings']
      }
    ];
  }
}
