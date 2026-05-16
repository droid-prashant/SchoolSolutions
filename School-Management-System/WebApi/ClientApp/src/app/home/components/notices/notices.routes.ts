import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PermissionGuardService } from '../../../shared/permissionGuard.service';
import { PermissionNames } from '../../../shared/common/constants/permission-names';
import { ManageNoticesComponent } from './manage-notices/manage-notices.component';

const routes: Routes = [
  {
    path: 'manage',
    component: ManageNoticesComponent,
    canActivate: [PermissionGuardService],
    data: { permissions: [PermissionNames.NoticeManage] }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class NoticesRoutingModule { }
