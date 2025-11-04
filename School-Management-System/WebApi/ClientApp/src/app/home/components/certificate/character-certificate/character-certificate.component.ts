import { Component, Input, ViewEncapsulation } from '@angular/core';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { StudentCertificateViewModel } from '../model/studentCertificate.ViewModel';

@Component({
  selector: 'app-character-certificate',
  standalone: false,
  templateUrl: './character-certificate.component.html',
  styleUrls: ['./character-certificate.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CharacterCertificateComponent {
  @Input('studentObj') studentObj!: StudentCertificateViewModel

}
