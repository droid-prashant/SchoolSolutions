import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExamMarksEntryComponent } from './exam-marks-entry/exam-marks-entry.component';
import { ResultComponent } from './result/result.component';
import { ResultPreviewComponent } from './result-preview/result-preview.component';
import { StudentExamSetupComponent } from './student-exam-setup/student-exam-setup.component';
import { PermissionGuardService } from '../../../shared/permissionGuard.service';
import { PermissionNames } from '../../../shared/common/constants/permission-names';

const routes: Routes = [
    {path:'exam-marks-entry', component:ExamMarksEntryComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.ExamMarksEntry] }},
    {path:'exam-results', component:ResultComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.ResultView] }},
    {path:'student-setup', component:StudentExamSetupComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.ExamMarksEntry] }},
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ExamRoutingModule { }
