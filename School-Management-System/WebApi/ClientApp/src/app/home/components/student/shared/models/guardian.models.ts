export interface GuardianCreateDto {
  fullName: string;
  contactNumber: string;
  email?: string | null;
  relationType: string;
  userName: string;
  password: string;
  isPrimaryGuardian: boolean;
  canViewFees: boolean;
  canViewResults: boolean;
  canViewAttendance: boolean;
  canPayFees: boolean;
}

export interface GuardianStudentLinkDto {
  guardianId: string;
  studentId: string;
  isPrimaryGuardian: boolean;
  canViewFees: boolean;
  canViewResults: boolean;
  canViewAttendance: boolean;
  canPayFees: boolean;
}

export interface GuardianStudentAccessDto {
  isPrimaryGuardian: boolean;
  canViewFees: boolean;
  canViewResults: boolean;
  canViewAttendance: boolean;
  canPayFees: boolean;
}

export interface GuardianViewModel {
  id: string;
  userId: string;
  fullName: string;
  contactNumber: string;
  email?: string | null;
  relationType: string;
  userName: string;
  isActive: boolean;
  linkedStudentsCount: number;
}

export interface StudentGuardianViewModel {
  guardianStudentId: string;
  guardianId: string;
  userId: string;
  fullName: string;
  contactNumber: string;
  email?: string | null;
  relationType: string;
  userName: string;
  isGuardianActive: boolean;
  isPrimaryGuardian: boolean;
  canViewFees: boolean;
  canViewResults: boolean;
  canViewAttendance: boolean;
  canPayFees: boolean;
}

export interface GuardianLinkedStudentViewModel {
  studentId: string;
  guardianStudentId: string;
  studentName: string;
  className: string;
  sectionName: string;
  rollNumber?: number | null;
  canViewFees: boolean;
  canViewResults: boolean;
  canViewAttendance: boolean;
  canPayFees: boolean;
}
