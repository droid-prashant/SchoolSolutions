export interface SubjectMarkDto {
  studentId: string;              
  examType: string;               
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