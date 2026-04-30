import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClassEntryComponent } from './class-entry/class-entry.component';
import { SectionEntryComponent } from './section-entry/section-entry.component';
import { CourseEntryComponent } from './course-entry/course-entry.component';
import { FeeTypeComponent } from './fee-type/fee-type.component';
import { AcademicYearEntryComponent } from './academic-year-entry/academic-year-entry.component';
import { PermissionGuardService } from '../../../shared/permissionGuard.service';
import { PermissionNames } from '../../../shared/common/constants/permission-names';

const routes: Routes = [
    {path:'add-academic-year', component:AcademicYearEntryComponent, pathMatch:'full', canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.AcademicYearManage] }},
    {path:'add-class', component:ClassEntryComponent, pathMatch:'full', canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.ClassManage] }},
    {path:'add-section', component:SectionEntryComponent, pathMatch:'full', canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.ClassManage] }},
    {path:'add-course', component:CourseEntryComponent, pathMatch:'full', canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.CourseManage] }},
    {path:'fee-type', component:FeeTypeComponent, pathMatch:'full', canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.FeeCreate] }}
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class MasterEntryRoutingModule { }
