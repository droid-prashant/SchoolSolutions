export interface SubjectMarkDto {
  studentId: string;              
  examType: string;               
  attendance: number;               
  totalSchoolDays: number;               
  studentMarksLists: StudentMarksList[];
}

export interface StudentMarksList {
  classCourseId: string;              
  isTheoryRequired: boolean;
  isPracticalRequired: boolean;
  theoryCredit: number | null;
  practicalCredit: number | null;
  theoryFullMarks: number | null;
  practicalFullMarks: number | null;
  obtainedTheoryMarks: number | null;
  obtainedPracticalMarks: number | null;
}
