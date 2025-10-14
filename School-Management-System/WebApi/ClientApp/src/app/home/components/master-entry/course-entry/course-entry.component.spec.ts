import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseEntryComponent } from './course-entry.component';

describe('CourseEntryComponent', () => {
  let component: CourseEntryComponent;
  let fixture: ComponentFixture<CourseEntryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CourseEntryComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CourseEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
