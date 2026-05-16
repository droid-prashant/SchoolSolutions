import { Injectable, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { MessageService } from 'primeng/api';
import * as signalR from '@microsoft/signalr';
import { initializeApp, FirebaseApp } from 'firebase/app';
import { getMessaging, getToken, isSupported, Messaging } from 'firebase/messaging';
import { environment } from '../../environments/environment';
import { AppNotification, NoticeLetter, NotificationDispatch, RegisterNotificationToken } from './notification.models';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private baseUrl = environment.API_BASE_URL;
  private notificationsSubject = new BehaviorSubject<AppNotification[]>([]);
  private unreadCountSubject = new BehaviorSubject<number>(0);
  private realtimeNotificationSubject = new Subject<AppNotification>();
  private firebaseApp?: FirebaseApp;
  private messaging?: Messaging;
  private hubConnection?: signalR.HubConnection;
  private isPushRegistrationRunning = false;

  notifications$ = this.notificationsSubject.asObservable();
  unreadCount$ = this.unreadCountSubject.asObservable();
  realtimeNotification$ = this.realtimeNotificationSubject.asObservable();

  constructor(
    private http: HttpClient,
    private messageService: MessageService,
    private zone: NgZone
  ) { }

  loadNotifications(): void {
    this.getMyNotifications().subscribe({
      next: notifications => {
        this.notificationsSubject.next(notifications);
        this.unreadCountSubject.next(notifications.filter(x => !x.isRead).length);
      }
    });
  }

  loadUnreadCount(): void {
    this.getUnreadCount().subscribe({
      next: count => this.unreadCountSubject.next(count)
    });
  }

  getMyNotifications(): Observable<AppNotification[]> {
    return this.http.get<AppNotification[]>(this.baseUrl + 'api/notifications/my');
  }

  getUnreadCount(): Observable<number> {
    return this.http.get<number>(this.baseUrl + 'api/notifications/unread-count');
  }

  getNoticeLetter(noticeId: string): Observable<NoticeLetter> {
    return this.http.get<NoticeLetter>(this.baseUrl + `api/notifications/notices/${noticeId}/letter`);
  }

  markAsRead(notification: AppNotification): void {
    if (!notification || notification.isRead) {
      return;
    }

    this.http.post<void>(this.baseUrl + `api/notifications/${notification.id}/mark-as-read`, {}).subscribe({
      next: () => {
        const updated = this.notificationsSubject.value.map(item =>
          item.id === notification.id ? { ...item, isRead: true, readAt: new Date().toISOString() } : item
        );
        this.notificationsSubject.next(updated);
        this.unreadCountSubject.next(Math.max(0, this.unreadCountSubject.value - 1));
      }
    });
  }

  markAllAsRead(): void {
    this.http.post<void>(this.baseUrl + 'api/notifications/mark-all-as-read', {}).subscribe({
      next: () => {
        const readAt = new Date().toISOString();
        this.notificationsSubject.next(this.notificationsSubject.value.map(item => ({ ...item, isRead: true, readAt })));
        this.unreadCountSubject.next(0);
      }
    });
  }

  async initializeGuardianNotifications(): Promise<void> {
    this.loadNotifications();
    await this.registerFirebaseToken();
    await this.connectSignalR();
  }

  async connectSignalR(): Promise<void> {
    const token = window.localStorage.getItem('token');
    if (!token || this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    const hubUrl = this.baseUrl.replace(/\/$/, '') + '/hubs/notifications';
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveNotification', (notification: NotificationDispatch) => {
      this.zone.run(() => this.handleRealtimeNotification(notification));
    });

    await this.hubConnection.start();
  }

  async disconnectSignalR(): Promise<void> {
    if (!this.hubConnection) {
      return;
    }

    await this.hubConnection.stop();
    this.hubConnection = undefined;
  }

  private async registerFirebaseToken(): Promise<void> {
    if (this.isPushRegistrationRunning || !this.hasFirebaseConfig() || !('Notification' in window)) {
      return;
    }

    this.isPushRegistrationRunning = true;
    try {
      const supported = await isSupported();
      if (!supported) {
        return;
      }

      const permission = await Notification.requestPermission();
      if (permission !== 'granted') {
        return;
      }

      const registration = await navigator.serviceWorker.register('/firebase-messaging-sw.js');
      this.firebaseApp = this.firebaseApp ?? initializeApp(environment.firebase);
      this.messaging = this.messaging ?? getMessaging(this.firebaseApp);
      const fcmToken = await getToken(this.messaging, {
        vapidKey: environment.firebase.vapidKey,
        serviceWorkerRegistration: registration
      });

      if (!fcmToken) {
        return;
      }

      const payload: RegisterNotificationToken = {
        fcmToken,
        deviceType: this.getDeviceType(),
        browser: this.getBrowserName(),
        platform: navigator.platform
      };

      this.http.post<void>(this.baseUrl + 'api/notifications/register-token', payload).subscribe();
    } finally {
      this.isPushRegistrationRunning = false;
    }
  }

  private handleRealtimeNotification(notification: NotificationDispatch): void {
    const viewModel: AppNotification = {
      id: notification.userNotificationId,
      notificationId: notification.notificationId,
      title: notification.title,
      message: notification.message,
      notificationType: notification.notificationType,
      notificationDateTimeNp: notification.notificationDateTimeNp || this.formatFallbackNepaliDateTime(new Date()),
      referenceId: notification.referenceId,
      studentId: notification.studentId,
      noticeId: notification.notificationType === 'Notice' ? notification.referenceId : null,
      isRead: false,
      createdAt: new Date().toISOString(),
      deliveryStatus: 'Sent'
    };

    const withoutDuplicate = this.notificationsSubject.value.filter(x => x.id !== viewModel.id);
    this.notificationsSubject.next([viewModel, ...withoutDuplicate]);
    this.unreadCountSubject.next(this.unreadCountSubject.value + 1);
    this.realtimeNotificationSubject.next(viewModel);
    this.messageService.add({
      severity: 'info',
      summary: viewModel.title,
      detail: `${viewModel.message} (${viewModel.notificationDateTimeNp})`
    });
  }

  private hasFirebaseConfig(): boolean {
    const config = environment.firebase;
    return !!config?.apiKey && !!config?.projectId && !!config?.messagingSenderId && !!config?.appId && !!config?.vapidKey;
  }

  private getDeviceType(): string {
    return /Mobi|Android/i.test(navigator.userAgent) ? 'Mobile' : 'Desktop';
  }

  private getBrowserName(): string {
    const userAgent = navigator.userAgent;
    if (userAgent.includes('Edg/')) {
      return 'Edge';
    }
    if (userAgent.includes('Chrome/')) {
      return 'Chrome';
    }
    if (userAgent.includes('Firefox/')) {
      return 'Firefox';
    }
    if (userAgent.includes('Safari/')) {
      return 'Safari';
    }
    return 'Browser';
  }

  private formatFallbackNepaliDateTime(date: Date): string {
    return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`;
  }
}
