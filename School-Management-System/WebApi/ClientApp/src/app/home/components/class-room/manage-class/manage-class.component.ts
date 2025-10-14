import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../shared/api.service';
import { ClassRoomViewModel } from '../shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../shared/models/viewModels/section.viewModel';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ClassSectionDto } from '../shared/models/dtos/classSection.dto';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-manage-class',
  standalone: false,
  templateUrl: './manage-class.component.html',
  styleUrl: './manage-class.component.scss'
})
export class ManageClassComponent implements OnInit {
  classList: ClassRoomViewModel[] = [];
  sections: SectionViewModel[] = [];
  classSectionList: ClassSectionDto[] = [];
  classSectionFormGroup: FormGroup

  rowIndex: number = 0;


  constructor(
    private _apiService: ApiService,
    private _formBuilder: FormBuilder,
    private _messageService: MessageService
  ) {
    let fb = _formBuilder;
    this.classSectionFormGroup = fb.group({
      classRoomId: ['', Validators.required],
      classSectionArray: fb.array([])
    });
  }
  ngOnInit(): void {
    this.listSections();
    this.listClass();
  }

  get classSectionFormArray(): FormArray {
    return this.classSectionFormGroup.get('classSectionArray') as FormArray;
  }

  getFormGroupAt(index: number): FormGroup {
    return this.classSectionFormArray.at(index) as FormGroup;
  }

  initClassSectionArray() {
    const formArray = this.classSectionFormArray;
    formArray.clear();
    this.classList.forEach(c => {
      formArray.push(this._formBuilder.group({
        classSection: [{ value: c.sections ? c.sections.map(s => s.sectionId) : [], disabled: true }, Validators.required]
      }));
    });
    console.log(formArray.controls[0].get('classSection'));
    console.log(this.sections);
  }


  listClass() {
    this._apiService.getClassRooms().subscribe({
      next: (response) => {
        this.classList = response;
        this.initClassSectionArray();
      },
      error: (err) => {

      },
      complete: () => console.log("Req Complete")
    })
  }

  listSections() {
    this._apiService.getSections().subscribe({
      next: (response) => {
        this.sections = response;
      },
      error: (error) => {
      },
      complete: () => console.log("request completed")
    });
  }

  onRowEditInit(classRoomId: string, rowIndex: number) {
    this.rowIndex = rowIndex;
    if (classRoomId) {
      const classRoomIdControl = this.classSectionFormGroup.get('classRoomId');
      classRoomIdControl?.patchValue(classRoomId);

      const classSectionControl = this.classSectionFormArray.at(this.rowIndex) as FormGroup;
      classSectionControl?.enable();
    }
  }

  onRowEditSave() {
    let formValue = this.classSectionFormGroup.value;
    const classSectionValues: ClassSectionDto = {
      classRoomId: formValue.classRoomId,
      sectionIdList: formValue.classSectionArray.map((x: any) => x.classSection).flat()
    };
    this._apiService.postClassSections(classSectionValues).subscribe({
      next: (response) => {
        this.listClass();
        this._messageService.add({ severity: 'success', summary: 'Success', detail: 'successfully mapped class and sections' });
        this.resetForm();
      },
      error: (err) => {
        this.resetForm();
        this._messageService.add({ severity: 'error', summary: 'Error', detail: 'failed to map class and sections' });
      },
      complete: () => console.log("class and section mapped")
    });
  }

  onRowEditCancel() {
    this.listClass();
    this.resetForm();
  }

  resetForm() {
    this.classSectionFormGroup.reset();
    const classSectionControl = this.classSectionFormArray.at(this.rowIndex) as FormGroup;
    classSectionControl?.disable();
    this.classSectionFormGroup.markAsPristine();
    this.classSectionFormGroup.markAsUntouched();
  }
}
