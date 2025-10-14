import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';

const routes: Routes = [
    {
        path: '', component: HomeComponent, children:
            [
                { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
                { path: "dashboard", component: DashboardComponent},
                { path: "master-entry", loadChildren:()=>import('./components/master-entry/master-entry.module').then(m=>m.MasterEntryModule)},
                { path: "student", loadChildren:()=>import('./components/student/student.module').then(m=>m.StudentModule)},
                { path: "class", loadChildren:()=>import('./components/class-room/class.module').then(m=>m.ClassModule)},
                { path: "course", loadChildren:()=>import('./components/course/course.module').then(m=>m.CourseModule)},
                { path: "exam", loadChildren:()=>import('./components/exam/exam.module').then(m=>m.ExamModule)}
            ]
    }
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class HomeRoutingModule { }
