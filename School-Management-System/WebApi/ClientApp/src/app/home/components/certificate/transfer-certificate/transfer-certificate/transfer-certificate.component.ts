import { Component } from '@angular/core';

@Component({
  selector: 'app-transfer-certificate',
  standalone: false,
  templateUrl: './transfer-certificate.component.html',
  styleUrl: './transfer-certificate.component.scss'
})
export class TransferCertificateComponent {
  studentName = 'Birendra Bir Bikram Shah Dev';
  fatherName = 'Uncle Bhatta';
  address = 'Dodhara Chandani Municipality-08, Kanchanpur';
  examName = 'Final';
  grade = '5';
  bsYear = '2081';
  adYear = '2024';
  gpa = '3.54';
}
