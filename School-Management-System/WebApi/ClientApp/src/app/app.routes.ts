import { Routes } from '@angular/router';
import { ResultPreviewComponent } from './home/components/exam/result-preview/result-preview.component';
import { AuthGuardService } from './shared/authGuard.service';
import { PermissionGuardService } from './shared/permissionGuard.service';
import { PermissionNames } from './shared/common/constants/permission-names';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', loadComponent: () => import('./user/login/login.component').then(m => m.LoginComponent)},
    { path: 'unauthorized', loadComponent: () => import('./shared/unauthorized/unauthorized.component').then(m => m.UnauthorizedComponent) },
    { path: 'create-user', loadComponent: () => import('./user/create-user/create-user.component').then(m => m.CreateUserComponent), canActivate: [PermissionGuardService], canMatch: [PermissionGuardService], data: { permissions: [PermissionNames.UserManage] } },
    { path: 'home', loadChildren: () => import('./home/home.module').then(m => m.HomeModule), canActivate: [AuthGuardService], canMatch: [AuthGuardService] },
    { path: 'result-preview', component: ResultPreviewComponent, canActivate: [PermissionGuardService], canMatch: [PermissionGuardService], data: { permissions: [PermissionNames.ResultView] } }
];
