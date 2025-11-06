import { Component, Input, OnInit } from '@angular/core';
import { StudentViewModel } from '../../../student/shared/models/viewModels/student.viewModel';
import { StudentCertificateViewModel } from '../../model/studentCertificate.ViewModel';

@Component({
  selector: 'app-transfer-certificate',
  standalone: false,
  templateUrl: './transfer-certificate.component.html',
  styleUrl: './transfer-certificate.component.scss'
})
export class TransferCertificateComponent implements OnInit {
  @Input('studentObj') studentObj!: StudentCertificateViewModel
   @Input('serialNumber') serialNumber: number = 0;
  
  ngOnInit(): void {
    console.log(this.studentObj);
  }
}
