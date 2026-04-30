import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PermissionNames } from '../../../shared/common/constants/permission-names';
import { PermissionGuardService } from '../../../shared/permissionGuard.service';
import { RoleManagementComponent } from './role-management/role-management.component';
import { UserRoleAssignmentComponent } from './user-role-assignment/user-role-assignment.component';

const routes: Routes = [
  { path: 'roles', component: RoleManagementComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.RoleManage] } },
  { path: 'user-roles', component: UserRoleAssignmentComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.UserManage] } }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SecurityRoutingModule { }
