import { Component, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-character-certificate',
  standalone: false,
  templateUrl: './character-certificate.component.html',
  styleUrls: ['./character-certificate.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CharacterCertificateComponent {
 studentName = 'BP Bhatta';
  fatherName = 'Uncle Bhatta';
  address = 'Dodhara Chandani Municipality-08, Kanchanpur';
  examName = 'Final';
  grade = '5';
  bsYear = '2081';
  adYear = '2024';
  gpa = '3.54';

}
