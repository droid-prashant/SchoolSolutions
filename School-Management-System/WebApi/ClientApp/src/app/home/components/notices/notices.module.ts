import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterOutlet } from '@angular/router';
import { SharedModule } from '../../../shared/shared.module';
import { NpDatepickerComponent } from '../../../shared/calender/np-datepicker/np-datepicker.component';
import { NoticesRoutingModule } from './notices.routes';
import { ManageNoticesComponent } from './manage-notices/manage-notices.component';

@NgModule({
  declarations: [
    ManageNoticesComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterOutlet,
    NpDatepickerComponent,
    SharedModule,
    NoticesRoutingModule
  ]
})
export class NoticesModule { }
