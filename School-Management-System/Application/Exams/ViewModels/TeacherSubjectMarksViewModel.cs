using Application.Exams.Dtos;

namespace Application.Exams.ViewModels
{
    public class TeacherMarksAssignmentViewModel
    {
        public Guid AssignmentId { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public Guid ClassSectionId { get; set; }
        public Guid ClassRoomId { get; set; }
        public string ClassRoomName { get; set; } = string.Empty;
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public Guid ClassCourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public bool IsTheoryRequired { get; set; }
        public bool IsPracticalRequired { get; set; }
        public decimal? TheoryFullMarks { get; set; }
        public decimal? PracticalFullMarks { get; set; }
        public decimal? TheoryCreditHour { get; set; }
        public decimal? PracticalCreditHour { get; set; }
    }

    public class TeacherSubjectStudentMarksViewModel
    {
        public Guid StudentEnrollmentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int? RollNumber { get; set; }
        public int? Attendance { get; set; }
        public int? TotalSchoolDays { get; set; }
        public bool HasMarksEntry { get; set; }
        public StudentMarksList? Marks { get; set; }
    }
}
