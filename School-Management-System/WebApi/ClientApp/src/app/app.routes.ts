import { Routes } from '@angular/router';
import { ResultPreviewComponent } from './home/components/exam/result-preview/result-preview.component';

export const routes: Routes = [
    {path:'', redirectTo:'home', pathMatch:'full'},
    {path:'home', loadChildren: ()=> import('./home/home.module').then(m=>m.HomeModule)},
    { path: 'result-preview', component:ResultPreviewComponent}
];
