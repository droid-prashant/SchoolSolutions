import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManageFeesTypesComponent } from './manage-fees-types/manage-fees-types.component';
import { FeePaymentComponent } from './fee-payment/fee-payment.component';
import { FeeReportComponent } from './fee-report/fee-report.component';
import { PermissionGuardService } from '../../../shared/permissionGuard.service';
import { PermissionNames } from '../../../shared/common/constants/permission-names';

const routes: Routes = [
    {path:'manage-fees', component:ManageFeesTypesComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.FeeCreate] }},
    {path:'fee-payment', component:FeePaymentComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.FeeCreate] }},
    {path:'reports', component:FeeReportComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.FeeView] }},
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class FeesRoutingModule { }
