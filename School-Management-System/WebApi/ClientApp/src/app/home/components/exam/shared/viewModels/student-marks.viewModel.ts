export interface SubjectMarksViewModel {
  studentId: string;
  examType: number;
  attendance: number;
  totalSchoolDays: number;
  studentMarksLists: StudentMarksList[];
}

export interface StudentMarksList {
  classCourseId: string;
  theoryCredit: number;
  practicalCredit: number;
  theoryFullMarks: number;
  practicalFullMarks: number;
  obtainedTheoryMarks: number;
  obtainedPracticalMarks: number;
}