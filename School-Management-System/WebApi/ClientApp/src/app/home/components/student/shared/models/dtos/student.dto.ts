export interface StudentDto {
  id?: string;
  firstName: string;
  lastName: string;
  grandFatherName: string;
  fatherName: string;
  motherName: string;
  gender: number;         
  age: number;
  address: string;
  contactNumber: string;
  parentContactNumber: string;
  parentEmail: string;
  classRoomId: string;    
  sectionId: string;    
  provinceId: string;    
  districtId: string;    
  municipalityId: string; 
  rollNumber: string;
}

export interface StudentStatusDto {
  isActive: boolean;
  remarks?: string;
}
