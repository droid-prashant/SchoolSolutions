import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentExamSetupComponent } from './student-exam-setup.component';

describe('StudentExamSetupComponent', () => {
  let component: StudentExamSetupComponent;
  let fixture: ComponentFixture<StudentExamSetupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentExamSetupComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(StudentExamSetupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
