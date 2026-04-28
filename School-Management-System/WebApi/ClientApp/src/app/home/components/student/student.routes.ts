import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListStudentComponent } from './list-student/list-student.component';
import { AddStudentComponent } from './add-student/add-student.component';
import { PromotionPreviewComponent } from './promotion-preview/promotion-preview.component';

const routes: Routes = [
    {path:'list-student', component:ListStudentComponent},
    {path:'add-student', component:AddStudentComponent},
    {path:'promotion-preview', component:PromotionPreviewComponent}
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class StudentRoutingModule { }
