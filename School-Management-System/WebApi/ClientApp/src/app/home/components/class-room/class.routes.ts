import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManageClassComponent } from './manage-class/manage-class.component';

const routes: Routes = [
    { path: 'manage-class', component: ManageClassComponent, pathMatch: 'full' },
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ClassRoutingModule { }
