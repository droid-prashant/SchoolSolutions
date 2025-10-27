import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AcademicYearEntryComponent } from './academic-year-entry.component';

describe('AcademicYearEntryComponent', () => {
  let component: AcademicYearEntryComponent;
  let fixture: ComponentFixture<AcademicYearEntryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AcademicYearEntryComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AcademicYearEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
