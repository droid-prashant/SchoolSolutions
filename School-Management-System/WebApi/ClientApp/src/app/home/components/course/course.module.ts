import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { RouterOutlet } from "@angular/router";
import { SharedModule } from "../../../shared/shared.module";
import { ManageCourseComponent } from "./manage-course/manage-course.component";
import { CourseRoutingModule } from "./course.routes";


@NgModule({
    declarations: [
        ManageCourseComponent
    ],
    imports: [
        CommonModule,
        CourseRoutingModule,
        FormsModule,
        ReactiveFormsModule,
        RouterOutlet,
        SharedModule
    ],
    exports: [],
})
export class CourseModule { }