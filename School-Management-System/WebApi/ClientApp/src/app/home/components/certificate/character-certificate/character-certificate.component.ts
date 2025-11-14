import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewEncapsulation } from '@angular/core';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { StudentCertificateViewModel } from '../model/studentCertificate.ViewModel';
import DateConverter from '@remotemerge/nepali-date-converter';
import { formatDate } from '@angular/common';

@Component({
  selector: 'app-character-certificate',
  standalone: false,
  templateUrl: './character-certificate.component.html',
  styleUrls: ['./character-certificate.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CharacterCertificateComponent implements OnChanges {
  @Input('studentObj') studentObj!: StudentCertificateViewModel
  @Input('serialNumber') serialNumber: number = 0;

  admittedDateEn: string = "";
  admittedDateNp: string = "";

  issueDateEn: string = "";
  issueDateNp: string = "";

  examHeldEn: string = "";
  examHeldNp: string = "";

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['studentObj'] && this.studentObj) {
      this.convertStudentDates(this.studentObj);
    }
  }

  formatDate(inputDate: Date, isYearOnlyRequired: boolean = false): { ad: string, bs: string } {
    let adDate = '';
    let bsDate = '';
    const dateString = inputDate.toString().split('T')[0];
    const nepaliDate = new DateConverter(dateString).toBs();
    const year = nepaliDate.year.toString().padStart(4, '0');
    const month = nepaliDate.month.toString().padStart(2, '0');
    const day = nepaliDate.date.toString().padStart(2, '0');
    if (isYearOnlyRequired) {
      adDate = formatDate(inputDate, 'yyyy', 'en-US');
      bsDate = `${year}`;
    }
    else {
      adDate = formatDate(inputDate, 'yyyy-MM-dd', 'en-US');
      bsDate = `${year}-${month}-${day}`;
    }
    return { ad: adDate, bs: bsDate };
  }

  private convertStudentDates(student: StudentCertificateViewModel): void {

    if (student.issueDate) {
      const issue = this.formatDate(student.issueDate, false);
      this.issueDateEn = issue.ad;
      this.issueDateNp = issue.bs;
    }

    if (student.admittedDate) {
      const admitted = this.formatDate(student.admittedDate, true);
      this.admittedDateEn = admitted.ad;
      this.admittedDateNp = admitted.bs;
    }

    if (student.examHeld) {
      const exam = this.formatDate(student.examHeld, true);
      this.examHeldEn = exam.ad;
      this.examHeldNp = exam.bs;
    }
  }

  getHonorofics(gender: number): string {
    if (gender === 1) return 'Mr.';
    if (gender === 2) return 'Miss.';
    return '';
  }

  getParentHonorofics(): string {
    const value = this.studentObj.fatherName != null ? 'Mr.' : this.studentObj.motherName != null ? 'Mrs.' : '';
    return value;
  }

  getParentName(): string {
    const value = this.studentObj.fatherName != null ? this.studentObj.fatherName : this.studentObj.motherName != null ? this.studentObj.motherName : '';
    return value;
  }

  getPronoun(gender: number, lower: boolean = false): string {
    let pronoun: string = '';
    if (gender === 1) pronoun = 'He';
    else if (gender === 2) pronoun = 'She';
    return lower ? pronoun.toLowerCase() : pronoun;
  }

  getPossessive(gender: number, lower: boolean = false): string {
    const value = gender === 1 ? 'His' : gender === 0 ? 'Her' : '';
    return lower ? value.toLowerCase() : value;
  }

  getChildRelation(gender: number, lower: boolean = false): string {
    const value = gender === 1 ? 'Son' : gender === 2 ? 'Daughter' : 'Child';
    return lower ? value.toLowerCase() : value;
  }

  getPassOrFail(gpa: number): string {
    return gpa < 0.8 ? 'failed' : 'passed';
  }
}
