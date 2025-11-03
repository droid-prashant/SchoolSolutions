import { Component, Input } from '@angular/core';
import { StudentViewModel } from '../../../student/shared/models/viewModels/student.viewModel';

@Component({
  selector: 'app-transfer-certificate',
  standalone: false,
  templateUrl: './transfer-certificate.component.html',
  styleUrl: './transfer-certificate.component.scss'
})
export class TransferCertificateComponent {
  @Input('studentObj') studentObj!:StudentViewModel
  studentName = 'Birendra Bir Bikram Shah Dev';
  fatherName = 'Uncle Bhatta';
  address = 'Dodhara Chandani Municipality-08, Kanchanpur';
  examName = 'Final';
  grade = '5';
  bsYear = '2081';
  adYear = '2024';
  gpa = '3.54';
}
