import { StudentMarksList } from '../../../../exam/shared/viewModels/student-marks.viewModel';

export interface TeacherMarksAssignmentViewModel {
  assignmentId: string;
  teacherId: string;
  teacherName: string;
  classSectionId: string;
  classRoomId: string;
  classRoomName: string;
  sectionId: string;
  sectionName: string;
  courseId: string;
  classCourseId: string;
  courseName: string;
  isTheoryRequired: boolean;
  isPracticalRequired: boolean;
  theoryFullMarks: number | null;
  practicalFullMarks: number | null;
  theoryCreditHour: number | null;
  practicalCreditHour: number | null;
}

export interface TeacherSubjectStudentMarksViewModel {
  studentEnrollmentId: string;
  studentName: string;
  rollNumber: number | null;
  attendance: number | null;
  totalSchoolDays: number | null;
  hasMarksEntry: boolean;
  marks: StudentMarksList | null;
}
