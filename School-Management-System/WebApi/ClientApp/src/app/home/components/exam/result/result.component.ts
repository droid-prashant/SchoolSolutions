import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';

@Component({
  selector: 'app-result',
  standalone: false,
  templateUrl: './result.component.html',
  styleUrl: './result.component.scss'
})
export class ResultComponent implements OnInit {
  showResultPreview: boolean = false;
  students: StudentViewModel[] = [];
  classRooms: ClassRoomViewModel[] = []
  classId: string = "";
  showResult: boolean = false;
  constructor(private _apiService: ApiService) {

  }
  ngOnInit(): void {
    this.getClassRooms();
  }
  previewResult(student:StudentViewModel) {
    this.showResult = true;
  }

  listStudent() {
    this._apiService.getStudentsByClass(this.classId).subscribe(
      {
        next: (response) => {
          this.students = response;
        },
        error: (err) => {

        },
        complete: () => console.log("Req is completed")
      }
    )
  }
  onClassRoomChange(event: any) {
    this.classId = event.value;
    if (this.classId) {
      this.listStudent();
    }
  }

  getClassRooms() {
    this._apiService.getClassRooms().subscribe(
      {
        next: (response) => {
          this.classRooms = response;
        },
        error: (err) => console.log(err),
        complete: () => console.log("Request is complete")
      }
    )
  }
  printSection(sectionId: string) {
    const content = document.getElementById(sectionId)?.innerHTML;
    const printWindow = window.open('', '', 'width=800,height=900');
    if (printWindow && content) {
      printWindow.document.write(`
      <html>
        <head>
          <title>Print Preview</title>
          <style>
            @page {
              size: A4;
              margin: 10mm;
            }
            body {
              font-family: "Times New Roman", serif;
              -webkit-print-color-adjust: exact;
              print-color-adjust: exact;
            }
            .a4-page {
              width: 210mm;
              min-height: 297mm;
              margin: 0 auto;
              background: white;
              padding: 10mm;
              box-sizing: border-box;
            }
          </style>
        </head>
        <body onload="window.print();window.close()">
          <div class="a4-page">
            ${content}
          </div>
        </body>
      </html>
    `);
      printWindow.document.close();
      printWindow.focus();
    }
  }
}
