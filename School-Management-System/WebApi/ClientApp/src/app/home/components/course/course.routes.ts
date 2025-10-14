import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManageCourseComponent } from './manage-course/manage-course.component';

const routes: Routes = [
    {path:'manage-course', component:ManageCourseComponent},
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CourseRoutingModule { }
