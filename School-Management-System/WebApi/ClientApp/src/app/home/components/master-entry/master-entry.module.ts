import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { RouterOutlet } from "@angular/router";
import { SharedModule } from "../../../shared/shared.module";
import { MasterEntryRoutingModule } from "./master-entry-routes";
import { ClassEntryComponent } from "./class-entry/class-entry.component";
import { CourseEntryComponent } from "./course-entry/course-entry.component";
import { SectionEntryComponent } from "./section-entry/section-entry.component";
import { FeeTypeComponent } from "./fee-type/fee-type.component";
import { AcademicYearEntryComponent } from "./academic-year-entry/academic-year-entry.component";


@NgModule({
    declarations: [
        ClassEntryComponent,
        CourseEntryComponent,
        SectionEntryComponent,
        FeeTypeComponent,
        AcademicYearEntryComponent
    ],
    imports: [
        CommonModule,
        MasterEntryRoutingModule,
        FormsModule,
        ReactiveFormsModule,
        RouterOutlet,
        SharedModule
    ],
    exports: [],
})
export class MasterEntryModule { }