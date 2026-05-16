export interface NoticeDto {
  title: string;
  description: string;
  noticeDate: string | Date;
  noticeDateNp: string;
  targetAudience: string;
  classId?: string | null;
  sectionId?: string | null;
  studentIds?: string[];
}

export interface NoticeViewModel {
  id: string;
  title: string;
  description: string;
  noticeDate: string;
  noticeDateNp: string;
  targetAudience: string;
  classId?: string | null;
  className?: string | null;
  sectionId?: string | null;
  sectionName?: string | null;
  studentIds?: string[];
  studentNames?: string[];
  isPublished: boolean;
  publishedAt?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface NoticeAudienceOption {
  label: string;
  value: string;
}
