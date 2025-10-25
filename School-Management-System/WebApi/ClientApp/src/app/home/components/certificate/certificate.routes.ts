import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CharacterCertificateComponent } from './character-certificate/character-certificate.component';
import { TransferCertificateComponent } from './transfer-certificate/transfer-certificate/transfer-certificate.component';
import { CertificateComponent } from './certificate.component';

const routes: Routes = [
    {path:'certificate', component:CertificateComponent}]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CertificateRoutingModule { }
