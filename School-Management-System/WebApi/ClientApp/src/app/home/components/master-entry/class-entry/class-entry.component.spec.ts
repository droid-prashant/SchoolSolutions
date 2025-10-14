import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassEntryComponent } from './class-entry.component';

describe('ClassEntryComponent', () => {
  let component: ClassEntryComponent;
  let fixture: ComponentFixture<ClassEntryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClassEntryComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ClassEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
