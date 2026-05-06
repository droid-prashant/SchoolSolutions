using Application.Attendance.Dtos;
using Application.Attendance.Interfaces;
using Application.Attendance.ViewModels;
using Application.Common.Interfaces;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Attendance
{
    public class StudentAttendanceService : IStudentAttendanceService
    {
        private readonly IApplicationDbContext _context;
        private readonly UserResolver _userResolver;

        public StudentAttendanceService(IApplicationDbContext context, UserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public async Task<StudentDailyAttendanceViewModel> UpsertDailyAttendanceAsync(StudentAttendanceBatchDto request, CancellationToken cancellationToken)
        {
            ValidateBatchRequest(request);
            var academicYearId = ParseGuid(request.AcademicYearId, "Academic year is required.");
            var classSectionId = ParseGuid(request.ClassSectionId, "Class section is required.");
            var recordedBy = GetCurrentUserId();

            var entryEnrollmentIds = request.Entries
                .Select(x => ParseGuid(x.StudentEnrollmentId, "Student enrollment id is required."))
                .ToList();

            if (entryEnrollmentIds.Count != entryEnrollmentIds.Distinct().Count())
            {
                throw new Exception("Duplicate student attendance entries are not allowed.");
            }

            var existingAttendance = await _context.StudentAttendances
                .Where(x => entryEnrollmentIds.Contains(x.StudentEnrollmentId) &&
                            x.AcademicYearId == academicYearId &&
                            x.AttendanceDateEn == request.AttendanceDateEn &&
                            !x.IsDeleted)
                .ToListAsync(cancellationToken);

            var existingEnrollmentIds = existingAttendance.Select(x => x.StudentEnrollmentId).ToHashSet();

            var enrollments = await _context.StudentEnrollments
                .Include(x => x.Student)
                .Where(x => entryEnrollmentIds.Contains(x.Id) &&
                            x.AcademicYearId == academicYearId &&
                            x.ClassSectionId == classSectionId &&
                            !x.IsDeleted &&
                            !x.Student.IsDeleted &&
                            (x.IsActive || existingEnrollmentIds.Contains(x.Id)))
                .ToListAsync(cancellationToken);

            if (enrollments.Count != entryEnrollmentIds.Count)
            {
                throw new Exception("One or more selected students are not valid for the selected academic year, class, section, and date.");
            }

            ValidateStatuses(request.Entries);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var entry in request.Entries)
                {
                    var enrollmentId = Guid.Parse(entry.StudentEnrollmentId);
                    var enrollment = enrollments.First(x => x.Id == enrollmentId);
                    var attendance = existingAttendance.FirstOrDefault(x => x.StudentEnrollmentId == enrollmentId);

                    if (attendance == null)
                    {
                        attendance = new StudentAttendance
                        {
                            Id = Guid.NewGuid(),
                            StudentEnrollmentId = enrollment.Id,
                            StudentId = enrollment.StudentId,
                            ClassSectionId = classSectionId,
                            AcademicYearId = academicYearId,
                            AttendanceDateEn = request.AttendanceDateEn,
                            AttendanceDateNp = request.AttendanceDateNp.Trim(),
                            CreatedBy = recordedBy,
                            RecordedByUserId = recordedBy,
                            IsActive = true
                        };
                        await _context.StudentAttendances.AddAsync(attendance, cancellationToken);
                    }

                    attendance.Status = entry.Status;
                    attendance.Remarks = entry.Remarks?.Trim() ?? string.Empty;
                    attendance.AttendanceDateNp = request.AttendanceDateNp.Trim();
                    attendance.RecordedByUserId = recordedBy;
                    attendance.ModifiedBy = recordedBy;
                    attendance.IsActive = true;
                    attendance.IsDeleted = false;
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

            return await GetAttendanceByDateAsync(request.AcademicYearId, request.ClassSectionId, request.AttendanceDateEn, cancellationToken);
        }

        public async Task<StudentDailyAttendanceViewModel> GetAttendanceByDateAsync(string academicYearId, string classSectionId, DateOnly attendanceDateEn, CancellationToken cancellationToken)
        {
            var academicYearGuid = ParseGuid(academicYearId, "Academic year is required.");
            var classSectionGuid = ParseGuid(classSectionId, "Class section is required.");

            var classSection = await _context.ClassSections
                .Include(x => x.ClassRoom)
                .Include(x => x.Section)
                .FirstOrDefaultAsync(x => x.Id == classSectionGuid, cancellationToken);

            if (classSection == null)
            {
                throw new Exception("Selected class section not found.");
            }

            var attendanceRows = await _context.StudentAttendances
                .Where(x => x.AcademicYearId == academicYearGuid &&
                            x.ClassSectionId == classSectionGuid &&
                            x.AttendanceDateEn == attendanceDateEn &&
                            !x.IsDeleted)
                .ToListAsync(cancellationToken);

            var attendedEnrollmentIds = attendanceRows.Select(x => x.StudentEnrollmentId).ToHashSet();

            var enrollments = await _context.StudentEnrollments
                .Include(x => x.Student)
                .Where(x => x.AcademicYearId == academicYearGuid &&
                            x.ClassSectionId == classSectionGuid &&
                            !x.IsDeleted &&
                            !x.Student.IsDeleted &&
                            (x.IsActive || attendedEnrollmentIds.Contains(x.Id)))
                .OrderBy(x => x.RollNumber ?? int.MaxValue)
                .ThenBy(x => x.Student.FirstName)
                .ThenBy(x => x.Student.LastName)
                .ToListAsync(cancellationToken);

            var rows = enrollments.Select(enrollment =>
            {
                var attendance = attendanceRows.FirstOrDefault(x => x.StudentEnrollmentId == enrollment.Id);
                var status = attendance?.Status ?? StudentAttendanceStatus.Present;
                return new StudentAttendanceRowViewModel
                {
                    AttendanceId = attendance?.Id,
                    StudentEnrollmentId = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    StudentName = BuildStudentName(enrollment.Student),
                    ClassRoomName = classSection.ClassRoom.Name,
                    SectionName = classSection.Section.Name,
                    RollNumber = enrollment.RollNumber,
                    Status = status,
                    StatusName = status.ToString(),
                    Remarks = attendance?.Remarks ?? string.Empty,
                    HasAttendance = attendance != null,
                    IsEnrollmentActive = enrollment.IsActive
                };
            }).ToList();

            return new StudentDailyAttendanceViewModel
            {
                AcademicYearId = academicYearGuid,
                ClassSectionId = classSectionGuid,
                AttendanceDateEn = attendanceDateEn,
                AttendanceDateNp = attendanceRows.Select(x => x.AttendanceDateNp).FirstOrDefault() ?? string.Empty,
                Students = rows,
                Summary = BuildStudentSummary(rows.Where(x => x.HasAttendance).Select(x => x.Status).ToList())
            };
        }

        public async Task<StudentAttendanceHistoryViewModel> GetAttendanceByStudentAsync(string studentId, string academicYearId, CancellationToken cancellationToken)
        {
            var studentGuid = ParseGuid(studentId, "Student id is required.");
            var academicYearGuid = ParseGuid(academicYearId, "Academic year is required.");

            var rows = await GetStudentAttendanceReportQuery()
                .Where(x => x.StudentId == studentGuid && x.AcademicYearId == academicYearGuid)
                .OrderBy(x => x.AttendanceDateEn)
                .ToListAsync(cancellationToken);
            FillStatusNames(rows);

            var studentName = rows.FirstOrDefault()?.StudentName
                ?? await _context.Students
                    .Where(x => x.Id == studentGuid)
                    .Select(x => (x.FirstName + " " + x.LastName).Trim())
                    .FirstOrDefaultAsync(cancellationToken)
                ?? string.Empty;

            return new StudentAttendanceHistoryViewModel
            {
                StudentId = studentGuid,
                StudentName = studentName,
                AcademicYearId = academicYearGuid,
                Rows = rows,
                Summary = BuildStudentSummary(rows.Select(x => x.Status).ToList())
            };
        }

        public async Task<MonthlyAttendanceReportViewModel<StudentAttendanceReportRowViewModel>> GetMonthlyAttendanceAsync(string academicYearId, string classSectionId, int year, int month, CancellationToken cancellationToken)
        {
            ValidateMonth(year, month);
            var academicYearGuid = ParseGuid(academicYearId, "Academic year is required.");
            var classSectionGuid = ParseGuid(classSectionId, "Class section is required.");
            var start = new DateOnly(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            var rows = await GetStudentAttendanceReportQuery()
                .Where(x => x.AcademicYearId == academicYearGuid &&
                            x.ClassSectionId == classSectionGuid &&
                            x.AttendanceDateEn >= start &&
                            x.AttendanceDateEn <= end)
                .OrderBy(x => x.AttendanceDateEn)
                .ThenBy(x => x.RollNumber ?? int.MaxValue)
                .ThenBy(x => x.StudentName)
                .ToListAsync(cancellationToken);
            FillStatusNames(rows);

            return new MonthlyAttendanceReportViewModel<StudentAttendanceReportRowViewModel>
            {
                AcademicYearId = academicYearGuid,
                Year = year,
                Month = month,
                Rows = rows,
                Summary = BuildStudentSummary(rows.Select(x => x.Status).ToList())
            };
        }

        public async Task<List<StudentAttendanceReportRowViewModel>> GetAbsentStudentsAsync(string academicYearId, string classSectionId, DateOnly attendanceDateEn, CancellationToken cancellationToken)
        {
            var academicYearGuid = ParseGuid(academicYearId, "Academic year is required.");
            var classSectionGuid = ParseGuid(classSectionId, "Class section is required.");

            var rows = await GetStudentAttendanceReportQuery()
                .Where(x => x.AcademicYearId == academicYearGuid &&
                            x.ClassSectionId == classSectionGuid &&
                            x.AttendanceDateEn == attendanceDateEn &&
                            x.Status == StudentAttendanceStatus.Absent)
                .OrderBy(x => x.RollNumber ?? int.MaxValue)
                .ThenBy(x => x.StudentName)
                .ToListAsync(cancellationToken);
            FillStatusNames(rows);
            return rows;
        }

        public async Task<AttendanceSummaryViewModel> GetAttendanceSummaryAsync(string academicYearId, string classSectionId, DateOnly attendanceDateEn, CancellationToken cancellationToken)
        {
            var academicYearGuid = ParseGuid(academicYearId, "Academic year is required.");
            var classSectionGuid = ParseGuid(classSectionId, "Class section is required.");

            var statuses = await _context.StudentAttendances
                .Where(x => x.AcademicYearId == academicYearGuid &&
                            x.ClassSectionId == classSectionGuid &&
                            x.AttendanceDateEn == attendanceDateEn &&
                            !x.IsDeleted)
                .Select(x => x.Status)
                .ToListAsync(cancellationToken);

            return BuildStudentSummary(statuses);
        }

        public async Task DeleteAttendanceAsync(string attendanceId, CancellationToken cancellationToken)
        {
            var attendanceGuid = ParseGuid(attendanceId, "Attendance id is required.");
            var attendance = await _context.StudentAttendances
                .FirstOrDefaultAsync(x => x.Id == attendanceGuid && !x.IsDeleted, cancellationToken);

            if (attendance == null)
            {
                throw new Exception("Attendance record not found.");
            }

            attendance.IsDeleted = true;
            attendance.IsActive = false;
            attendance.DeletedOn = DateTime.UtcNow;
            attendance.DeletedBy = GetCurrentUserId();
            attendance.ModifiedBy = GetCurrentUserId();
            await _context.SaveChangesAsync(cancellationToken);
        }

        private IQueryable<StudentAttendanceReportRowViewModel> GetStudentAttendanceReportQuery()
        {
            return _context.StudentAttendances
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Select(x => new StudentAttendanceReportRowViewModel
                {
                    AttendanceId = x.Id,
                    AcademicYearId = x.AcademicYearId,
                    ClassSectionId = x.ClassSectionId,
                    StudentEnrollmentId = x.StudentEnrollmentId,
                    StudentId = x.StudentId,
                    StudentName = (x.Student.FirstName + " " + x.Student.LastName).Trim(),
                    ClassRoomName = x.ClassSection.ClassRoom.Name,
                    SectionName = x.ClassSection.Section.Name,
                    RollNumber = x.StudentEnrollment.RollNumber,
                    AttendanceDateEn = x.AttendanceDateEn,
                    AttendanceDateNp = x.AttendanceDateNp,
                    Status = x.Status,
                    StatusName = string.Empty,
                    Remarks = x.Remarks,
                    IsEnrollmentActive = x.StudentEnrollment.IsActive
                });
        }

        private static void ValidateBatchRequest(StudentAttendanceBatchDto request)
        {
            if (request == null)
            {
                throw new Exception("Attendance request is required.");
            }

            if (string.IsNullOrWhiteSpace(request.AttendanceDateNp))
            {
                throw new Exception("Nepali attendance date is required.");
            }

            if (request.Entries == null || request.Entries.Count == 0)
            {
                throw new Exception("At least one student attendance entry is required.");
            }
        }

        private static void FillStatusNames(IEnumerable<StudentAttendanceReportRowViewModel> rows)
        {
            foreach (var row in rows)
            {
                row.StatusName = row.Status.ToString();
            }
        }

        private static void ValidateStatuses(IEnumerable<StudentAttendanceEntryDto> entries)
        {
            foreach (var entry in entries)
            {
                if (!Enum.IsDefined(typeof(StudentAttendanceStatus), entry.Status))
                {
                    throw new Exception("Invalid student attendance status.");
                }
            }
        }

        private static AttendanceSummaryViewModel BuildStudentSummary(List<StudentAttendanceStatus> statuses)
        {
            var total = statuses.Count;
            var present = statuses.Count(x => x == StudentAttendanceStatus.Present);
            var absent = statuses.Count(x => x == StudentAttendanceStatus.Absent);
            var late = statuses.Count(x => x == StudentAttendanceStatus.Late);
            var leave = statuses.Count(x => x == StudentAttendanceStatus.Leave);
            var halfDay = statuses.Count(x => x == StudentAttendanceStatus.HalfDay);
            var weightedPresent = present + late + (halfDay * 0.5m);

            return new AttendanceSummaryViewModel
            {
                Total = total,
                Present = present,
                Absent = absent,
                Late = late,
                Leave = leave,
                HalfDay = halfDay,
                AttendancePercentage = total == 0 ? 0 : Math.Round((weightedPresent / total) * 100, 2),
                StatusCounts = statuses
                    .GroupBy(x => x)
                    .Select(x => new AttendanceStatusCountViewModel
                    {
                        Status = (int)x.Key,
                        StatusName = x.Key.ToString(),
                        Count = x.Count()
                    })
                    .OrderBy(x => x.Status)
                    .ToList()
            };
        }

        private Guid GetCurrentUserId()
        {
            if (!Guid.TryParse(_userResolver.UserId, out var userId))
            {
                throw new Exception("Current user is not available.");
            }

            return userId;
        }

        private static Guid ParseGuid(string value, string message)
        {
            if (!Guid.TryParse(value, out var parsed))
            {
                throw new Exception(message);
            }

            return parsed;
        }

        private static void ValidateMonth(int year, int month)
        {
            if (year < 1900 || year > 2100 || month < 1 || month > 12)
            {
                throw new Exception("Invalid report month.");
            }
        }

        private static string BuildStudentName(Student student)
        {
            return $"{student.FirstName} {student.LastName}".Trim();
        }
    }
}
