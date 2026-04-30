import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../../shared/shared.module';
import { RoleManagementComponent } from './role-management/role-management.component';
import { SecurityRoutingModule } from './security.routes';
import { UserRoleAssignmentComponent } from './user-role-assignment/user-role-assignment.component';

@NgModule({
  declarations: [
    RoleManagementComponent,
    UserRoleAssignmentComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    SecurityRoutingModule
  ]
})
export class SecurityModule { }
