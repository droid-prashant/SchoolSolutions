import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { CourseViewModel } from '../shared/models/course.viewModel';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClassCreditCourseViewModel } from '../shared/models/classCourse.viewModel';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { ClassCourseDto } from '../shared/models/classCourse.dto';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-manage-course',
  standalone: false,
  templateUrl: './manage-course.component.html',
  styleUrl: './manage-course.component.scss'
})
export class ManageCourseComponent implements OnInit {
  courses: CourseViewModel[] = []
  classCourses: ClassCreditCourseViewModel[] = [];
  classCourseForm: FormGroup
  classRooms: ClassRoomViewModel[] = [];
  classCourseId: string = "";

  isNewRow: boolean = false;
  isUpdateRow: boolean = false;
  constructor(
    private _apiService: ApiService,
    private _formBuilder: FormBuilder,
    private _messageService: MessageService
  ) {
    let fb = this._formBuilder;
    this.classCourseForm = fb.group({
      classRoomId: ['', Validators.required],
      courseId: ['', Validators.required],
      theoryCreditHour: ['', Validators.required],
      practicalCreditHour: ['', Validators.required],
      theoryFullMarks: ['', Validators.required],
      practicalFullMarks: ['', Validators.required],
    });
  }
  ngOnInit(): void {
    this.listAllClassCourse();
    this.listCourses();
    this.listClass();
  }

  listClass() {
    this._apiService.getClassRooms().subscribe({
      next: (response) => {
        this.classRooms = response;
      },
      error: (err) => {

      }
    });
  }

  listAllClassCourse() {
    this._apiService.getAllClassCourse().subscribe(
      {
        next: (response) => {
          this.classCourses = response;
        },
        error: (error) => {

        },
        complete: () => {

        }
      }
    )
  }

  listCourses() {
    this._apiService.getCourses().subscribe(
      {
        next: (response) => {
          this.courses = response;
        },
        error: (error) => {

        },
        complete: () => {

        }
      }
    )
  }

  addNewRow() {
    this.isNewRow = true;
  }

  onRowSubmit() {
    const classCourseDto: ClassCourseDto = this.classCourseForm.value;
    if (this.isUpdateRow && !this.isNewRow) {
      this.onRowUpdate(classCourseDto);
    }
    else {
      this.onRowSave(classCourseDto);
    }

    this.isNewRow = false;
  }

  onRowUpdate(classCourseDto: ClassCourseDto) {
    classCourseDto.classCourseId = this.classCourseId;
    this._apiService.putClassCourse(classCourseDto).subscribe(
      {
        next: (response) => {
          this._messageService.add({ severity: 'success', summary: 'Success', detail: 'successfully updated class and course' });
          this.listAllClassCourse();
          this.reset();
        },
        error: (error) => {
          this._messageService.add({ severity: 'error', summary: 'Error', detail: 'failed to update class and course' });
          this.reset();
        },
        complete: () => {

        }
      });
  }

  onRowSave(classCourseDto: ClassCourseDto) {
    this._apiService.postClassCourse(classCourseDto).subscribe(
      {
        next: (response) => {
          this._messageService.add({ severity: 'success', summary: 'Success', detail: 'successfully configured class and course' });
          this.listAllClassCourse();
          this.reset();
        },
        error: (error) => {
          this._messageService.add({ severity: 'error', summary: 'Error', detail: 'failed to configured class and course' });
          this.reset();
        },
        complete: () => {

        }
      });
  }
  onRowEdit(classCourse: ClassCreditCourseViewModel) {
    this.classCourseId = classCourse.classCreditCourseId;
    this.classCourseForm.reset();
    this.patchClassCourseForm(classCourse);
    this.isUpdateRow = true;
  }

  patchClassCourseForm(classCourse: ClassCreditCourseViewModel) {
    this.classCourseForm.patchValue(classCourse);
  }

  onRowCancel() {
    this.isNewRow = false;
    this.reset();
  }

  reset() {
    this.classCourseForm.reset();
    this.isNewRow = false;
    this.isUpdateRow = false;
  }
}
