import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListStudentComponent } from './list-student/list-student.component';
import { AddStudentComponent } from './add-student/add-student.component';
import { PromotionPreviewComponent } from './promotion-preview/promotion-preview.component';
import { PermissionGuardService } from '../../../shared/permissionGuard.service';
import { PermissionNames } from '../../../shared/common/constants/permission-names';

const routes: Routes = [
    {path:'list-student', component:ListStudentComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.StudentView] }},
    {path:'add-student', component:AddStudentComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.StudentCreate] }},
    {path:'promotion-preview', component:PromotionPreviewComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.StudentUpdate] }}
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class StudentRoutingModule { }
