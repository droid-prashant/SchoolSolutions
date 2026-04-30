import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListTeacherComponent } from './list-teacher/list-teacher.component';
import { TeacherAssignmentsComponent } from './teacher-assignments/teacher-assignments.component';
import { TeacherAccountComponent } from './teacher-account/teacher-account.component';
import { TeacherDetailComponent } from './teacher-detail/teacher-detail.component';
import { TeacherDocumentsComponent } from './teacher-documents/teacher-documents.component';
import { TeacherMarksEntryComponent } from './teacher-marks-entry/teacher-marks-entry.component';
import { PermissionGuardService } from '../../../shared/permissionGuard.service';
import { PermissionNames } from '../../../shared/common/constants/permission-names';

const routes: Routes = [
  { path: 'list-teacher', component: ListTeacherComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.TeacherView] } },
  { path: 'assignments/:teacherId', component: TeacherAssignmentsComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.TeacherManage] } },
  { path: 'details/:teacherId', component: TeacherDetailComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.TeacherView] } },
  { path: 'documents/:teacherId', component: TeacherDocumentsComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.TeacherView] } },
  { path: 'user/:teacherId', component: TeacherAccountComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.UserManage] } },
  { path: 'marks-entry', component: TeacherMarksEntryComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.ExamMarksEntry] } }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeacherRoutingModule { }
