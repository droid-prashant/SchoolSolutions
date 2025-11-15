export interface SubjectMarkDto {
  studentId: string;              
  examType: string;               
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