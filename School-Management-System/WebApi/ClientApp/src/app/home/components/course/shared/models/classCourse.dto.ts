export interface ClassCourseDto {
    classCourseId:string;
    classRoomId: string;
    courseId: string;
    isTheoryRequired: boolean;
    isPracticalRequired: boolean;
    theoryCreditHour: number | null;
    practicalCreditHour: number | null;
    theoryFullMarks: number | null;
    practicalFullMarks: number | null;
}
