export interface TeacherViewModel {
  id: string;
  userId?: string;
  employeeCode?: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  fullName: string;
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
  isActive: boolean;
  inactiveReason?: string;
  qualifications: TeacherQualificationViewModel[];
  experiences: TeacherExperienceViewModel[];
  documents: TeacherDocumentViewModel[];
  assignments: TeacherClassSectionViewModel[];
}

export interface TeacherQualificationViewModel {
  id: string;
  degreeName: string;
  institutionName: string;
  boardOrUniversity?: string;
  passedYear?: string;
  gradeOrPercentage?: string;
  majorSubject?: string;
  remarks?: string;
}

export interface TeacherExperienceViewModel {
  id: string;
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

export interface TeacherClassSectionViewModel {
  id: string;
  academicYearId: string;
  academicYearName: string;
  classSectionId: string;
  classRoomId: string;
  classRoomName: string;
  sectionId: string;
  sectionName: string;
  courseId: string;
  courseName: string;
  isClassTeacher: boolean;
  isActive: boolean;
  remarks?: string;
}

export interface TeacherDocumentViewModel {
  id: string;
  documentType: string;
  documentTitle: string;
  filePath: string;
  originalFileName: string;
  mimeType: string;
  fileSize: number;
  uploadedDate: string;
}

export interface TeacherDashboardViewModel {
  totalActiveTeachers: number;
  totalInactiveTeachers: number;
  assignedTeachers: number;
  classTeachers: number;
}
