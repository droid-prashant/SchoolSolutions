import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ButtonModule } from 'primeng/button'
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { SplitterModule } from 'primeng/splitter';
import { MenubarModule } from 'primeng/menubar';
import { MenuModule } from 'primeng/menu';
import { AvatarModule } from 'primeng/avatar';
import { BadgeModule } from 'primeng/badge';
import { RippleModule } from 'primeng/ripple';
import { PanelMenuModule } from 'primeng/panelmenu';
import { TableModule } from 'primeng/table';
import { SpeedDialModule } from 'primeng/speeddial';
import { ToastModule } from 'primeng/toast';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { InputMaskModule } from 'primeng/inputmask';
import { DialogModule } from 'primeng/dialog';
import { MultiSelectModule } from 'primeng/multiselect';
import { TagModule } from 'primeng/tag';
import { NgxPrintModule } from 'ngx-print';
import { InputSwitchModule } from 'primeng/inputswitch';
import { DividerModule } from 'primeng/divider';
import { PasswordModule } from 'primeng/password';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { FormsModule } from "@angular/forms";
import { ChartModule } from 'primeng/chart';
import { StudentFilterComponent } from "../home/components/student-filter/student-filter.component";
import { PageHeaderComponent } from "./page-header/page-header.component";

@NgModule({
    declarations: [],
    imports: [
        CommonModule,
        ButtonModule,
        InputTextModule,
        CardModule,
        SplitterModule,
        MenubarModule,
        AvatarModule,
        BadgeModule,
        MenuModule,
        RippleModule,
        PanelMenuModule,
        TableModule,
        SpeedDialModule,
        ToastModule,
        DropdownModule,
        CalendarModule,
        InputMaskModule,
        DialogModule,
        MultiSelectModule,
        TagModule,
        NgxPrintModule,
        InputSwitchModule,
        DividerModule,
        PasswordModule,
        CheckboxModule,
        ConfirmDialogModule,
        OverlayPanelModule,
        FormsModule,
        ChartModule,
        StudentFilterComponent,
        PageHeaderComponent
        
    ],
    exports: [
        CommonModule,
        ButtonModule,
        InputTextModule,
        CardModule,
        SplitterModule,
        MenubarModule,
        AvatarModule,
        BadgeModule,
        MenuModule,
        RippleModule,
        PanelMenuModule,
        TableModule,
        SpeedDialModule,
        ToastModule,
        DropdownModule,
        CalendarModule,
        InputMaskModule,
        DialogModule,
        MultiSelectModule,
        TagModule,
        NgxPrintModule,
        InputSwitchModule,
        DividerModule,
        PasswordModule,
        CheckboxModule,
        ConfirmDialogModule,
        OverlayPanelModule,
        ChartModule,
        StudentFilterComponent,
        PageHeaderComponent
    ]
})
export class SharedModule { }
