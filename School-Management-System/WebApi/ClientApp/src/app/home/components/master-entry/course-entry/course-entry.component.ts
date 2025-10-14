import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CourseDto } from '../../course/shared/models/course.dto';
import { CourseViewModel } from '../../course/shared/models/course.viewModel';

@Component({
  selector: 'app-course-entry',
  standalone: false,
  templateUrl: './course-entry.component.html',
  styleUrl: './course-entry.component.scss'
})
export class CourseEntryComponent implements OnInit {
  courseForm: FormGroup;
  courses: CourseViewModel[] = [];

  constructor(private _apiService: ApiService,
    private _formBuilder: FormBuilder
  ) {
    let fb = _formBuilder;
    this.courseForm = fb.group({
      name: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.listCourse();
  }

  addCourse() {
    let course: CourseDto = this.courseForm.value;
    this._apiService.postCourse(course).subscribe(
      {
        next: (response) => {
          this.listCourse();
          this.courseForm.reset();
        },
        error: (err) => console.log(err),
        complete: () => { }
      }
    )
  }

  listCourse() {
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
  editCourse(course: CourseViewModel) {

  }
}
