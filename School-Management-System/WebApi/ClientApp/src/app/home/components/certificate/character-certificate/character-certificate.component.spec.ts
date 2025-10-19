import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CharacterCertificateComponent } from './character-certificate.component';

describe('CharacterCertificateComponent', () => {
  let component: CharacterCertificateComponent;
  let fixture: ComponentFixture<CharacterCertificateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CharacterCertificateComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CharacterCertificateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
