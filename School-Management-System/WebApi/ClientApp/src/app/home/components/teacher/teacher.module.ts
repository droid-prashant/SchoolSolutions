import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { SharedModule } from '../../../shared/shared.module';
import { TeacherRoutingModule } from './teacher.routes';
import { ListTeacherComponent } from './list-teacher/list-teacher.component';
import { AddTeacherComponent } from './add-teacher/add-teacher.component';
import { NpDatepickerComponent } from '../../../shared/calender/np-datepicker/np-datepicker.component';

@NgModule({
  declarations: [
    ListTeacherComponent,
    AddTeacherComponent
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
