import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ResultViewModel } from '../shared/viewModels/result.viewModel';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { formatDate } from '@angular/common';
import { DateConverterService } from '../../../../shared/calender/date-convertor.service';
import DateConverter from '@remotemerge/nepali-date-converter';
import { LookupService } from '../../../../shared/common/lookup.service';

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
  academicYearNameNp: string = "";
  academicYearNameEn: string = "";

  student = {
    schoolName: 'OM PUSHPANJALI ENGLISH SCHOOL',
    alias: '(Jubie International English Academy)',
    address: 'Dodhara Chandani Municipality-7, Kanchanpur',
    province: 'Sudurpashchim Pradesh, Nepal',
    statement: 'STATEMENT OF GRADE-SHEET'
  };

  constructor(private dateConverter: DateConverterService, private lookupService: LookupService) {

  }
  ngOnInit(): void {
    const todaysDateAd = new Date();
    const todaysDateBs = this.dateConverter.adToBsParts(todaysDateAd);
    this.currentYearNp = todaysDateBs.year.toString();
    this.currentYearEn = formatDate(todaysDateAd, 'yyyy', 'en-US');
    this.setAcademicYear();
    console.log(this.studentObj);
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['resultObj'] && this.resultObj) {
      this.convertStudentDates(this.resultObj);
    }
  }

  private setAcademicYear(): void {
    this.lookupService.getActiveAcademicYear().subscribe({
      next: (academicYear) => {
        this.academicYearNameNp = academicYear?.yearName ?? this.currentYearNp;
      },
      error: () => {
      }
    });
  }

  private convertStudentDates(result: ResultViewModel): void {

    if (result.issueDate) {
      const issue = this.formatDate(result.issueDate, false);
      this.issueDateEn = issue.ad;
      this.issueDateNp = issue.bs;
      this.academicYearNameEn = formatDate(result.issueDate, 'yyyy', 'en-US');
    }
  }

  get statementAcademicYear(): string {
    const nepaliYear = this.academicYearNameNp || this.currentYearNp;
    const englishYear = this.academicYearNameEn || this.currentYearEn;
    return `${nepaliYear} B.S (${englishYear} A.D)`;
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
    return this.formatSubjectGradePoint(gradePoint);
  }

  get resultStatus(): string {
    const nonGradedStatus = this.nonGradedStatus;
    if (nonGradedStatus) {
      return nonGradedStatus;
    }

    return (this.resultObj?.gpa ?? 0) >= 1.6 ? 'PASSED' : 'NEEDS IMPROVEMENT';
  }

  get displayGpa(): string {
    return this.resultStatus === 'PASSED' ? this.formatGpa(this.resultObj?.gpa) : '-';
  }

  private formatSubjectGradePoint(value: number | null | undefined): string {
    if (typeof value !== 'number' || Number.isNaN(value)) {
      return '-';
    }

    return Number.isInteger(value) ? value.toFixed(1) : value.toString();
  }

  private formatGpa(value: number | null | undefined): string {
    return typeof value === 'number' && !Number.isNaN(value) ? value.toFixed(2) : '-';
  }

  private get nonGradedStatus(): string {
    let hasNg = false;

    for (const mark of this.resultObj?.studentMarks ?? []) {
      const grades = [mark.finalGrade, mark.gradeTheory, mark.gradePractical];

      if (grades.some(grade => this.normalizeGrade(grade) === 'NQ')) {
        return 'NQ';
      }

      if (grades.some(grade => this.normalizeGrade(grade) === 'NG')) {
        hasNg = true;
      }
    }

    return hasNg ? 'NG' : '';
  }

  private normalizeGrade(grade: string | null | undefined): string {
    return grade?.trim().toUpperCase().replace(/[^A-Z]/g, '') ?? '';
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
