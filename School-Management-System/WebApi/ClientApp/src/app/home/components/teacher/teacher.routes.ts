import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListTeacherComponent } from './list-teacher/list-teacher.component';

const routes: Routes = [
  { path: 'list-teacher', component: ListTeacherComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeacherRoutingModule { }
