export interface StudentCertificateViewModel {
  id: string;
  academicYear: string;
  admittedYear: string;
  issueDate: string;
  name: string;
  age: number;
  gender: number;
  fatherName: string;
  motherName: string;
  dateOfBirth: string;
  address: string;
  registrationNumber?: string | null;
  symbolNumber?: string | null;
  wardNo: number;
  classRoom: string;
  firstAdmittedClass: string;
  section: string;
  gpa: number;
  examType: string;
  examHeldYear: string;
  isCharacterCertificateTaken:boolean;
  isTransferCertificateTaken:boolean;
}