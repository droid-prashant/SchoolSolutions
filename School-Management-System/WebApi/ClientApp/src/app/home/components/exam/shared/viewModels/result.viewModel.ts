export interface ResultViewModel {
  examType: string;
  totalCredit: number;
  gpa: number;
  studentName: string;
  fatherName: string;
  motherName: string;
  dateOfBirth: string;
  rollNo: number;
  wardNo: number;
  attendance: number;
  totalSchoolDays: number;
  classRoom: string;
  section: string;
  issueDate: Date;
  studentMarks: StudentMarksViewModel[];
}

export interface StudentMarksViewModel {
  courseName: string;
  theoryCreditHour: number | null;
  practicalCreditHour: number | null;
  gradeTheory: string;
  gradePractical: string;
  finalGrade: string;
  finalGradePoint: number;
}
