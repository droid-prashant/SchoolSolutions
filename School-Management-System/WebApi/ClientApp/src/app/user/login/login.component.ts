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
  private readonly rememberedLoginKey = 'rememberedLogin';

  loginForm!: FormGroup;
  academicYearList: AcademicViewModel[] = [];
  rememberedLogin: { username: string; academicYear: string } | null = null;

  isSubmitted: boolean = false;

  constructor(private _fb: FormBuilder, private _apiService: ApiService, private _authService: AuthService, private _messageService: MessageService,
    private _router: Router
  ) {
    this.loginForm = this._fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      academicYear: ['', Validators.required],
      remember: [false]
    });
  }
  ngOnInit(): void {
    this.loadRememberedLogin();
    this.listAcademicYear();
  }

  listAcademicYear() {
    this._apiService.getAcademicYear().subscribe({
      next: (res) => {
        this.academicYearList = res;
        const rememberedAcademicYear = this.rememberedLogin?.academicYear;
        const isRememberedAcademicYearAvailable = this.academicYearList.some(x => x.id === rememberedAcademicYear);
        const activeAcademicYear = this.academicYearList.find(x => x.isActive === true);
        const selectedAcademicYear = isRememberedAcademicYearAvailable ? rememberedAcademicYear : activeAcademicYear?.id;

        this.loginForm.patchValue({
          username: this.rememberedLogin?.username ?? this.loginForm.get('username')?.value,
          academicYear: selectedAcademicYear ?? '',
          remember: !!this.rememberedLogin
        });
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
    const formValue = this.loginForm.value;
    const loginCred: Login = {
      userName: formValue.username,
      password: formValue.password,
      academicYear: formValue.academicYear
    };

    this._authService.login(loginCred).subscribe({
      next: (res) => {
        let token = res.token;
        if (token) {
          window.localStorage.setItem("token", token);
          this.saveRememberedLogin();
          this._router.navigateByUrl("/home");
        }
      },
      error: () => {
        this.isSubmitted = false;
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to Login' });
      },
      complete: () => {
        this.isSubmitted = false;
        this.loginForm.reset();
      }
    })
  }

  private loadRememberedLogin(): void {
    const rememberedLogin = window.localStorage.getItem(this.rememberedLoginKey);
    if (!rememberedLogin) {
      return;
    }

    try {
      this.rememberedLogin = JSON.parse(rememberedLogin);
      this.loginForm.patchValue({
        username: this.rememberedLogin?.username ?? '',
        remember: true
      });
    } catch {
      window.localStorage.removeItem(this.rememberedLoginKey);
    }
  }

  private saveRememberedLogin(): void {
    const formValue = this.loginForm.value;
    if (!formValue.remember) {
      window.localStorage.removeItem(this.rememberedLoginKey);
      return;
    }

    window.localStorage.setItem(this.rememberedLoginKey, JSON.stringify({
      username: formValue.username,
      academicYear: formValue.academicYear
    }));
  }
}
