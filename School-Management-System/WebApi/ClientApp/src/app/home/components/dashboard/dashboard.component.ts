import { Component, OnInit } from '@angular/core';
import { SharedModule } from '../../../shared/shared.module';
import { ApiService } from '../../../shared/api.service';
import { StudentsByClassViewModel } from './model/studentsByClass.viewModel';
import { ProvinceViewModel } from '../../../shared/common/models/master/master.ViewModel';
import { MasterApiService } from '../../../shared/master-api.service';
import { ClassRoomViewModel } from '../class-room/shared/models/viewModels/classRoom.viewModel';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  data: any;
  options: any;
  studentsCount: number = 0;
  courseCount: number = 0;
  teachersCount: number = 0;
  feePendingOverall: number = 0;
  feeCollectedToday: number = 0;
  feeCollectedOverall: number = 0;
  pendingFeeStudents: number = 0;
  studentsByClass: StudentsByClassViewModel[] = []
  provinceDetails: ProvinceViewModel[] = [];
  classRooms: ClassRoomViewModel[] = []

  constructor(private _apiService: ApiService, private masterApiService: MasterApiService) { }

  ngOnInit(): void {
    this.loadDashboardSummary();
    this.classVsStudents();
    this.getProvinceDetails();
    this.getClassRooms();
  }

  loadDashboardSummary() {
    this._apiService.getDashboardSummary().subscribe({
      next: (res) => {
        this.studentsCount = res.studentsCount;
        this.courseCount = res.coursesCount;
        this.teachersCount = res.teachersCount;
        this.feePendingOverall = res.feePendingOverall;
        this.feeCollectedToday = res.feeCollectedToday;
        this.feeCollectedOverall = res.feeCollectedOverall;
        this.pendingFeeStudents = res.pendingFeeStudents;
      },
      error: () => {
        this.studentCount();
        this.coursesCount();
        this.teachersCountLoad();
      }
    });
  }

  getProvinceDetails() {
    this._apiService.getProvinceDetails().subscribe(
      {
        next: (response) => {
          this.provinceDetails = response;
          localStorage.setItem('provinceDetails', JSON.stringify(response));
        },
        error: (err) => console.log(err),
        complete: () => console.log("Request is complete")
      }
    )
  }

  studentCount() {
    this._apiService.getStudentsCount().subscribe({
      next: (res) => {
        this.studentsCount = res;
      },
      error: (err) => {

      }
    });
  }

  coursesCount() {
    this._apiService.getCoursesCount().subscribe({
      next: (res) => {
        this.courseCount = res;
      },
      error: (err) => {

      }
    });
  }

  teachersCountLoad() {
    this._apiService.getTeachersCount().subscribe({
      next: (res) => {
        this.teachersCount = res;
      },
      error: (err) => {

      }
    });
  }

  classVsStudents() {
    this._apiService.getStudentsByClassCount().subscribe({
      next: (res) => {
        this.studentsByClass = res;
        this.initializeBarChart();

      },
      error: (err) => {

      }
    });
  }

  getClassRooms() {
    this.masterApiService.getClassRooms().subscribe({
      next: (res) => {
        this.classRooms = res;
        localStorage.setItem('classRoomDetails', JSON.stringify(res));
      },
      error: (err) => {
      }
    });
  }



  initializeBarChart() {
    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--text-color');
    const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
    const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

    const colors = [
      'rgba(75, 192, 192, 0.7)',
      'rgba(255, 159, 64, 0.7)',
      'rgba(153, 102, 255, 0.7)',
      'rgba(255, 205, 86, 0.7)',
      'rgba(54, 162, 235, 0.7)'
    ];
    const labels = this.studentsByClass.map(x => x.classRoom);
    const allSections = Array.from(
      new Set(this.studentsByClass.flatMap(c => c.studentsCountBySections ?? []).map(s => s.sectionName)));

    const dataSets = allSections.map((section, i) => ({
      type: 'bar',
      label: `Section ${section}`,
      backgroundColor: colors[i % colors.length],
      data: this.studentsByClass.map(c => {
        const match = c.studentsCountBySections.find(s => s.sectionName === section);
        return match ? match.studentCount : 0;
      })
    }))

    this.data = {
      labels: labels,
      datasets: dataSets
    };

    this.options = {
      maintainAspectRatio: false,
      aspectRatio: 0.8,
      plugins: {
        tooltip: {
          mode: 'index',
          intersect: false
        },
        legend: {
          labels: {
            color: textColor
          }
        }
      },
      scales: {
        x: {
          stacked: true,
          ticks: {
            color: textColorSecondary
          },
          grid: {
            color: surfaceBorder,
            drawBorder: false
          }
        },
        y: {
          stacked: true,
          ticks: {
            color: textColorSecondary
          },
          grid: {
            color: surfaceBorder,
            drawBorder: false
          }
        }
      }
    };
  }
}
