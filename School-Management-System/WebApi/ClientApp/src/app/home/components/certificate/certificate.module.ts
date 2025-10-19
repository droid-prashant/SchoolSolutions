import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { RouterOutlet } from "@angular/router";
import { SharedModule } from "../../../shared/shared.module";
import { CertificateRoutingModule } from "./certificate.routes";
import { CharacterCertificateComponent } from "./character-certificate/character-certificate.component";
import { TransferCertificateComponent } from "./transfer-certificate/transfer-certificate/transfer-certificate.component";
import { CertificateComponent } from "./certificate.component";


@NgModule({
    declarations: [
        CharacterCertificateComponent,
        TransferCertificateComponent,
        CertificateComponent
    ],
    imports: [
        CommonModule,
        CertificateRoutingModule,
        FormsModule,
        ReactiveFormsModule,
        RouterOutlet,
        SharedModule
    ],
    exports: [],
})
export class CertificateModule { }