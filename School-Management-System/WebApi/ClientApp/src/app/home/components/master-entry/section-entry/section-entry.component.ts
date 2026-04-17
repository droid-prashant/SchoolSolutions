import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { SectionDto } from '../model/dtos/section.dto';
import { MessageService } from 'primeng/api';
import { LookupService } from '../../../../shared/common/lookup.service';

@Component({
  selector: 'app-section-entry',
  standalone: false,
  templateUrl: './section-entry.component.html',
  styleUrl: './section-entry.component.scss'
})
export class SectionEntryComponent implements OnInit {
  sectionForm: FormGroup
  sections: SectionViewModel[] = [];
  isUpdate: boolean = false;
  isSubmitted: boolean = false;
  submitButtonLable: string = "Save";

  constructor(private _apiService: ApiService,
    private _formBuilder: FormBuilder,
    private _messageService: MessageService,
    private _lookupService: LookupService
  ) {
    let fb = this._formBuilder;
    this.sectionForm = fb.group({
      sectionId: [''],
      name: ['', Validators.required]
    });
  }
  ngOnInit(): void {
    this.listSection();
  }

  submit() {
    if (!this.isUpdate) {
      this.addSection();
    }
    else {
      this.updateSection();
    }
  }

  addSection() {
    this.isSubmitted = true;
    if (!this.sectionForm.valid) {
      return;
    }
    const section: SectionDto = this.sectionForm.value;
    this._apiService.addSection(section).subscribe({
      next: (response) => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Added Section' });
        this.listSection(true);
        this.sectionForm.reset();
      },
      error: (err) => {
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to add Section' });
      },
      complete: () => {
        this.isSubmitted = false;
      }
    });
  }

  updateSection() {
    this.isSubmitted = true;
    if (!this.sectionForm.valid) {
      return;
    }
    const section: SectionDto = this.sectionForm.value;
    this._apiService.updateSection(section).subscribe({
      next: (response) => {
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Added Section' });
      },
      error: (err) => {
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to add Section' });
      },
      complete: () => {
        this.isSubmitted = false;
        this.isUpdate = false;
        this.submitButtonLable = "Save";
        this.listSection(true);
        this.sectionForm.reset();
      }
    });
  }

  listSection(forceRefresh = false) {
    this._lookupService.getAllSections(forceRefresh).subscribe({
      next: (response) => {
        this.sections = response;
      },
      error: (err) => {
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load section list' });
      },
      complete: () => { }
    });
  }

  editSection(section: SectionDto) {
    this.isUpdate = true;
    this.submitButtonLable = "Update"
    this.sectionForm.patchValue(section);
  }
}
