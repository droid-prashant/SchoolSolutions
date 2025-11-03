import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { MessageService } from 'primeng/api';
import { StudentEnrollmentViewModel } from '../shared/viewModels/studentEnrollment.viewModel';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-student-exam-setup',
  standalone: false,
  templateUrl: './student-exam-setup.component.html',
  styleUrl: './student-exam-setup.component.scss'
})
export class StudentExamSetupComponent implements OnInit {
  classRooms: ClassRoomViewModel[] = []
  sections: SectionViewModel[] = [];
  enrolledStudents: StudentEnrollmentViewModel[] = [];

  studentExamSetupForm!: FormGroup
  classSectionId: string = "";
  studentEnrollmentId: string = "";
  rowIndex: number = 0;

  isClassSelected: boolean = false;
  constructor(private _fb: FormBuilder, private _apiService: ApiService, private _messageService: MessageService) {
    this.studentExamSetupForm = _fb.group({
      formArray: _fb.array([])
    });
  }
  ngOnInit(): void {
    this.getClassRooms();
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

  get formArray(): FormArray {
    return this.studentExamSetupForm.get('formArray') as FormArray;
  }

  getFormGroupAt(index: number): FormGroup {
    return this.formArray.at(index) as FormGroup;
  }

  initFormArray() {
    const formArray = this.formArray;
    formArray.clear();
    this.enrolledStudents.forEach(x => {
      formArray.push(this._fb.group({
        registrationNumber: [{ value: x.registrationNumber, disabled: true }, Validators.required],
        symbolNumber: [{ value: x.symbolNumber, disabled: true }, Validators.required]
      }));
    });
  }

  listEnrolledStudents(classSectionId: string) {
    this._apiService.getRegAndSymCompliantStudent(classSectionId).subscribe({
      next: (res) => {
        this.enrolledStudents = res;
        this.initFormArray()
      },
      error: (err) => {

      },
      complete: () => { }
    })
  }

  onClassRoomChange(event: any) {
    const classId = event.value;
    if (classId) {
      this.isClassSelected = true;
      const selectedClass = this.classRooms.filter(x => x.id === classId);
      const sections = selectedClass.map(x => x.sections);
      this.sections = sections[0];
    }

  }

  onClassSectionChange(event: any) {
    const classsSectionId = event.value;
    if (classsSectionId) {
      this.classSectionId = classsSectionId;
      this.isClassSelected = true;
      const selectedClassSection = this.sections.filter(x => x.classSectionId === classsSectionId);
      if (selectedClassSection) {
        this.listEnrolledStudents(classsSectionId);
      }
    }
  }

  onRowEditInit(studentEnrollmentId: string, rowIndex: number) {
    if (studentEnrollmentId) {
      this.studentEnrollmentId = studentEnrollmentId;
      this.rowIndex = rowIndex;
      const formRow= this.formArray.at(rowIndex) as FormGroup;
      formRow?.enable();
    }
  }

  onRowEditSave() {
    const formValue: StudentEnrollmentViewModel = this.formArray.at(this.rowIndex).value;
    this._apiService.assignRegistrationAndSymbolNumber(formValue, this.studentEnrollmentId).subscribe({
      next: (response) => {
        this.listEnrolledStudents(this.classSectionId);
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'successfully mapped class and sections' });
        this.resetForm();
      },
      error: (err) => {
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'failed to map class and sections' });
      },
      complete: () => { }
    });
  }

  onRowEditCancel() {
    this.listEnrolledStudents(this.classSectionId);
    this.resetForm();
  }

  resetForm() {
    this.studentExamSetupForm.reset();
    this.studentExamSetupForm.markAsPristine();
    this.studentExamSetupForm.markAsUntouched();
    this.studentExamSetupForm.updateValueAndValidity();
  }

}
