import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageFeesTypesComponent } from './manage-fees-types.component';

describe('ManageFeesTypesComponent', () => {
  let component: ManageFeesTypesComponent;
  let fixture: ComponentFixture<ManageFeesTypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageFeesTypesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ManageFeesTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
