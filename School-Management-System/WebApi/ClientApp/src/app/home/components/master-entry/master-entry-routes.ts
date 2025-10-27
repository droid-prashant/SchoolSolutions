import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClassEntryComponent } from './class-entry/class-entry.component';
import { SectionEntryComponent } from './section-entry/section-entry.component';
import { CourseEntryComponent } from './course-entry/course-entry.component';
import { FeeTypeComponent } from './fee-type/fee-type.component';
import { AcademicYearEntryComponent } from './academic-year-entry/academic-year-entry.component';

const routes: Routes = [
    {path:'add-academic-year', component:AcademicYearEntryComponent, pathMatch:'full'},
    {path:'add-class', component:ClassEntryComponent, pathMatch:'full'},
    {path:'add-section', component:SectionEntryComponent, pathMatch:'full'},
    {path:'add-course', component:CourseEntryComponent, pathMatch:'full'},
    {path:'fee-type', component:FeeTypeComponent, pathMatch:'full'}
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class MasterEntryRoutingModule { }
