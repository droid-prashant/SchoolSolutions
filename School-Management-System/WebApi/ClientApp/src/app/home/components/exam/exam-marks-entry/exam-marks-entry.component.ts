import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { CourseViewModel } from '../../course/shared/models/course.viewModel';
import { MenuItem, MessageService, PrimeIcons } from 'primeng/api';
import { Section } from '../shared/models/section.dto';
import { ExamTerminal } from '../shared/models/examTerminal.dto';
import { StudentMarksList, SubjectMarkDto } from '../shared/models/examMarksEntry.dto';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { ClassCreditCourseViewModel } from '../../course/shared/models/classCourse.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';

@Component({
  selector: 'app-exam-marks-entry',
  standalone: false,
  templateUrl: './exam-marks-entry.component.html',
  styleUrl: './exam-marks-entry.component.scss'
})
export class ExamMarksEntryComponent implements OnInit {
  classRooms: ClassRoomViewModel[] = []
  students: StudentViewModel[] = [];
  courses: ClassCreditCourseViewModel[] = [];
  sections: SectionViewModel[] = [];
  examTerminals: ExamTerminal[] = [];
  classId: string = "";

  isClassSectionSelected: boolean = false;


  studentMarks: FormGroup;

  isClassSelected: boolean = false;
  isMarksEntryVisible: boolean = false;

  constructor(private _apiService: ApiService,
    private _formBuilder: FormBuilder,
    private _messageService: MessageService
  ) {
    this.examTerminals = [
      { id: 1, terminalName: 'First Terminal' },
      { id: 2, terminalName: 'Second Terminal' },
      { id: 3, terminalName: 'Third Terminal' },
      { id: 4, terminalName: 'Final Terminal' }
    ]

    let fb = this._formBuilder;
    this.studentMarks = fb.group({
      studentId: ['', Validators.required],
      examType: ['', Validators.required],
      studentMarksLists: fb.array([], Validators.required)
    });
  }

  ngOnInit(): void {
    this.getClassRooms();
  }

  getFormGroup(index: number): FormGroup {
    return this.studentMarksLists.at(index) as FormGroup
  }

  get studentMarksLists(): FormArray {
    return this.studentMarks.get('studentMarksLists') as FormArray;
  }

  setStudentMarksFormArray() {
    const marksArray = this.studentMarks.get('studentMarksLists') as FormArray;
    this.courses.forEach(course => {
      marksArray.push(this._formBuilder.group({
        classCourseId: [course.classCreditCourseId, Validators.required],
        theoryCredit: [course.theoryCreditHour, Validators.required],
        practicalCredit: [course.practicalCreditHour, Validators.required],
        theoryFullMarks: [course.theoryFullMarks, Validators.required],
        practicalFullMarks: [course.practicalFullMarks, Validators.required],
        obtainedTheoryMarks: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
        obtainedPracticalMarks: [0, [Validators.required, Validators.min(0), Validators.max(100)]]
      }));
    });
  }

  onClassRoomChange(event: any) {
    // this.classId = event.value;
    // if (this.classId) {
    //   this.isClassSelected = true;
    //   
    //   this.listStudent();
    // }
    // else {
    //   this.isClassSelected = false;
    // }
    this.classId = event.value;
    if (this.classId) {
      this.listSubject();
      this.isClassSelected = true;
      const selectedClass = this.classRooms.filter(x => x.id === this.classId);
      const sections = selectedClass.map(x => x.sections);
      this.sections = sections[0];
    }

  }

  onClassSectionChange(event: any) {
    const classsSectionId = event.value;
    if (classsSectionId) {
      this.isClassSectionSelected = true;
      const selectedClassSection = this.sections.filter(x => x.classSectionId === classsSectionId);
      if (selectedClassSection) {
        this.getStudentByClassSection(classsSectionId);

      }
      // this.classSectionId = selectedClassSection[0].classSectionId;
    }
  }

  getStudentByClassSection(classSectionId: string) {
    this._apiService.getStudentsByClassSectionId(classSectionId).subscribe({
      next: (response) => {
        this.students = response;
      },
      error: (err) => console.log(err),
      complete: () => console.log("Request is complete")
    });
  }

  onClassSectionChanged(event: any) {

  }

  onExamTerminalChange() {

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

  listSubject() {
    this._apiService.getClassCourseByClassId(this.classId).subscribe(
      {
        next: (response) => {
          this.courses = response;
          this.setStudentMarksFormArray();
        },
        error: (error) => {

        },
        complete: () => {

        }
      }
    )
  }

  showDialog(student: StudentViewModel) {
    if (student) {
      this.studentMarks.get('studentId')?.setValue(student.id);
      this.isMarksEntryVisible = true;
    }
  }

  saveStudentMarks() {
    if (this.studentMarks.invalid) {
      return;
    }
    let studentMarks: SubjectMarkDto = this.studentMarks.value;
    this._apiService.postStudentMarks(studentMarks).subscribe(
      {
        next: (response) => {
          this._messageService.add({ severity: 'success', summary: 'Success', detail: 'successfully added marks' });
          this.isMarksEntryVisible = false;
        },
        error: (err) => {
          this._messageService.add({ severity: 'error', summary: 'Error', detail: 'failed to add marks' });
        },
        complete: () => console.log("Req is completed")
      }
    )
  }
}