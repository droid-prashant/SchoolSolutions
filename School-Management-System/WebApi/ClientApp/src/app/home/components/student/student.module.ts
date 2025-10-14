import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { RouterOutlet } from "@angular/router";
import { ListStudentComponent } from "./list-student/list-student.component";
import { AddStudentComponent } from "./add-student/add-student.component";
import { SharedModule } from "../../../shared/shared.module";
import { StudentRoutingModule } from "./student.routes";
import { HttpClientModule } from "@angular/common/http";


@NgModule({
    declarations: [
        ListStudentComponent,
        AddStudentComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        StudentRoutingModule,
        SharedModule,
        FormsModule,
        HttpClientModule
    ],
    exports: [],
})
export class StudentModule { }