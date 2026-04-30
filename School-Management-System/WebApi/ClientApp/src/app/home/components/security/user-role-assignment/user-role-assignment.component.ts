import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { RoleViewModel, UserViewModel } from '../../../../shared/common/models/security/role-permission.models';

@Component({
  selector: 'app-user-role-assignment',
  standalone: false,
  templateUrl: './user-role-assignment.component.html',
  styleUrl: './user-role-assignment.component.scss'
})
export class UserRoleAssignmentComponent implements OnInit {
  users: UserViewModel[] = [];
  roles: RoleViewModel[] = [];
  selectedUserId = '';
  selectedRoleNames: string[] = [];

  constructor(
    private apiService: ApiService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.apiService.getUsers().subscribe({
      next: users => this.users = users,
      error: err => this.showError(err?.error?.message ?? 'Failed to load users.')
    });

    this.apiService.getRoles().subscribe({
      next: roles => this.roles = roles,
      error: err => this.showError(err?.error?.message ?? 'Failed to load roles.')
    });
  }

  onUserChange(): void {
    this.selectedRoleNames = [];
    if (!this.selectedUserId) {
      return;
    }

    this.apiService.getUserRoles(this.selectedUserId).subscribe({
      next: userRoles => this.selectedRoleNames = userRoles.roleNames,
      error: err => this.showError(err?.error?.message ?? 'Failed to load user roles.')
    });
  }

  saveRoles(): void {
    if (!this.selectedUserId) {
      this.messageService.add({
        severity: 'warn',
        summary: 'User Required',
        detail: 'Please select a user.'
      });
      return;
    }

    this.apiService.setUserRoles(this.selectedUserId, { roleNames: this.selectedRoleNames }).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Saved',
          detail: 'User roles updated successfully.'
        });
        this.loadData();
      },
      error: err => this.showError(err?.error?.message ?? 'Failed to update user roles.')
    });
  }

  private showError(detail: string): void {
    this.messageService.add({
      severity: 'error',
      summary: 'Error',
      detail
    });
  }
}
