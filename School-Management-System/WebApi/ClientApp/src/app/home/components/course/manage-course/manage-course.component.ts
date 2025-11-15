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
  masterCourses: CourseViewModel[] = []
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

  onClassChange(event: any) {
    const classRoomId = event.value;
    if (classRoomId) {
      this.classCourseForm.get('classRoomId')?.setValue(classRoomId);
      this.listClassCourseByClassId(classRoomId);
    }
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

  listClassCourseByClassId(classCourseId: string) {
    this._apiService.getClassCourseByClassId(classCourseId).subscribe(
      {
        next: (response) => {
          this.classCourses = response;
          this.filterCourse();
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
          this.masterCourses = response;
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
          const classId = this.classCourseForm.get('classRoomId')?.value;
          this.listClassCourseByClassId(classId);
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

  filterCourse() {
    if (this.classCourses.length > 0) {
      const classCourses = this.classCourses.map(x => x.courseId);
      const filteredCourses = this.masterCourses.filter(x => !classCourses.includes(x.id));
      this.courses = filteredCourses;
    }
  }

  reset() {
    const classId = this.classCourseForm.get('classRoomId')?.value;
    this.classCourseForm.reset({
      classRoomId: classId,
      courseId: '',
      theoryCreditHour: '',
      practicalCreditHour: '',
      theoryFullMarks: '',
      practicalFullMarks: '',
    });
    this.isNewRow = false;
    this.isUpdateRow = false;
  }
}
