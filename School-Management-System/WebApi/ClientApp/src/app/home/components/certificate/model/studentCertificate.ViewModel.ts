export interface StudentCertificateViewModel {
  id: string;
  academicYear: string;
  admittedDate: Date;
  issueDate: Date;
  name: string;
  age: number;
  gender: number;
  fatherName: string;
  motherName: string;
  dateOfBirthNp: string;
  dateOfBirthEn: string;
  address: string;
  registrationNumber?: string | null;
  symbolNumber?: string | null;
  wardNo: number;
  classRoom: string;
  firstAdmittedClass: string;
  section: string;
  gpa: number;
  examType: string;
  examHeld: Date;
  isCharacterCertificateTaken:boolean;
  isTransferCertificateTaken:boolean;
}