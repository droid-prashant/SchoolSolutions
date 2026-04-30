import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListTeacherComponent } from './list-teacher/list-teacher.component';
import { TeacherAssignmentsComponent } from './teacher-assignments/teacher-assignments.component';
import { TeacherAccountComponent } from './teacher-account/teacher-account.component';
import { TeacherDetailComponent } from './teacher-detail/teacher-detail.component';
import { TeacherDocumentsComponent } from './teacher-documents/teacher-documents.component';
import { TeacherMarksEntryComponent } from './teacher-marks-entry/teacher-marks-entry.component';

const routes: Routes = [
  { path: 'list-teacher', component: ListTeacherComponent },
  { path: 'assignments/:teacherId', component: TeacherAssignmentsComponent },
  { path: 'details/:teacherId', component: TeacherDetailComponent },
  { path: 'documents/:teacherId', component: TeacherDocumentsComponent },
  { path: 'user/:teacherId', component: TeacherAccountComponent },
  { path: 'marks-entry', component: TeacherMarksEntryComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeacherRoutingModule { }
