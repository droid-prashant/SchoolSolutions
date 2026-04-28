export interface TeacherDto {
  id?: string;
  employeeCode?: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  gender: number;
  age?: number;
  dateOfBirthNp?: string;
  dateOfBirthEn?: string;
  contactNumber: string;
  alternateContactNumber?: string;
  email?: string;
  provinceId?: number;
  districtId?: number;
  municipalityId?: number;
  wardNo?: number;
  fatherName?: string;
  motherName?: string;
  designation?: string;
  joiningDateNp?: string;
  joiningDateEn?: string;
  createUser: boolean;
  userName?: string;
  password?: string;
  qualifications: TeacherQualificationDto[];
  experiences: TeacherExperienceDto[];
  assignments: TeacherClassSectionDto[];
}

export interface TeacherQualificationDto {
  id?: string;
  degreeName: string;
  institutionName: string;
  boardOrUniversity?: string;
  passedYear?: string;
  gradeOrPercentage?: string;
  majorSubject?: string;
  remarks?: string;
}

export interface TeacherExperienceDto {
  id?: string;
  organizationName: string;
  designation: string;
  subjectOrDepartment?: string;
  startDateNp?: string;
  startDateEn?: string;
  endDateNp?: string;
  endDateEn?: string;
  isCurrent: boolean;
  remarks?: string;
}

export interface TeacherClassSectionDto {
  id?: string;
  academicYearId: string;
  classSectionId: string;
  courseId: string;
  isClassTeacher: boolean;
  remarks?: string;
}

export interface TeacherStatusDto {
  isActive: boolean;
  inactiveReason?: string;
}

export interface TeacherAccountCreateDto {
  userName: string;
  password: string;
}

export interface TeacherAccountStatusDto {
  isActive: boolean;
}

export interface TeacherPasswordResetDto {
  newPassword: string;
}

export interface TeacherAssignmentCopyDto {
  sourceAcademicYearId: string;
  targetAcademicYearId: string;
}
