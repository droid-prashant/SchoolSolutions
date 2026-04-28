import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { RouterOutlet } from "@angular/router";
import { SharedModule } from "../../../shared/shared.module";
import { ManageFeesTypesComponent } from "./manage-fees-types/manage-fees-types.component";
import { FeesRoutingModule } from "./fees.route";
import { FeePaymentComponent } from "./fee-payment/fee-payment.component";
import { FeeReportComponent } from "./fee-report/fee-report.component";


@NgModule({
    declarations: [
        ManageFeesTypesComponent,
        FeePaymentComponent,
        FeeReportComponent
    ],
    imports: [
        CommonModule,
        FeesRoutingModule,
        FormsModule,
        ReactiveFormsModule,
        RouterOutlet,
        SharedModule
    ],
    exports: [],
})
export class FeesModule { }
