import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManageCourseComponent } from './manage-course/manage-course.component';
import { PermissionGuardService } from '../../../shared/permissionGuard.service';
import { PermissionNames } from '../../../shared/common/constants/permission-names';

const routes: Routes = [
    {path:'manage-course', component:ManageCourseComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.CourseManage] }},
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CourseRoutingModule { }
