import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CharacterCertificateComponent } from './character-certificate/character-certificate.component';
import { TransferCertificateComponent } from './transfer-certificate/transfer-certificate/transfer-certificate.component';
import { CertificateComponent } from './certificate.component';
import { PermissionGuardService } from '../../../shared/permissionGuard.service';
import { PermissionNames } from '../../../shared/common/constants/permission-names';

const routes: Routes = [
    {path:'certificate', component:CertificateComponent, canActivate: [PermissionGuardService], data: { permissions: [PermissionNames.StudentView] }}]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CertificateRoutingModule { }
