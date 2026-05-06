import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PermissionNames } from '../../../shared/common/constants/permission-names';
import { PermissionGuardService } from '../../../shared/permissionGuard.service';
import { StudentAttendanceEntryComponent } from './student-attendance-entry/student-attendance-entry.component';
import { StudentAttendanceReportComponent } from './student-attendance-report/student-attendance-report.component';
import { TeacherAttendanceEntryComponent } from './teacher-attendance-entry/teacher-attendance-entry.component';
import { TeacherAttendanceReportComponent } from './teacher-attendance-report/teacher-attendance-report.component';

const routes: Routes = [
  {
    path: 'student-entry',
    component: StudentAttendanceEntryComponent,
    canActivate: [PermissionGuardService],
    data: { permissions: [PermissionNames.StudentAttendanceView, PermissionNames.StudentAttendanceTake, PermissionNames.StudentAttendanceEdit] }
  },
  {
    path: 'student-report',
    component: StudentAttendanceReportComponent,
    canActivate: [PermissionGuardService],
    data: { permissions: [PermissionNames.StudentAttendanceReport] }
  },
  {
    path: 'teacher-entry',
    component: TeacherAttendanceEntryComponent,
    canActivate: [PermissionGuardService],
    data: { permissions: [PermissionNames.TeacherAttendanceView, PermissionNames.TeacherAttendanceTake, PermissionNames.TeacherAttendanceEdit, PermissionNames.TeacherAttendanceCheckInOut] }
  },
  {
    path: 'teacher-report',
    component: TeacherAttendanceReportComponent,
    canActivate: [PermissionGuardService],
    data: { permissions: [PermissionNames.TeacherAttendanceReport] }
  },
  { path: '', redirectTo: 'student-entry', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AttendanceRoutingModule {}
