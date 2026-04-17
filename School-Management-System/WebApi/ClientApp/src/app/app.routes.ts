import { Routes } from '@angular/router';
import { ResultPreviewComponent } from './home/components/exam/result-preview/result-preview.component';
import { AuthGuardService } from './shared/authGuard.service';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', loadComponent: () => import('./user/login/login.component').then(m => m.LoginComponent)},
    { path: 'create-user', loadComponent: () => import('./user/create-user/create-user.component').then(m => m.CreateUserComponent), canActivate: [AuthGuardService], canMatch: [AuthGuardService] },
    { path: 'home', loadChildren: () => import('./home/home.module').then(m => m.HomeModule), canActivate: [AuthGuardService], canMatch: [AuthGuardService] },
    { path: 'result-preview', component: ResultPreviewComponent, canActivate: [AuthGuardService], canMatch: [AuthGuardService] }
];
