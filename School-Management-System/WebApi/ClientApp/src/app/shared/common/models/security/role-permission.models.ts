export interface PermissionViewModel {
  id: string;
  code: string;
  name: string;
  groupName: string;
  description: string;
}

export interface RoleViewModel {
  id: string;
  name: string;
  description: string;
  permissionCodes: string[];
}

export interface RoleDto {
  name: string;
  description: string;
  permissionCodes: string[];
}

export interface RolePermissionsDto {
  permissionCodes: string[];
}

export interface UserViewModel {
  id: string;
  userName: string;
  email: string;
  fullName: string;
  isActive: boolean;
  roles: string[];
}

export interface UserRolesViewModel {
  userId: string;
  userName: string;
  roleNames: string[];
}

export interface UserRolesDto {
  roleNames: string[];
}
