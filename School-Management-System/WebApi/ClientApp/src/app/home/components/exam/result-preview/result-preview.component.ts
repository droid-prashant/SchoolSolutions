import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ResultViewModel } from '../shared/viewModels/result.viewModel';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { formatDate } from '@angular/common';
import { DateConverterService } from '../../../../shared/calender/date-convertor.service';
import DateConverter from '@remotemerge/nepali-date-converter';

@Component({
  selector: 'app-result-preview',
  standalone: false,
  templateUrl: './result-preview.component.html',
  styleUrl: './result-preview.component.scss'
})
export class ResultPreviewComponent implements OnInit, OnChanges {

  @Input('resultObj') resultObj!: ResultViewModel;
  @Input('studentObj') studentObj!: StudentViewModel;

  issueDateEn: string = "";
  issueDateNp: string = "";
  currentYearNp: string = "";
  currentYearEn: string = "";

  student = {
    schoolName: 'OM PUSHPANJALI ENGLISH SCHOOL',
    alias: '(Jubie International English Academy)',
    address: 'Dodhara Chandani Municipality-7, Kanchanpur',
    province: 'Sudurpashchim Pradesh, Nepal',
    statement: 'STATEMENT OF GRADE-SHEET'
  };

  constructor(private dateConverter: DateConverterService) {

  }
  ngOnInit(): void {
    const todaysDateAd = new Date();
    const todaysDateBs = this.dateConverter.adToBsParts(todaysDateAd);
    this.currentYearNp = todaysDateBs.year.toString();
    this.currentYearEn = formatDate(todaysDateAd, 'yyyy', 'en-US');
    console.log(this.studentObj);
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['resultObj'] && this.resultObj) {
      this.convertStudentDates(this.resultObj);
    }
  }

  private convertStudentDates(result: ResultViewModel): void {

    if (result.issueDate) {
      const issue = this.formatDate(result.issueDate, false);
      this.issueDateEn = issue.ad;
      this.issueDateNp = issue.bs;
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

  getFinalGradePoint(gradePoint: number): string {
    if (gradePoint === 4) {
      return '4.0';
    }
    return gradePoint.toString();
  }

  get displayDateOfBirth(): string {
    const dateOfBirth = this.resultObj?.dateOfBirth || this.studentObj?.dateOfBirthNp;
    return dateOfBirth ? `${dateOfBirth} B.S` : '-';
  }

  get displayAddress(): string {
    if (this.resultObj?.address?.trim()) {
      return this.resultObj.address.trim();
    }

    if (this.studentObj?.address?.trim()) {
      return this.studentObj.address.trim();
    }

    const municipality = this.studentObj?.municipalityName?.trim();
    const wardNo = this.studentObj?.wardNo;

    if (municipality) {
      return wardNo ? `${municipality} - ${wardNo}` : municipality;
    }

    return this.resultObj?.wardNo ? `Ward ${this.resultObj.wardNo}` : '-';
  }

  get overallRemark(): string {
    const gpa = this.resultObj?.gpa ?? 0;

    if (gpa >= 3.6) {
      return 'Outstanding progress. The student has performed very well in all areas. Keep it up.';
    }

    if (gpa >= 3.2) {
      return 'Very good progress. The student is doing well and should continue the same effort.';
    }

    if (gpa >= 2.8) {
      return 'Good progress. With a little more focus and regular practice, the student can do even better.';
    }

    if (gpa >= 2.0) {
      return 'Satisfactory progress. The student needs to work harder and improve consistency in studies.';
    }

    if (gpa >= 1.6) {
      return 'Basic progress has been made, but much more effort is needed. Regular study and guidance are recommended.';
    }

    return 'The student needs significant improvement. More hard work, practice, and support from home and school are necessary.';
  }
}
