import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { ApiService } from '../../shared/api.service';
import { MessageService } from 'primeng/api';
import { AcademicViewModel } from '../../home/components/master-entry/model/viewModels/academicYear.ViewModel';

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

  constructor(private _fb: FormBuilder, private _apiService: ApiService, private _messageService: MessageService) {
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
    if (this.loginForm.valid) {
      console.log('✅ Login data:', this.loginForm.value);
      // TODO: Call your authentication API here
    } else {
      this.loginForm.markAllAsTouched();
    }
  }
}
