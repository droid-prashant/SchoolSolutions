import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { ApiService } from '../../shared/api.service';
import { MessageService } from 'primeng/api';
import { AcademicViewModel } from '../../home/components/master-entry/model/viewModels/academicYear.ViewModel';
import { Login } from '../../shared/common/models/login/login.mode';
import { AuthService } from '../../shared/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, SharedModule, ReactiveFormsModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  academicYearList: AcademicViewModel[] = [];

  isSubmitted: boolean = false;

  constructor(private _fb: FormBuilder, private _apiService: ApiService, private _authService: AuthService, private _messageService: MessageService,
    private _router: Router
  ) {
    this.loginForm = this._fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      academicYear: ['', Validators.required],
      remember: ['']
    });
  }
  ngOnInit(): void {
    this.listAcademicYear();
  }

  listAcademicYear() {
    this._apiService.getAcademicYear().subscribe({
      next: (res) => {
        this.academicYearList = res;
        const activeAcademicYear = this.academicYearList.find(x => x.isActive === true);
        if (activeAcademicYear) {
          this.loginForm.get('academicYear')?.setValue(activeAcademicYear);
        }
      },
      error: (res) => {
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load Academic Year list' });
      }
    })
  }

  onSubmit() {
    this.isSubmitted = true;
    if (!this.loginForm.valid) {
      return;
    }
    const loginCred: Login = this.loginForm.value;
    this._authService.login(loginCred).subscribe({
      next: (res) => {
        let token = res.token;
        if (token) {
          window.localStorage.setItem("token", token);
          this._router.navigateByUrl("/home");
        }
      },
      error: () => {
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to Login' });
      },
      complete: () => {
        this.isSubmitted = false;
        this.loginForm.reset();
      }
    })
  }
}
