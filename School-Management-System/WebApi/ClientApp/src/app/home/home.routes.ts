import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AuthGuardService } from '../shared/authGuard.service';

const routes: Routes = [
    {
        path: '', component: HomeComponent, canActivateChild: [AuthGuardService], children:
            [
                { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
                { path: "dashboard", component: DashboardComponent},
                { path: "master-entry", loadChildren:()=>import('./components/master-entry/master-entry.module').then(m=>m.MasterEntryModule)},
                { path: "student", loadChildren:()=>import('./components/student/student.module').then(m=>m.StudentModule)},
                { path: "teacher", loadChildren:()=>import('./components/teacher/teacher.module').then(m=>m.TeacherModule)},
                { path: "class", loadChildren:()=>import('./components/class-room/class.module').then(m=>m.ClassModule)},
                { path: "course", loadChildren:()=>import('./components/course/course.module').then(m=>m.CourseModule)},
                { path: "exam", loadChildren:()=>import('./components/exam/exam.module').then(m=>m.ExamModule)},
                { path: "certificate", loadChildren:()=>import('./components/certificate/certificate.module').then(m=>m.CertificateModule)},
                { path: "fees", loadChildren:()=>import('./components/fees/fees.module').then(m=>m.FeesModule)}
            ]
    }
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class HomeRoutingModule { }
