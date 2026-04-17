export interface ClassCreditCourseViewModel {
    classCreditCourseId: string;
    classRoomId: string;
    courseId: string;
    courseName: string;
    className: string;
    isTheoryRequired: boolean;
    isPracticalRequired: boolean;
    theoryFullMarks: number | null;
    practicalFullMarks: number | null;
    theoryCreditHour: number | null;
    practicalCreditHour: number | null;
}
