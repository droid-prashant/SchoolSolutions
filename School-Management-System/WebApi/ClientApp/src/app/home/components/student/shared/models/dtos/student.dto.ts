export interface StudentDto {
  id?: string;
  firstName: string;
  lastName: string;
  grandFatherName: string;
  fatherName: string;
  motherName: string;
  gender: number;
  age: number;
  dobNp: string;
  dobEn: string;
  wardNo: number;
  contactNumber: string;
  parentContactNumber: string;
  parentEmail: string;
  classRoomId: string;
  classSectionId: string;
  sectionId: string;
  provinceId: string;
  districtId: string;
  municipalityId: string;
  rollNumber: string;
  isBusRequired: boolean;
}

export interface StudentStatusDto {
  isActive: boolean;
  remarks?: string;
}
