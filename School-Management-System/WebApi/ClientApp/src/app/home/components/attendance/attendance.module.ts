import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NpDatepickerComponent } from '../../../shared/calender/np-datepicker/np-datepicker.component';
import { SharedModule } from '../../../shared/shared.module';
import { AttendanceRoutingModule } from './attendance.routes';
import { StudentAttendanceEntryComponent } from './student-attendance-entry/student-attendance-entry.component';
import { StudentAttendanceReportComponent } from './student-attendance-report/student-attendance-report.component';
import { TeacherAttendanceEntryComponent } from './teacher-attendance-entry/teacher-attendance-entry.component';
import { TeacherAttendanceReportComponent } from './teacher-attendance-report/teacher-attendance-report.component';

@NgModule({
  declarations: [
    StudentAttendanceEntryComponent,
    StudentAttendanceReportComponent,
    TeacherAttendanceEntryComponent,
    TeacherAttendanceReportComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    SharedModule,
    NpDatepickerComponent,
    AttendanceRoutingModule
  ]
})
export class AttendanceModule {}
