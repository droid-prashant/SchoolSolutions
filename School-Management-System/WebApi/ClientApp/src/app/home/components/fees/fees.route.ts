import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManageFeesTypesComponent } from './manage-fees-types/manage-fees-types.component';
import { FeePaymentComponent } from './fee-payment/fee-payment.component';
import { FeeReportComponent } from './fee-report/fee-report.component';

const routes: Routes = [
    {path:'manage-fees', component:ManageFeesTypesComponent},
    {path:'fee-payment', component:FeePaymentComponent},
    {path:'reports', component:FeeReportComponent},
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class FeesRoutingModule { }
