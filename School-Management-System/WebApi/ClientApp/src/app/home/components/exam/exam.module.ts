import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { RouterOutlet } from "@angular/router";
import { SharedModule } from "../../../shared/shared.module";
import { ExamMarksEntryComponent } from "./exam-marks-entry/exam-marks-entry.component";
import { ExamRoutingModule } from "./exam.routes";
import { ResultComponent } from "./result/result.component";
import { ResultPreviewComponent } from "./result-preview/result-preview.component";
import { StudentExamSetupComponent } from "./student-exam-setup/student-exam-setup.component";


@NgModule({
    declarations: [
        ExamMarksEntryComponent,
        ResultComponent,
        ResultPreviewComponent,
        StudentExamSetupComponent
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