namespace Domain.Constants
{
    public static class PermissionNames
    {
        public const string StudentView = "Student.View";
        public const string StudentCreate = "Student.Create";
        public const string StudentUpdate = "Student.Update";
        public const string StudentDelete = "Student.Delete";
        public const string ExamView = "Exam.View";
        public const string ExamMarksEntry = "Exam.MarksEntry";
        public const string SubjectMarksEntry = "SubjectMarks.Entry";
        public const string ResultView = "Result.View";
        public const string ResultPrint = "Result.Print";
        public const string FeeView = "Fee.View";
        public const string FeeCreate = "Fee.Create";
        public const string AcademicYearManage = "AcademicYear.Manage";
        public const string UserManage = "User.Manage";
        public const string RoleManage = "Role.Manage";
        public const string TeacherView = "Teacher.View";
        public const string TeacherManage = "Teacher.Manage";
        public const string CourseManage = "Course.Manage";
        public const string ClassManage = "Class.Manage";
        public const string StudentAttendanceView = "StudentAttendance.View";
        public const string StudentAttendanceTake = "StudentAttendance.Take";
        public const string StudentAttendanceEdit = "StudentAttendance.Edit";
        public const string StudentAttendanceDelete = "StudentAttendance.Delete";
        public const string StudentAttendanceReport = "StudentAttendance.Report";
        public const string TeacherAttendanceView = "TeacherAttendance.View";
        public const string TeacherAttendanceTake = "TeacherAttendance.Take";
        public const string TeacherAttendanceEdit = "TeacherAttendance.Edit";
        public const string TeacherAttendanceDelete = "TeacherAttendance.Delete";
        public const string TeacherAttendanceReport = "TeacherAttendance.Report";
        public const string TeacherAttendanceCheckInOut = "TeacherAttendance.CheckInOut";

        public static readonly IReadOnlyList<PermissionDefinition> All =
        [
            new(StudentView, "View Students", "Student", "Can view students and student-related read screens."),
            new(StudentCreate, "Create Students", "Student", "Can create new student records."),
            new(StudentUpdate, "Update Students", "Student", "Can update students, enrollment data, roll numbers, and promotions."),
            new(StudentDelete, "Delete Students", "Student", "Can delete student records."),
            new(ExamView, "View Exams", "Exam", "Can view exam setup and exam-related screens."),
            new(ExamMarksEntry, "Marks Entry", "Exam", "Can enter, edit, and delete exam marks."),
            new(SubjectMarksEntry, "Subject Marks Entry", "Exam", "Can access subject marks entry, search teachers, view outputs, and perform available operations."),
            new(ResultView, "View Results", "Result", "Can view student results."),
            new(ResultPrint, "Print Results", "Result", "Can print result documents."),
            new(FeeView, "View Fees", "Fee", "Can view fee summaries and reports."),
            new(FeeCreate, "Collect/Manage Fees", "Fee", "Can create fee types, structures, payments, and adjustments."),
            new(AcademicYearManage, "Manage Academic Years", "Academic", "Can create and update academic years."),
            new(UserManage, "Manage Users", "Security", "Can create users and assign roles."),
            new(RoleManage, "Manage Roles", "Security", "Can create roles and assign permissions."),
            new(TeacherView, "View Teachers", "Teacher", "Can view teacher records."),
            new(TeacherManage, "Manage Teachers", "Teacher", "Can create, update, delete, and assign teachers."),
            new(CourseManage, "Manage Courses", "Academic", "Can manage courses and class course mappings."),
            new(ClassManage, "Manage Classes", "Academic", "Can manage classes, sections, and class-section mappings."),
            new(StudentAttendanceView, "View Student Attendance", "Student Attendance", "Can view student daily attendance entries."),
            new(StudentAttendanceTake, "Take Student Attendance", "Student Attendance", "Can take student attendance in batch."),
            new(StudentAttendanceEdit, "Edit Student Attendance", "Student Attendance", "Can edit existing student attendance entries."),
            new(StudentAttendanceDelete, "Delete Student Attendance", "Student Attendance", "Can delete student attendance entries."),
            new(StudentAttendanceReport, "Student Attendance Reports", "Student Attendance", "Can view student attendance reports and summaries."),
            new(TeacherAttendanceView, "View Teacher Attendance", "Teacher Attendance", "Can view teacher daily attendance entries."),
            new(TeacherAttendanceTake, "Take Teacher Attendance", "Teacher Attendance", "Can take teacher attendance in batch."),
            new(TeacherAttendanceEdit, "Edit Teacher Attendance", "Teacher Attendance", "Can edit existing teacher attendance entries."),
            new(TeacherAttendanceDelete, "Delete Teacher Attendance", "Teacher Attendance", "Can delete teacher attendance entries."),
            new(TeacherAttendanceReport, "Teacher Attendance Reports", "Teacher Attendance", "Can view teacher attendance reports and summaries."),
            new(TeacherAttendanceCheckInOut, "Teacher Check-In/Out", "Teacher Attendance", "Can record own teacher check-in and check-out.")
        ];
    }

    public sealed record PermissionDefinition(string Code, string Name, string GroupName, string Description);
}
