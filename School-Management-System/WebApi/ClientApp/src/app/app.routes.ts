import { Routes } from '@angular/router';
import { ResultPreviewComponent } from './home/components/exam/result-preview/result-preview.component';

export const routes: Routes = [
    {path:'', redirectTo:'home', pathMatch:'full'},
    {path:'login', loadComponent:()=>import('./user/login/login.component').then(m=>m.LoginComponent)},
    {path:'create-user', loadComponent:()=>import('./user/create-user/create-user.component').then(m=>m.CreateUserComponent)},
    {path:'home', loadChildren: ()=> import('./home/home.module').then(m=>m.HomeModule)},
    { path: 'result-preview', component:ResultPreviewComponent}
];
