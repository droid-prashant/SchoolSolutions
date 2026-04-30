import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManageClassComponent } from './manage-class/manage-class.component';
import { PermissionGuardService } from '../../../shared/permissionGuard.service';
import { PermissionNames } from '../../../shared/common/constants/permission-names';

const routes: Routes = [
    { path: 'manage-class', component: ManageClassComponent, pathMatch: 'full', canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.ClassManage] } },
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ClassRoutingModule { }
