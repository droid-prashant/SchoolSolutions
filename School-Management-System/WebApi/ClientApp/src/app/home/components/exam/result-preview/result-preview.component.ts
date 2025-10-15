import { Component, Input, OnInit } from '@angular/core';
import { ResultViewModel } from '../shared/viewModels/result.viewModel';

@Component({
  selector: 'app-result-preview',
  standalone: false,
  templateUrl: './result-preview.component.html',
  styleUrl: './result-preview.component.scss'
})
export class ResultPreviewComponent implements OnInit {

  @Input('resultObj') result!: ResultViewModel;

  students = Array.from({ length: 50 }, (_, i) => ({
    name: `Student ${i + 1}`,
    course: ['Math', 'Science', 'English', 'Computer'][i % 4],
    marks: Math.floor(Math.random() * 41) + 60 // random marks 60–100
  }));


  student = {
    schoolName: 'OM PUSHPANJALI ENGLISH SCHOOL',
    alias: '(Jubie International English Academy)',
    address: 'Dodhara Chandani Municipality-7, Kanchanpur',
    province: 'Sudurpashchim Pradesh, Nepal',
    statement: 'STATEMENT OF GRADE-SHEET 2081 BS (2025 AD)',

    name: 'Rijan Sunar',
    father: 'Suresh Sunar',
    mother: 'Renu Sunar',
    dob: '2069/02/07 B.S.',
    rollNo: 9,
    class: 7,
    wardNo: '07',

    subjects: [
      { sn: 1, name: 'English', credit: 4, th: 'A', pr: 'A+', final: 'A', gp: 3.6 },
      { sn: 2, name: 'Nepali', credit: 4, th: 'B+', pr: 'A', final: 'B+', gp: 3.2 },
      { sn: 3, name: 'Mathematics', credit: 4, th: 'A+', pr: 'A', final: 'A+', gp: 4.0 },
      { sn: 4, name: 'Science & Environment', credit: 4, th: 'B', pr: 'B+', final: 'B+', gp: 2.8 },
      { sn: 5, name: 'Social Studies', credit: 4, th: 'B', pr: 'B+', final: 'B+', gp: 2.8 },
      { sn: 6, name: 'Hamro D.C', credit: 4, th: 'A', pr: 'A+', final: 'A', gp: 3.6 },
      { sn: 7, name: 'Health', credit: 4, th: 'A', pr: 'A+', final: 'A', gp: 3.6 },
    ],

    gpa: 3.21,
    attendance: '249/254',
    issueDate: '2081/12/28'
  };

  constructor(){

  }
  ngOnInit(): void {
   console.log(this.result)
  }
}
