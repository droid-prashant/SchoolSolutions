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
}
