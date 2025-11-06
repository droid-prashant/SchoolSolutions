import { CertificateType } from "./certificateType.enum";

export interface StudentCertificateDto {
  studentEnrollmentId: string;
  certificateType: CertificateType;
  certificateNumber: number;
}