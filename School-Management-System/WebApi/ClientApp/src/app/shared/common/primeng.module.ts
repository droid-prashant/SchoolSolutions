import { NgModule } from "@angular/core";
import { DataViewModule } from 'primeng/dataview';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { MenubarModule } from 'primeng/menubar';
import { AvatarModule } from 'primeng/avatar';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { ToastModule } from 'primeng/toast';
import { InputTextModule } from "primeng/inputtext";
import { RippleModule } from "primeng/ripple";

@NgModule({
    imports: [
        ButtonModule,
        DataViewModule,
        TagModule,
        MenubarModule,
        AvatarModule,
        CardModule,
        DividerModule,
        ToastModule,
        InputTextModule,
        RippleModule, 
        ButtonModule
    ],
    exports: [
        ButtonModule,
        DataViewModule,
        TagModule,
        MenubarModule,
        AvatarModule,
        CardModule,
        DividerModule,
        ToastModule,
        InputTextModule,
        RippleModule, 
        ButtonModule
    ]
})
export class PrimengModule { }