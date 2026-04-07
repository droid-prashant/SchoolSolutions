import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ClassRoomViewModel } from '../../home/components/class-room/shared/models/viewModels/classRoom.viewModel';
import { SectionViewModel } from '../../home/components/class-room/shared/models/viewModels/section.viewModel';
import { ApiService } from '../api.service';
import { DistrictViewModel } from './models/master/district.ViewModel';
import { ProvinceViewModel } from './models/master/master.ViewModel';
import { MunicipalityViewModel } from './models/master/municipality.ViewModel';
import { AcademicViewModel } from '../../home/components/master-entry/model/viewModels/academicYear.ViewModel';

@Injectable({
  providedIn: 'root'
})
export class LookupService {
  private readonly CACHE_TTL_MS = 1000 * 60 * 60 * 24; // 24 hours

  private provincesCache: ProvinceViewModel[] | null = null;
  private academicYearsCache: AcademicViewModel[] | null = null;
  private classRoomsCache: ClassRoomViewModel[] | null = null;

  constructor(private apiService: ApiService) {}

  getProvinces(forceRefresh = false): Observable<ProvinceViewModel[]> {
    if (!forceRefresh) {
      if (this.provincesCache) {
        return of(this.provincesCache);
      }

      const stored = this.getFromStorage<ProvinceViewModel[]>('lookup_provinces');
      if (stored) {
        this.provincesCache = stored;
        return of(stored);
      }
    }

    return this.apiService.getProvinceDetails().pipe(
      tap((response: ProvinceViewModel[]) => {
        this.provincesCache = response ?? [];
        this.saveToStorage('lookup_provinces', this.provincesCache);
      })
    );
  }

  getDistrictsByProvince(
    provinceId: string | number,
    forceRefresh = false
  ): Observable<DistrictViewModel[]> {
    return new Observable<DistrictViewModel[]>((observer) => {
      this.getProvinces(forceRefresh).subscribe({
        next: (provinces) => {
          const selectedProvince = provinces.find(x => String(x.id) === String(provinceId));
          observer.next(selectedProvince?.districts ?? []);
          observer.complete();
        },
        error: (err) => observer.error(err)
      });
    });
  }

  getMunicipalitiesByDistrict(
    provinceId: string | number,
    districtId: string | number,
    forceRefresh = false
  ): Observable<MunicipalityViewModel[]> {
    return new Observable<MunicipalityViewModel[]>((observer) => {
      this.getDistrictsByProvince(provinceId, forceRefresh).subscribe({
        next: (districts) => {
          const selectedDistrict = districts.find(x => String(x.id) === String(districtId));
          observer.next(selectedDistrict?.municipalities ?? []);
          observer.complete();
        },
        error: (err) => observer.error(err)
      });
    });
  }

  getAcademicYears(forceRefresh = false): Observable<AcademicViewModel[]> {
    if (!forceRefresh) {
      if (this.academicYearsCache) {
        return of(this.academicYearsCache);
      }

      const stored = this.getFromStorage<AcademicViewModel[]>('lookup_academic_years');
      if (stored) {
        this.academicYearsCache = stored;
        return of(stored);
      }
    }

    return this.apiService.getAcademicYear().pipe(
      tap((response: AcademicViewModel[]) => {
        this.academicYearsCache = response ?? [];
        this.saveToStorage('lookup_academic_years', this.academicYearsCache);
      })
    );
  }

  getActiveAcademicYear(forceRefresh = false): Observable<AcademicViewModel | null> {
    return new Observable<AcademicViewModel | null>((observer) => {
      this.getAcademicYears(forceRefresh).subscribe({
        next: (years) => {
          const activeYear = years.find(x => x.isActive) ?? null;
          observer.next(activeYear);
          observer.complete();
        },
        error: (err) => observer.error(err)
      });
    });
  }

  getClassRooms(forceRefresh = false): Observable<ClassRoomViewModel[]> {
    if (!forceRefresh) {
      if (this.classRoomsCache) {
        return of(this.classRoomsCache);
      }

      const stored = this.getFromStorage<ClassRoomViewModel[]>('lookup_class_rooms');
      if (stored) {
        this.classRoomsCache = stored;
        return of(stored);
      }
    }

    return this.apiService.getClassRooms().pipe(
      tap((response: ClassRoomViewModel[]) => {
        this.classRoomsCache = response ?? [];
        this.saveToStorage('lookup_class_rooms', this.classRoomsCache);
      })
    );
  }

  getSectionsByClassRoom(
    classRoomId: string,
    forceRefresh = false
  ): Observable<SectionViewModel[]> {
    return new Observable<SectionViewModel[]>((observer) => {
      this.getClassRooms(forceRefresh).subscribe({
        next: (classRooms) => {
          const selectedClass = classRooms.find(x => String(x.id) === String(classRoomId));
          observer.next(selectedClass?.sections ?? []);
          observer.complete();
        },
        error: (err) => observer.error(err)
      });
    });
  }

  clearAllCache(): void {
    this.provincesCache = null;
    this.academicYearsCache = null;
    this.classRoomsCache = null;

    localStorage.removeItem('lookup_provinces');
    localStorage.removeItem('lookup_academic_years');
    localStorage.removeItem('lookup_class_rooms');
  }

  clearCacheByKey(key: 'provinces' | 'academicYears' | 'classRooms'): void {
    switch (key) {
      case 'provinces':
        this.provincesCache = null;
        localStorage.removeItem('lookup_provinces');
        break;
      case 'academicYears':
        this.academicYearsCache = null;
        localStorage.removeItem('lookup_academic_years');
        break;
      case 'classRooms':
        this.classRoomsCache = null;
        localStorage.removeItem('lookup_class_rooms');
        break;
    }
  }

  private saveToStorage(key: string, data: unknown): void {
    const payload = {
      savedAt: Date.now(),
      data
    };

    localStorage.setItem(key, JSON.stringify(payload));
  }

  private getFromStorage<T>(key: string): T | null {
    const raw = localStorage.getItem(key);
    if (!raw) {
      return null;
    }

    try {
      const parsed = JSON.parse(raw) as { savedAt: number; data: T };
      const isExpired = Date.now() - parsed.savedAt > this.CACHE_TTL_MS;

      if (isExpired) {
        localStorage.removeItem(key);
        return null;
      }

      return parsed.data;
    } catch {
      localStorage.removeItem(key);
      return null;
    }
  }
}