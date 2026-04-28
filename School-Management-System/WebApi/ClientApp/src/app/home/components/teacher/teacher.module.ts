import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { SharedModule } from '../../../shared/shared.module';
import { TeacherRoutingModule } from './teacher.routes';
import { ListTeacherComponent } from './list-teacher/list-teacher.component';
import { AddTeacherComponent } from './add-teacher/add-teacher.component';
import { TeacherAccountComponent } from './teacher-account/teacher-account.component';
import { TeacherAssignmentsComponent } from './teacher-assignments/teacher-assignments.component';
import { TeacherDetailComponent } from './teacher-detail/teacher-detail.component';
import { TeacherDocumentsComponent } from './teacher-documents/teacher-documents.component';
import { NpDatepickerComponent } from '../../../shared/calender/np-datepicker/np-datepicker.component';

@NgModule({
  declarations: [
    ListTeacherComponent,
    AddTeacherComponent,
    TeacherAccountComponent,
    TeacherAssignmentsComponent,
    TeacherDetailComponent,
    TeacherDocumentsComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    SharedModule,
    NpDatepickerComponent,
    TeacherRoutingModule
  ]
})
export class TeacherModule { }
