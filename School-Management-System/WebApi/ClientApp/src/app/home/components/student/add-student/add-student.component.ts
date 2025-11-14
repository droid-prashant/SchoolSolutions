import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../../../shared/api.service';
import { Router } from '@angular/router';
import { ClassRoomViewModel } from '../../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';
import { StudentDto } from '../shared/models/dtos/student.dto';
import { ProvinceViewModel } from '../../../../shared/common/models/master/master.ViewModel';
import { DistrictViewModel } from '../../../../shared/common/models/master/district.ViewModel';
import { MunicipalityViewModel } from '../../../../shared/common/models/master/municipality.ViewModel';

@Component({
  selector: 'app-add-student',
  standalone: false,
  templateUrl: './add-student.component.html',
  styleUrl: './add-student.component.scss'
})
export class AddStudentComponent implements OnInit {
  genders: any[] = []
  classRooms: ClassRoomViewModel[] = []
  sections: SectionViewModel[] = [];
  provinceDetails: ProvinceViewModel[] = [];
  selectedDistricts: DistrictViewModel[] = [];
  selectedMunicipalities: MunicipalityViewModel[] = [];

  selectedClass!: ClassRoomViewModel
  studentForm: FormGroup
  constructor(private _fb: FormBuilder,
    private _apiService: ApiService,
    private _router: Router
  ) {
    let fb = this._fb;
    this.studentForm = fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      grandFatherName: ['', Validators.required],
      fatherName: ['', Validators.required],
      motherName: ['', Validators.required],
      gender: [null, Validators.required],
      age: [null, [Validators.required, Validators.min(1)]],
      address: ['', Validators.required],
      dobNp: ['', Validators.required],
      dobEn: ['', Validators.required],
      wardNo: ['', Validators.required],
      contactNumber: ['', Validators.required],
      parentContactNumber: ['', Validators.required],
      parentEmail: ['', Validators.required],
      classSectionId: [null, Validators.required],
      provinceId: [null, Validators.required],
      districtId: [null, Validators.required],
      municipalityId: [null, Validators.required]
    });
  }
  ngOnInit(): void {
    this.genders = [
      { label: 'Male', value: 1 },
      { label: 'Female', value: 2 },
      { label: 'Other', value: 3 }
    ];
    this.getClassRooms();
    this.getProvinceDetails();
    this.studentForm.get('provinceId')?.valueChanges.subscribe(value => {
      this.onProvinceChange(value);
    })
    this.studentForm.get('districtId')?.valueChanges.subscribe(value => {
      if (value != null) {
        this.onDistrictChange(value);
      }
    })
  }

  onDobChange(event: { bs: string; ad: string }) {
    const dobNp = event.bs;
    const dobEn = event.ad;
    if (dobNp && dobEn) {
      this.studentForm.get('dobNp')?.setValue(dobNp);
      this.studentForm.get('dobEn')?.setValue(dobNp);
    }
  }

  getProvinceDetails() {
    this._apiService.getProvinceDetails().subscribe(
      {
        next: (response) => {
          this.provinceDetails = response;
          console.log()
        },
        error: (err) => console.log(err),
        complete: () => console.log("Request is complete")
      }
    )
  }

  getClassRooms() {
    this._apiService.getClassRooms().subscribe(
      {
        next: (response) => {
          this.classRooms = response;
        },
        error: (err) => console.log(err),
        complete: () => console.log("Request is complete")
      }
    )
  }

  addStudent() {
    let student: StudentDto = this.studentForm.value;
    this._apiService.postStudent(student).subscribe(
      {
        next: (response) => this._router.navigateByUrl("/home/student/list-student"),
        error: (err) => console.log(err),
        complete: () => console.log("Request is complete")
      }
    )
  }

  onClassRoomChange(event: any) {
    const classRoomId = event.value;
    if (classRoomId) {
      const selectedClass = this.classRooms.filter(x => x.id === classRoomId);
      const sections = selectedClass.map(x => x.sections);
      this.sections = sections[0];
    }
  }

  onSectionChange(event: any) {
    const sectionId = event.value;
    if (sectionId) {
      const selectedClassSection = this.sections.filter(x => x.sectionId === sectionId);
      const selectedCLassSectionId = selectedClassSection[0].classSectionId;
      this.studentForm.get('classSectionId')?.setValue(selectedCLassSectionId);
    }
  }

  onProvinceChange(provinceId: any) {
    this.studentForm.get('districtId')?.patchValue(null);
    this.studentForm.get('municipalityId')?.patchValue(null);
    if (provinceId) {
      const selectedProvince = this.provinceDetails.find(x => x.id == provinceId)
      if (selectedProvince) {
        this.selectedDistricts = selectedProvince.districts;
      }
    }
  }

  onDistrictChange(districtId: any) {
    this.studentForm.get('')
    if (districtId) {
      const selectedMunicipalities = this.selectedDistricts.find(x => x.id == districtId)
      if (selectedMunicipalities) {
        this.selectedMunicipalities = selectedMunicipalities.municipalities;
      }
    }
  }
}
