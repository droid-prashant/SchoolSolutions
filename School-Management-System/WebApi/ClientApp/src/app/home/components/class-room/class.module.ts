import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { SharedModule } from "../../../shared/shared.module";
import { HttpClientModule } from "@angular/common/http";
import { ClassRoutingModule } from "./class.routes";
import { ManageClassComponent } from "./manage-class/manage-class.component";


@NgModule({
    declarations: [
        ManageClassComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        ClassRoutingModule,
        SharedModule,
        FormsModule,
        HttpClientModule
    ],
    exports: [],
})
export class ClassModule { }