export interface AppNotification {
  id: string;
  notificationId: string;
  title: string;
  message: string;
  notificationType: string;
  referenceId?: string | null;
  studentId?: string | null;
  studentName?: string | null;
  noticeId?: string | null;
  isRead: boolean;
  readAt?: string | null;
  createdAt: string;
  notificationDateTimeNp: string;
  deliveryStatus: string;
}

export interface NotificationDispatch {
  userNotificationId: string;
  userId: string;
  notificationId: string;
  title: string;
  message: string;
  notificationType: string;
  notificationDateTimeNp: string;
  referenceId?: string | null;
  studentId?: string | null;
}

export interface RegisterNotificationToken {
  fcmToken: string;
  deviceType?: string;
  browser?: string;
  platform?: string;
}

export interface NoticeLetter {
  noticeId: string;
  schoolName: string;
  schoolAddress: string;
  schoolPhoneNumber: string;
  schoolEmail: string;
  schoolWebsite: string;
  noticeNumber: string;
  noticeDate: string;
  noticeDateNp: string;
  publishedDateTimeNp: string;
  subject: string;
  body: string;
  targetAudience: string;
  issuedBy: string;
}
