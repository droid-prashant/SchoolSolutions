import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { NoticeDto, NoticeViewModel } from './notice.models';

@Injectable({
  providedIn: 'root'
})
export class NoticeApiService {
  private baseUrl = environment.API_BASE_URL;

  constructor(private http: HttpClient) { }

  getNotices(): Observable<NoticeViewModel[]> {
    return this.http.get<NoticeViewModel[]>(this.baseUrl + 'api/notices');
  }

  getNotice(id: string): Observable<NoticeViewModel> {
    return this.http.get<NoticeViewModel>(this.baseUrl + `api/notices/${id}`);
  }

  createNotice(notice: NoticeDto): Observable<NoticeViewModel> {
    return this.http.post<NoticeViewModel>(this.baseUrl + 'api/notices', notice);
  }

  updateNotice(id: string, notice: NoticeDto): Observable<NoticeViewModel> {
    return this.http.put<NoticeViewModel>(this.baseUrl + `api/notices/${id}`, notice);
  }

  publishNotice(id: string): Observable<void> {
    return this.http.post<void>(this.baseUrl + `api/notices/${id}/publish`, {});
  }
}
