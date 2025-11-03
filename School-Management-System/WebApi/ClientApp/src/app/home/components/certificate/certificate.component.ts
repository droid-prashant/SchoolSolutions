import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ApiService } from '../../../shared/api.service';
import { StudentViewModel } from '../student/shared/models/viewModels/student.viewModel';
import { ClassRoomViewModel } from '../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../class-room/shared/models/viewModels/section.viewModel';

@Component({
    selector: 'app-certificate',
    standalone: false,
    templateUrl: './certificate.component.html',
    styleUrls: ['./certificate.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class CertificateComponent implements OnInit {
    isCharacterCertificate: boolean = false;
    isTransferCertificate: boolean = false;
    showResult: boolean = false;
    isClassSelected: boolean = false;
    classId: string = "";
    students: StudentViewModel[] = [];
    student!: StudentViewModel;
    classRooms: ClassRoomViewModel[] = []
    sections: SectionViewModel[] = [];

    constructor(private _apiService: ApiService) {

    }

    ngOnInit(): void {
        this.getClassRooms();
    }

    // listStudent() {
    //     this._apiService.getStudentsByClass(this.classId).subscribe(
    //         {
    //             next: (response) => {
    //                 this.students = response;
    //             },
    //             error: (err) => {

    //             },
    //             complete: () => console.log("Req is completed")
    //         }
    //     )
    // }
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

    // onClassRoomChange(event: any) {
    //     this.classId = event.value;
    //     if (this.classId) {
    //         this.listStudent();
    //     }
    // }

    onClassRoomChange(event: any) {
        this.classId = event.value;
        if (this.classId) {
            this.isClassSelected = true;
            const selectedClass = this.classRooms.filter(x => x.id === this.classId);
            const sections = selectedClass.map(x => x.sections);
            this.sections = sections[0];
        }

    }

    onClassSectionChange(event: any) {
        const classsSectionId = event.value;
        if (classsSectionId) {
            this.isClassSelected = true;
            const selectedClassSection = this.sections.filter(x => x.classSectionId === classsSectionId);
            if (selectedClassSection) {
                this.getStudentByClassSection(classsSectionId);

            }
            // this.classSectionId = selectedClassSection[0].classSectionId;
        }
    }

    getStudentByClassSection(classSectionId: string) {
        this._apiService.getStudentsByClassSectionId(classSectionId).subscribe({
            next: (response) => {
                this.students = response;
            },
            error: (err) => console.log(err),
            complete: () => console.log("Request is complete")
        });
    }

    previewCharacterCertificate(student: StudentViewModel) {
        this.student = student;
        this.showResult = true;
        this.isCharacterCertificate = true;
    }

    previewTransferCertificate(student: StudentViewModel) {
        this.student = student;
        this.showResult = true;
        this.isTransferCertificate = true;
    }

    onDialogClose() {
        this.isCharacterCertificate = false;
        this.isTransferCertificate = false;
    }
}
