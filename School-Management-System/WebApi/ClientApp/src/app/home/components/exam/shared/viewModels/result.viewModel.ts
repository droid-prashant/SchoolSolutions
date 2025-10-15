export interface ResultViewModel {
  examType: string;
  totalCredit: number;
  gpa: number;
  studentName: string;
  fatherName: string;
  motherName: string;
  dateOfBirth: string;
  wardNo: number;
  classRoom: string;
  section: string;
  studentMarks: StudentMarksViewModel[];
}

export interface StudentMarksViewModel {
  courseName: string;
  creditHour: number;
  gradeTheory: string;
  gradePractical: string;
  finalGrade: string;
  finalGradePoint: number;
}
