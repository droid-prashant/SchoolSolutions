import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { LookupService } from '../../../shared/common/lookup.service';
import { ClassRoomViewModel } from '../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../class-room/shared/models/viewModels/section.viewModel';
import { AcademicViewModel } from '../master-entry/model/viewModels/academicYear.ViewModel';

export interface FilterSelection {
  classId?: string;
  sectionId?: string;
  classSectionId?: string;
  academicYearId?: string;
}

@Component({
  selector: 'app-student-filter',
  standalone: true,
  imports: [CommonModule, FormsModule, DropdownModule, ButtonModule],
  templateUrl: './student-filter.component.html',
  styleUrl: './student-filter.component.scss'
})
export class StudentFilterComponent implements OnInit {
  @Input() showClassFilter: boolean = false;
  @Input() showSectionFilter: boolean = false
  @Input() showAcademicYearFilter: boolean = false;
  @Output() loadStudents = new EventEmitter<FilterSelection>();

  classOptions: ClassRoomViewModel[] = [];
  sectionOptions: SectionViewModel[] = [];
  academicYearOptions: AcademicViewModel[] = [];

  selectedClass: ClassRoomViewModel | null = null;
  selectedSection: SectionViewModel | null = null;
  selectedAcademicYear: AcademicViewModel | null = null;

  constructor(private lookupService: LookupService) { }

  ngOnInit() {
    this.loadOptions();
  }

  loadOptions() {
    if (this.showClassFilter) {
      this.lookupService.getClassRooms().subscribe(classes => {
        this.classOptions = classes;
      });
    }

    if (this.showAcademicYearFilter) {
      this.lookupService.getAcademicYears().subscribe(years => {
        this.academicYearOptions = years;
      });
    }
  }

  onClassChange(event: any) {
    const classRoomId = event.value;
    if (classRoomId) {
      this.selectedClass = this.classOptions.find(c => c.id === classRoomId) || null;
      const sections = this.classOptions.find(x => x.id === classRoomId)?.sections || [];
      if (sections.length > 0) {
        this.sectionOptions = sections.map(s => ({
          sectionId: s.sectionId,
          name: s.name,
          classSectionId: s.classSectionId
        }));
      }
    }
  }

  onSectionChange(event: any) {
    const classSectionId = event.value;
    const selectedSection = this.sectionOptions.find(s => s.classSectionId === classSectionId);
    if (selectedSection) {
      this.selectedSection = selectedSection;
    }
  }

  onAcademicYearChange() {
  }


  onLoadStudents() {
    const selection: FilterSelection = {
      classId: this.selectedClass?.id,
      sectionId: this.selectedSection?.sectionId,
      classSectionId: this.selectedSection?.classSectionId,
      academicYearId: this.selectedAcademicYear?.id
    };
    this.loadStudents.emit(selection);
  }
}
