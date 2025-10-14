import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { SectionDto } from '../model/dtos/section.dto';

@Component({
  selector: 'app-section-entry',
  standalone: false,
  templateUrl: './section-entry.component.html',
  styleUrl: './section-entry.component.scss'
})
export class SectionEntryComponent implements OnInit {

  sectionForm: FormGroup
  sections: SectionViewModel[] = [];
  constructor(private _apiService: ApiService,
    private _formBuilder: FormBuilder
  ) {
    let fb = this._formBuilder;
    this.sectionForm = fb.group({
      name: ['', Validators.required]
    });
  }
  ngOnInit(): void {
    this.listSection();
  }
  addSection() {
    const section: SectionDto = this.sectionForm.value;
    this._apiService.addSection(section).subscribe({
      next: (response) => {
        this.listSection();
        this.sectionForm.reset();
      },
      error: (err) => {

      },
      complete: () => console.log("Request complete")
    });
  }

  listSection() {
    this._apiService.getSections().subscribe({
      next: (response) => {
        this.sections = response;
      },
      error: (err) => {

      },
      complete: () => { }
    });
  }

  editSection(section: SectionDto) {

  }
}
