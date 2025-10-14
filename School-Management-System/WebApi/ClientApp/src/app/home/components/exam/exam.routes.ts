import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExamMarksEntryComponent } from './exam-marks-entry/exam-marks-entry.component';
import { ResultComponent } from './result/result.component';
import { ResultPreviewComponent } from './result-preview/result-preview.component';

const routes: Routes = [
    {path:'exam-marks-entry', component:ExamMarksEntryComponent},
    {path:'exam-results', component:ResultComponent},
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ExamRoutingModule { }
