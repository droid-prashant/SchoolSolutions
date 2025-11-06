import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ApiService } from '../../../shared/api.service';
import { StudentViewModel } from '../student/shared/models/viewModels/student.viewModel';
import { ClassRoomViewModel } from '../class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../class-room/shared/models/viewModels/section.viewModel';
import { StudentCertificateViewModel } from './model/studentCertificate.ViewModel';
import { StudentCertificateDto } from './model/studentCertificate.dto';
import { CertificateType } from './model/certificateType.enum';

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
    students: StudentCertificateViewModel[] = [];
    student!: StudentCertificateViewModel;
    classRooms: ClassRoomViewModel[] = []
    sections: SectionViewModel[] = [];

    certificateLog: StudentCertificateDto = {
        certificateType: CertificateType.characterCertificate,
        studentEnrollmentId: '',
        certificateNumber: 0
    };
    serialNumber: number = 0;
    classSectionId: string = "";

    constructor(private _apiService: ApiService) {

    }

    ngOnInit(): void {
        this.getClassRooms();
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
            this.isClassSelected = true;
            const selectedClass = this.classRooms.filter(x => x.id === this.classId);
            const sections = selectedClass.map(x => x.sections);
            this.sections = sections[0];
        }

    }

    onClassSectionChange(event: any) {
        const classsSectionId = event.value;
        if (classsSectionId) {
            this.classSectionId = classsSectionId;
            this.isClassSelected = true;
            const selectedClassSection = this.sections.filter(x => x.classSectionId === classsSectionId);
            if (selectedClassSection) {
                this.getStudentByClassSection(classsSectionId);

            }
        }
    }

    getStudentByClassSection(classSectionId: string) {
        this._apiService.getStudentCertificateDataByClassSectionId(classSectionId).subscribe({
            next: (response) => {
                this.students = response;
            },
            error: (err) => console.log(err),
            complete: () => console.log("Request is complete")
        });
    }

    previewCharacterCertificate(student: StudentCertificateViewModel) {
        this.student = student;
        this.getStudentCertificateLog(CertificateType.characterCertificate);
        this.certificateLog.studentEnrollmentId = student.id;
        this.certificateLog.certificateType = CertificateType.characterCertificate;
        this.isCharacterCertificate = true;
    }

    previewTransferCertificate(student: StudentCertificateViewModel) {
        this.student = student;
        this.getStudentCertificateLog(CertificateType.transferCertificate);
        this.certificateLog.studentEnrollmentId = student.id;
        this.certificateLog.certificateType = CertificateType.transferCertificate;
        this.isTransferCertificate = true;
    }

    onDialogClose() {
        this.getStudentByClassSection(this.classSectionId);
        this.isCharacterCertificate = false;
        this.isTransferCertificate = false;
    }

    getStudentCertificateLog(certificateLog: CertificateType) {
        this._apiService.getStudentCertificateLog(certificateLog).subscribe({
            next: (res) => {
                let serialNumber: number = res.certificateNumber;
                serialNumber++;
                this.serialNumber = serialNumber;
                this.certificateLog.certificateNumber = this.serialNumber;
                this.showResult = true;
                this.addStudentCertificateLog(this.certificateLog);
            },
            error: (err) => {

            }
        });
    }

    addStudentCertificateLog(certificateLog: StudentCertificateDto) {
        this._apiService.addStudentCertificateLog(certificateLog).subscribe({
            next: (res) => {

            },
            error: (err) => {

            }
        });
    }
}
