import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { ApiService } from '../../../../shared/api.service';
import { StudentViewModel } from '../shared/models/viewModels/student.viewModel';
import { StudentFilterComponent, FilterSelection } from '../../student-filter/student-filter.component';
import { LookupService } from '../../../../shared/common/lookup.service';
import { Section } from '../../exam/shared/models/section.dto';
import { SectionViewModel } from '../../class-room/shared/models/viewModels/section.viewModel';

@Component({
  selector: 'app-list-student',
  standalone: false,
  templateUrl: './list-student.component.html',
  styleUrl: './list-student.component.scss'
})
export class ListStudentComponent implements OnInit {
  students: StudentViewModel[] = [];
  filteredStudents: StudentViewModel[] = [];
  sections: SectionViewModel[] = [];
  isAddDialogVisible: boolean = false;
  isEditDialogVisible: boolean = false;
  selectedStudent: StudentViewModel | null = null;
  currentFilter: FilterSelection = {};
  searchText: string = '';

  constructor(private _apiService: ApiService, private _messageService: MessageService, private _lookupService: LookupService) {
  }
  ngOnInit(): void {
  }

  editStudent(student: StudentViewModel) {
    this.selectedStudent = { ...student };
    this.isEditDialogVisible = true;
  }

  openAddStudentDialog() {
    this.isAddDialogVisible = true;
  }

  getGender(genderId: number): string {
    switch (genderId) {
      case 1:
        return 'Male';
      case 2:
        return 'Female';
      case 3:
        return 'Other';
      default:
        return 'Unknown';
    }
  }

  onSearchChange() {
    this.applyFilters();
  }

  applyFilters() {
    let filtered = this.students;

    // Apply filter criteria
    if (this.currentFilter.classId) {
      filtered = filtered.filter(s => s.classRoomId === this.currentFilter.classId);
    }
    if (this.currentFilter.sectionId) {
      filtered = filtered.filter(s => s.sectionId === this.currentFilter.sectionId);
    }

    // Apply search text
    if (this.searchText.trim()) {
      const searchLower = this.searchText.toLowerCase();
      filtered = filtered.filter(student =>
        student.firstName?.toLowerCase().includes(searchLower) ||
        student.lastName?.toLowerCase().includes(searchLower) ||
        student.classRoomName?.toLowerCase().includes(searchLower) ||
        student.sectionName?.toLowerCase().includes(searchLower) ||
        student.address?.toLowerCase().includes(searchLower) ||
        student.fatherName?.toLowerCase().includes(searchLower) ||
        student.motherName?.toLowerCase().includes(searchLower)
      );
    }

    this.filteredStudents = filtered;
  }

  onLoadStudents(filter: FilterSelection) {
    this.currentFilter = filter;
    if (filter && filter.classSectionId) {
      this.listStudentByClassSection(filter.classSectionId);
    }
    else if (filter && filter.classId) {
      this.listStudentByClass(filter.classId);
    }
  }

  listStudentByClassSection(classSectionId: string) {
    this._apiService.getStudentsByClassSectionId(classSectionId).subscribe(
      {
        next: (response) => {
          this.students = response;
          this.applyFilters();
        },
        error: (err) => console.log(err)
      }
    );
  }

  listStudentByClass(classId: string) {
    this._apiService.getStudentsByClass(classId).subscribe(
      {
        next: (response) => {
          this.students = response;
          this.applyFilters();
        },
        error: (err) => console.log(err)
      }
    );
  }

  getClassName(classRoomId: string): string {
    let classRoom: string = 'Unknown';
    this._lookupService.getClassRooms().subscribe(classes => {
      classRoom = classes.find(s => s.id === classRoomId)?.name || 'Unknown';
      this.sections = classes.find(s => s.id === classRoomId)?.sections || [];
    });
    return classRoom;
  }

  getSectionName(sectionId: string): string {
    const section = this.sections.find(s => s.sectionId === sectionId);
    return section ? section.name : 'Unknown';
  }

  onStudentSaved() {
    if (this.isEditDialogVisible) {
      this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Student updated successfully' });
    } else if (this.isAddDialogVisible) {
      this._messageService.add({ severity: 'success', summary: 'Success', detail: 'Student added successfully' });
    }
    this.onLoadStudents(this.currentFilter);
    this.isEditDialogVisible = false;
    this.isAddDialogVisible = false;
    this.selectedStudent = null;
  }
  // listStudent() {
  //   this._apiService.getStudents().subscribe(
  //     {
  //       next: (response) => {
  //         this.students = response;
  //         this.filteredStudents = response; // Initialize filtered
  //         console.log(this.students);
  //       },
  //       error: (err) => {

  //       },
  //       complete: () => console.log("Req is completed")
  //     }
  //   )
  // }
}
