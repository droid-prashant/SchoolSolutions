import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SectionEntryComponent } from './section-entry.component';

describe('SectionEntryComponent', () => {
  let component: SectionEntryComponent;
  let fixture: ComponentFixture<SectionEntryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SectionEntryComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SectionEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
