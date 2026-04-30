import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { PermissionViewModel, RoleDto, RoleViewModel } from '../../../../shared/common/models/security/role-permission.models';

@Component({
  selector: 'app-role-management',
  standalone: false,
  templateUrl: './role-management.component.html',
  styleUrl: './role-management.component.scss'
})
export class RoleManagementComponent implements OnInit {
  roles: RoleViewModel[] = [];
  permissions: PermissionViewModel[] = [];
  groupedPermissions: { groupName: string; permissions: PermissionViewModel[] }[] = [];
  selectedPermissionCodes: string[] = [];
  roleForm: FormGroup;
  isDialogVisible = false;
  isEditMode = false;
  selectedRoleId: string | null = null;

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder,
    private messageService: MessageService
  ) {
    this.roleForm = this.fb.group({
      name: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.apiService.getRoles().subscribe({
      next: roles => this.roles = roles,
      error: err => this.showError(err?.error?.message ?? 'Failed to load roles.')
    });

    this.apiService.getPermissions().subscribe({
      next: permissions => {
        this.permissions = permissions;
        this.groupedPermissions = this.groupPermissions(permissions);
      },
      error: err => this.showError(err?.error?.message ?? 'Failed to load permissions.')
    });
  }

  openCreateDialog(): void {
    this.isEditMode = false;
    this.selectedRoleId = null;
    this.selectedPermissionCodes = [];
    this.roleForm.reset({ name: '', description: '' });
    this.isDialogVisible = true;
  }

  openEditDialog(role: RoleViewModel): void {
    this.isEditMode = true;
    this.selectedRoleId = role.id;
    this.selectedPermissionCodes = [...role.permissionCodes];
    this.roleForm.reset({
      name: role.name,
      description: role.description
    });
    this.isDialogVisible = true;
  }

  saveRole(): void {
    this.roleForm.markAllAsTouched();
    if (this.roleForm.invalid) {
      return;
    }

    const payload: RoleDto = {
      name: this.roleForm.value.name,
      description: this.roleForm.value.description ?? '',
      permissionCodes: this.selectedPermissionCodes
    };

    const request = this.isEditMode && this.selectedRoleId
      ? this.apiService.updateRole(this.selectedRoleId, payload)
      : this.apiService.createRole(payload);

    request.subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Saved',
          detail: 'Role saved successfully.'
        });
        this.isDialogVisible = false;
        this.loadData();
      },
      error: err => this.showError(err?.error?.message ?? 'Failed to save role.')
    });
  }

  deleteRole(role: RoleViewModel): void {
    if (!window.confirm(`Delete role ${role.name}?`)) {
      return;
    }

    this.apiService.deleteRole(role.id).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Deleted',
          detail: 'Role deleted successfully.'
        });
        this.loadData();
      },
      error: err => this.showError(err?.error?.message ?? 'Failed to delete role.')
    });
  }

  private groupPermissions(permissions: PermissionViewModel[]): { groupName: string; permissions: PermissionViewModel[] }[] {
    const groups = new Map<string, PermissionViewModel[]>();
    permissions.forEach(permission => {
      const groupPermissions = groups.get(permission.groupName) ?? [];
      groupPermissions.push(permission);
      groups.set(permission.groupName, groupPermissions);
    });

    return Array.from(groups.entries()).map(([groupName, groupPermissions]) => ({
      groupName,
      permissions: groupPermissions
    }));
  }

  private showError(detail: string): void {
    this.messageService.add({
      severity: 'error',
      summary: 'Error',
      detail
    });
  }
}
