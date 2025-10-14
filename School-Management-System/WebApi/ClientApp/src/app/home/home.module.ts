import { NgModule } from "@angular/core";
import { HomeRoutingModule } from "./home.routes"
import { HomeComponent } from "./home.component";
import { FormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { RouterOutlet } from "@angular/router";
import { SharedModule } from "../shared/shared.module";


@NgModule({
    declarations: [
        HomeComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterOutlet,
        HomeRoutingModule,
        SharedModule
    ],
    exports: [],
})
export class HomeModule { }