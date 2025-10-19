import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ApiService } from '../../../shared/api.service';
import { StudentViewModel } from '../student/shared/models/viewModels/student.viewModel';
import { ClassRoomViewModel } from '../class-room/shared/models/viewModels/classRoom.viewModel';

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
    classId: string = "";
    students: StudentViewModel[] = [];
    classRooms: ClassRoomViewModel[] = []

    constructor(private _apiService: ApiService) {

    }

    ngOnInit(): void {
        this.getClassRooms();
    }

    listStudent() {
        this._apiService.getStudentsByClass(this.classId).subscribe(
            {
                next: (response) => {
                    this.students = response;
                },
                error: (err) => {

                },
                complete: () => console.log("Req is completed")
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

    onClassRoomChange(event: any) {
        this.classId = event.value;
        if (this.classId) {
            this.listStudent();
        }
    }

    previewCharacterCertificate(student: StudentViewModel) {
        this.showResult = true;
        this.isCharacterCertificate = true;
    }

    previewTransferCertificate(student: StudentViewModel) {
        this.showResult = true;
        this.isTransferCertificate = true;
    }

    onDialogClose() {
        this.isCharacterCertificate = false;
        this.isTransferCertificate = false;
    }
}
