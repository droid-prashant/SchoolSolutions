import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CourseDto } from '../../course/shared/models/course.dto';
import { CourseViewModel } from '../../course/shared/models/course.viewModel';
import { MessageService } from 'primeng/api';
import { LookupService } from '../../../../shared/common/lookup.service';

@Component({
  selector: 'app-course-entry',
  standalone: false,
  templateUrl: './course-entry.component.html',
  styleUrl: './course-entry.component.scss'
})
export class CourseEntryComponent implements OnInit {
  courseForm: FormGroup;
  courses: CourseViewModel[] = [];
  isSubmitted: boolean = false;
  isUpdate: boolean = false;
  submitButtonLabel: string = "Save";

  constructor(private _apiService: ApiService,
    private _formBuilder: FormBuilder,
    private _messageService: MessageService,
    private _lookupService: LookupService
  ) {
    let fb = _formBuilder;
    this.courseForm = fb.group({
      id: [''],
      name: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.listCourse();
  }

  submit() {
    if (!this.isUpdate) {
      this.addCourse();
    }
    else {
      this.updateCourse();
    }
  }

  addCourse() {
    this.isSubmitted = true;
    if (!this.courseForm.valid) {
      return;
    }
    let course: CourseDto = this.courseForm.value;
    this._apiService.postCourse(course).subscribe(
      {
        next: (response) => {
          this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Added Course' });
        },
        error: (err) => {
          this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to add Course' });
        },
        complete: () => {
          this.listCourse();
          this.isSubmitted = false;
          this.courseForm.reset();
        }
      }
    )
  }

  updateCourse() {
    this.isSubmitted = true;
    if (!this.courseForm.valid) {
      return;
    }
    let course: CourseDto = this.courseForm.value;
    this._apiService.updateCourse(course).subscribe(
      {
        next: (response) => {
          this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Added Course' });
        },
        error: (err) => {
          this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to add Course' });
        },
        complete: () => {
          this.listCourse(true);
          this.isSubmitted = false;
          this.isUpdate = false;
          this.submitButtonLabel = "Save";
          this.courseForm.reset();
        }
      }
    )
  }

  listCourse(forceRefresh = false) {
    this._lookupService.getCourses(forceRefresh).subscribe({
      next: (response) => {
        this.courses = response;
      }
    });
  }
  editCourse(course: CourseViewModel) {
    this.isUpdate = true;
    this.courseForm.patchValue(course);
    this.submitButtonLabel = "Update";
  }
}
