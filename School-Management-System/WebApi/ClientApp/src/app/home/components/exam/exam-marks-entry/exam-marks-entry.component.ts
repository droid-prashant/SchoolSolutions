import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StudentViewModel } from '../../student/shared/models/viewModels/student.viewModel';
import { ExamTerminal } from '../shared/models/examTerminal.dto';
import { SubjectMarkDto } from '../shared/models/examMarksEntry.dto';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { ClassCreditCourseViewModel } from '../../course/shared/models/classCourse.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { MessageService } from 'primeng/api';
import { FilterSelection } from '../../student-filter/student-filter.component';
import { LookupService } from '../../../../shared/common/lookup.service';

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
  currentFilter: FilterSelection = {};
  submitButtonLabel: string = "Save";
  isClassSectionSelected: boolean = false;
  isStudentMarksUpdate: boolean = false;


  studentMarks: FormGroup;

  isClassSelected: boolean = false;
  isMarksEntryVisible: boolean = false;

  constructor(private _apiService: ApiService,
    private _formBuilder: FormBuilder,
    private _messageService: MessageService,
    private _lookupService: LookupService
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
      attendance: [null, Validators.required],
      totalSchoolDays: [null, Validators.required],
      studentMarksLists: fb.array([], Validators.required)
    });
  }

  ngOnInit(): void {
    // this.getClassRooms();
  }

  getFormGroup(index: number): FormGroup {
    return this.studentMarksLists.at(index) as FormGroup
  }

  get studentMarksLists(): FormArray {
    return this.studentMarks.get('studentMarksLists') as FormArray;
  }

  createStudentMarksFormArray() {
    const marksArray = this.studentMarks.get('studentMarksLists') as FormArray;
    marksArray.clear();
    this.courses.forEach(course => {
      marksArray.push(this._formBuilder.group({
        classCourseId: [course.classCreditCourseId, Validators.required],
        theoryCredit: [course.theoryCreditHour, Validators.required],
        practicalCredit: [course.practicalCreditHour, Validators.required],
        theoryFullMarks: [course.theoryFullMarks, Validators.required],
        practicalFullMarks: [course.practicalFullMarks, Validators.required],
        obtainedTheoryMarks: [0, [Validators.required, Validators.min(0), Validators.max(course.theoryFullMarks)]],
        obtainedPracticalMarks: [0, [Validators.required, Validators.min(0), Validators.max(course.practicalFullMarks)]]
      }));
    });
  }

  onLoadStudents(filter: FilterSelection) {
    this.currentFilter = filter;
    if (filter && filter.classSectionId && filter.classId) {
      this.listSubject(filter.classId)
      this.listStudentByClassSection(filter.classSectionId);
    }
  }

  listStudentByClassSection(classSectionId: string) {
    this._apiService.getStudentsByClassSectionId(classSectionId).subscribe(
      {
        next: (response) => {
          this.students = response;
        },
        error: (err) => console.log(err)
      }
    );
  }

  // onClassRoomChange(event: any) {
  //   this.classId = event.value;
  //   if (this.classId) {
  //     this.listSubject();
  //     this.isClassSelected = true;
  //     const selectedClass = this.classRooms.filter(x => x.id === this.classId);
  //     const sections = selectedClass.map(x => x.sections);
  //     this.sections = sections[0];
  //   }

  // }

  onClassSectionChange(event: any) {
    const classsSectionId = event.value;
    if (classsSectionId) {
      this.isClassSectionSelected = true;
      const selectedClassSection = this.sections.filter(x => x.classSectionId === classsSectionId);
      if (selectedClassSection) {
        this.getStudentByClassSection(classsSectionId);
      }
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

  listSubject(classId: string) {
    this._apiService.getClassCourseByClassId(classId).subscribe(
      {
        next: (response) => {
          this.courses = response;
          this.createStudentMarksFormArray();
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
      const studentEnrollmentId = student.studentEnrollmentId;
      this.getStudentMarks(studentEnrollmentId);
      this.studentMarks.get('studentId')?.setValue(studentEnrollmentId);
      this.isMarksEntryVisible = true;
    }
  }

  getStudentMarks(studentEnrollmentId: string) {
    const examType = this.studentMarks.get('examType')?.value;
    if (examType) {
      this._apiService.getStudentMarks(studentEnrollmentId, examType).subscribe(
        {
          next: (response) => {
            if (response) {
              this.studentMarks.patchValue({
                studentId: response.studentId,
                examType: response.examType,
                attendance: response.attendance,
                totalSchoolDays: response.totalSchoolDays
              });
              this.patchStudentMarksLists(response.studentMarksLists);
            }
          },
          error: (error) => {
            console.error(error);
          }
        });
    }

  }

  patchStudentMarksLists(studentMarksLists: any[]) {
    const marksArray = this.studentMarks.get('studentMarksLists') as FormArray;

    if (!studentMarksLists || !studentMarksLists.length) {
      return;
    }
    this.submitButtonLabel = "Update";
    this.isStudentMarksUpdate = true;
    studentMarksLists.forEach((item) => {
      const index = marksArray.controls.findIndex(control =>
        control.get('classCourseId')?.value === item.classCourseId
      );

      if (index !== -1) {
        const group = marksArray.at(index) as FormGroup;

        group.patchValue({
          classCourseId: item.classCourseId,
          theoryCredit: item.theoryCredit,
          practicalCredit: item.practicalCredit,
          theoryFullMarks: item.theoryFullMarks,
          practicalFullMarks: item.practicalFullMarks,
          obtainedTheoryMarks: item.obtainedTheoryMarks,
          obtainedPracticalMarks: item.obtainedPracticalMarks
        });
      }
    });

    console.log(this.studentMarks.value);
  }

  save() {
    if (this.isStudentMarksUpdate) {
      this.updateStudentMarks();
    } else {
      this.saveStudentMarks();
    }
  }

  updateStudentMarks() {
    if (this.studentMarks.invalid) {
      return;
    }
    let studentMarks: SubjectMarkDto = this.studentMarks.value;
    this._apiService.updateStudentMarks(studentMarks).subscribe(
      {
        next: (response) => {
          this._messageService.add({ severity: 'success', summary: 'Success', detail: 'successfully updated marks' });
          this.isMarksEntryVisible = false;
          this.resetForm();
        },
        error: (err) => {
          this._messageService.add({ severity: 'error', summary: 'Error', detail: 'failed to update marks' });
        }
      }
    )
  }

  resetForm() {
    this.studentMarks.reset();

    this.studentMarks.patchValue({
      studentId: '',
      examType: '',
      attendance: null,
      totalSchoolDays: null
    });

    const marksArray = this.studentMarks.get('studentMarksLists') as FormArray;
    marksArray.clear();

    this.createStudentMarksFormArray();
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
          this.resetForm();
        },
        error: (err) => {
          this._messageService.add({ severity: 'error', summary: 'Error', detail: 'failed to add marks' });
        }
      }
    )
  }

  getClassName(classRoomId: string): string {
    let classRoom: string = 'Unknown';
    this._lookupService.getClassRooms().subscribe(classes => {
      classRoom = classes.find(s => s.id === classRoomId)?.name || 'Unknown';
      this.sections = classes.find(s => s.id === classRoomId)?.sections || [];
    });
    return classRoom;
  }

  getSectionName(sectionId: string): string {
    const section = this.sections.find(s => s.sectionId === sectionId);
    return section ? section.name : 'Unknown';
  }

}