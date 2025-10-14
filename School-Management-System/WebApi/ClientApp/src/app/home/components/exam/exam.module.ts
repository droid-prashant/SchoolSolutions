import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { RouterOutlet } from "@angular/router";
import { SharedModule } from "../../../shared/shared.module";
import { ExamMarksEntryComponent } from "./exam-marks-entry/exam-marks-entry.component";
import { ExamRoutingModule } from "./exam.routes";
import { ResultComponent } from "./result/result.component";
import { ResultPreviewComponent } from "./result-preview/result-preview.component";


@NgModule({
    declarations: [
        ExamMarksEntryComponent,
        ResultComponent,
        ResultPreviewComponent
    ],
    imports: [
        CommonModule,
        ExamRoutingModule,
        FormsModule,
        ReactiveFormsModule,
        RouterOutlet,
        SharedModule
    ],
    exports: [],
})
export class ExamModule { }