using Application.Attendance.Dtos;
using Application.Attendance.Interfaces;
using Application.Attendance.ViewModels;
using Application.Common.Interfaces;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Attendance
{
    public class TeacherAttendanceService : ITeacherAttendanceService
    {
        private readonly IApplicationDbContext _context;
        private readonly UserResolver _userResolver;

        public TeacherAttendanceService(IApplicationDbContext context, UserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public async Task<TeacherDailyAttendanceViewModel> UpsertDailyAttendanceAsync(TeacherAttendanceBatchDto request, CancellationToken cancellationToken)
        {
            ValidateBatchRequest(request);
            var academicYearId = ParseGuid(request.AcademicYearId, "Academic year is required.");
            var teacherIds = request.Entries.Select(x => ParseGuid(x.TeacherId, "Teacher id is required.")).ToList();

            if (teacherIds.Count != teacherIds.Distinct().Count())
            {
                throw new Exception("Duplicate teacher attendance entries are not allowed.");
            }

            ValidateEntries(request.Entries);
            var recordedBy = GetCurrentUserId();

            var existingAttendance = await _context.TeacherAttendances
                .Where(x => teacherIds.Contains(x.TeacherId) &&
                            x.AcademicYearId == academicYearId &&
                            x.AttendanceDateEn == request.AttendanceDateEn &&
                            !x.IsDeleted)
                .ToListAsync(cancellationToken);

            var existingTeacherIds = existingAttendance.Select(x => x.TeacherId).ToHashSet();
            var teachers = await _context.Teachers
                .Where(x => teacherIds.Contains(x.Id) &&
                            !x.IsDeleted &&
                            (x.IsActive || existingTeacherIds.Contains(x.Id)))
                .ToListAsync(cancellationToken);

            if (teachers.Count != teacherIds.Count)
            {
                throw new Exception("One or more selected teachers are not valid for the selected date.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var entry in request.Entries)
                {
                    var teacherId = Guid.Parse(entry.TeacherId);
                    var attendance = existingAttendance.FirstOrDefault(x => x.TeacherId == teacherId);

                    if (attendance == null)
                    {
                        attendance = new TeacherAttendance
                        {
                            Id = Guid.NewGuid(),
                            TeacherId = teacherId,
                            AcademicYearId = academicYearId,
                            AttendanceDateEn = request.AttendanceDateEn,
                            AttendanceDateNp = request.AttendanceDateNp.Trim(),
                            CreatedBy = recordedBy,
                            RecordedByUserId = recordedBy,
                            IsActive = true
                        };
                        await _context.TeacherAttendances.AddAsync(attendance, cancellationToken);
                    }

                    attendance.Status = entry.Status;
                    attendance.CheckInTime = entry.CheckInTime;
                    attendance.CheckOutTime = entry.CheckOutTime;
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

            return await GetAttendanceByDateAsync(request.AcademicYearId, request.AttendanceDateEn, cancellationToken);
        }

        public async Task<TeacherDailyAttendanceViewModel> GetAttendanceByDateAsync(string academicYearId, DateOnly attendanceDateEn, CancellationToken cancellationToken)
        {
            var academicYearGuid = ParseGuid(academicYearId, "Academic year is required.");

            var attendanceRows = await _context.TeacherAttendances
                .Where(x => x.AcademicYearId == academicYearGuid &&
                            x.AttendanceDateEn == attendanceDateEn &&
                            !x.IsDeleted)
                .ToListAsync(cancellationToken);

            var attendedTeacherIds = attendanceRows.Select(x => x.TeacherId).ToHashSet();

            var teachers = await _context.Teachers
                .Where(x => !x.IsDeleted && (x.IsActive || attendedTeacherIds.Contains(x.Id)))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync(cancellationToken);

            var rows = teachers.Select(teacher =>
            {
                var attendance = attendanceRows.FirstOrDefault(x => x.TeacherId == teacher.Id);
                var status = attendance?.Status ?? TeacherAttendanceStatus.Present;
                return new TeacherAttendanceRowViewModel
                {
                    AttendanceId = attendance?.Id,
                    TeacherId = teacher.Id,
                    TeacherName = BuildTeacherName(teacher),
                    EmployeeCode = teacher.EmployeeCode,
                    Designation = teacher.Designation,
                    Status = status,
                    StatusName = status.ToString(),
                    CheckInTime = attendance?.CheckInTime,
                    CheckOutTime = attendance?.CheckOutTime,
                    Remarks = attendance?.Remarks ?? string.Empty,
                    HasAttendance = attendance != null,
                    IsTeacherActive = teacher.IsActive
                };
            }).ToList();

            return new TeacherDailyAttendanceViewModel
            {
                AcademicYearId = academicYearGuid,
                AttendanceDateEn = attendanceDateEn,
                AttendanceDateNp = attendanceRows.Select(x => x.AttendanceDateNp).FirstOrDefault() ?? string.Empty,
                Teachers = rows,
                Summary = BuildTeacherSummary(rows.Where(x => x.HasAttendance).Select(x => x.Status).ToList())
            };
        }

        public async Task<MonthlyAttendanceReportViewModel<TeacherAttendanceReportRowViewModel>> GetMonthlyAttendanceAsync(string academicYearId, int year, int month, string? teacherId, CancellationToken cancellationToken)
        {
            ValidateMonth(year, month);
            var academicYearGuid = ParseGuid(academicYearId, "Academic year is required.");
            var start = new DateOnly(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            Guid? teacherGuid = string.IsNullOrWhiteSpace(teacherId) ? null : ParseGuid(teacherId, "Teacher id is invalid.");

            var query = GetTeacherAttendanceReportQuery()
                .Where(x => x.AcademicYearId == academicYearGuid &&
                            x.AttendanceDateEn >= start &&
                            x.AttendanceDateEn <= end);

            if (teacherGuid.HasValue)
            {
                query = query.Where(x => x.TeacherId == teacherGuid.Value);
            }

            var rows = await query
                .OrderBy(x => x.AttendanceDateEn)
                .ThenBy(x => x.TeacherName)
                .ToListAsync(cancellationToken);

            FillStatusNames(rows);

            return new MonthlyAttendanceReportViewModel<TeacherAttendanceReportRowViewModel>
            {
                AcademicYearId = academicYearGuid,
                Year = year,
                Month = month,
                Rows = rows,
                Summary = BuildTeacherSummary(rows.Select(x => x.Status).ToList())
            };
        }

        public async Task<List<TeacherAttendanceReportRowViewModel>> GetLateArrivalReportAsync(string academicYearId, int year, int month, CancellationToken cancellationToken)
        {
            ValidateMonth(year, month);
            var academicYearGuid = ParseGuid(academicYearId, "Academic year is required.");
            var start = new DateOnly(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            var rows = await GetTeacherAttendanceReportQuery()
                .Where(x => x.AcademicYearId == academicYearGuid &&
                            x.AttendanceDateEn >= start &&
                            x.AttendanceDateEn <= end &&
                            x.Status == TeacherAttendanceStatus.Late)
                .OrderBy(x => x.AttendanceDateEn)
                .ThenBy(x => x.TeacherName)
                .ToListAsync(cancellationToken);

            FillStatusNames(rows);
            return rows;
        }

        public async Task<List<TeacherAttendanceReportRowViewModel>> GetLeaveReportAsync(string academicYearId, int year, int month, CancellationToken cancellationToken)
        {
            ValidateMonth(year, month);
            var academicYearGuid = ParseGuid(academicYearId, "Academic year is required.");
            var start = new DateOnly(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            var rows = await GetTeacherAttendanceReportQuery()
                .Where(x => x.AcademicYearId == academicYearGuid &&
                            x.AttendanceDateEn >= start &&
                            x.AttendanceDateEn <= end &&
                            x.Status == TeacherAttendanceStatus.Leave)
                .OrderBy(x => x.AttendanceDateEn)
                .ThenBy(x => x.TeacherName)
                .ToListAsync(cancellationToken);

            FillStatusNames(rows);
            return rows;
        }

        public async Task<AttendanceSummaryViewModel> GetAttendanceSummaryAsync(string academicYearId, DateOnly attendanceDateEn, CancellationToken cancellationToken)
        {
            var academicYearGuid = ParseGuid(academicYearId, "Academic year is required.");
            var statuses = await _context.TeacherAttendances
                .Where(x => x.AcademicYearId == academicYearGuid &&
                            x.AttendanceDateEn == attendanceDateEn &&
                            !x.IsDeleted)
                .Select(x => x.Status)
                .ToListAsync(cancellationToken);

            return BuildTeacherSummary(statuses);
        }

        public async Task<TeacherAttendanceRowViewModel> CheckInAsync(TeacherCheckInOutDto request, CancellationToken cancellationToken)
        {
            ValidateCheckInOutRequest(request);
            var academicYearGuid = ParseGuid(request.AcademicYearId, "Academic year is required.");
            var teacher = await GetCurrentTeacherAsync(cancellationToken);
            var userId = GetCurrentUserId();

            var attendance = await _context.TeacherAttendances
                .FirstOrDefaultAsync(x => x.TeacherId == teacher.Id &&
                                          x.AcademicYearId == academicYearGuid &&
                                          x.AttendanceDateEn == request.AttendanceDateEn &&
                                          !x.IsDeleted,
                    cancellationToken);

            if (attendance == null)
            {
                attendance = new TeacherAttendance
                {
                    Id = Guid.NewGuid(),
                    TeacherId = teacher.Id,
                    AcademicYearId = academicYearGuid,
                    AttendanceDateEn = request.AttendanceDateEn,
                    AttendanceDateNp = request.AttendanceDateNp.Trim(),
                    Status = TeacherAttendanceStatus.Present,
                    CreatedBy = userId,
                    RecordedByUserId = userId,
                    IsActive = true
                };
                await _context.TeacherAttendances.AddAsync(attendance, cancellationToken);
            }

            attendance.CheckInTime = request.AttendanceTime;
            attendance.AttendanceDateNp = request.AttendanceDateNp.Trim();
            attendance.Remarks = request.Remarks?.Trim() ?? attendance.Remarks;
            attendance.ModifiedBy = userId;
            attendance.RecordedByUserId = userId;
            await _context.SaveChangesAsync(cancellationToken);

            return MapTeacherRow(teacher, attendance);
        }

        public async Task<TeacherAttendanceRowViewModel> CheckOutAsync(TeacherCheckInOutDto request, CancellationToken cancellationToken)
        {
            ValidateCheckInOutRequest(request);
            var academicYearGuid = ParseGuid(request.AcademicYearId, "Academic year is required.");
            var teacher = await GetCurrentTeacherAsync(cancellationToken);
            var userId = GetCurrentUserId();

            var attendance = await _context.TeacherAttendances
                .FirstOrDefaultAsync(x => x.TeacherId == teacher.Id &&
                                          x.AcademicYearId == academicYearGuid &&
                                          x.AttendanceDateEn == request.AttendanceDateEn &&
                                          !x.IsDeleted,
                    cancellationToken);

            if (attendance == null)
            {
                throw new Exception("Check-in is required before check-out.");
            }

            if (attendance.CheckInTime.HasValue && request.AttendanceTime < attendance.CheckInTime.Value)
            {
                throw new Exception("Check-out time cannot be earlier than check-in time.");
            }

            attendance.CheckOutTime = request.AttendanceTime;
            attendance.Remarks = request.Remarks?.Trim() ?? attendance.Remarks;
            attendance.ModifiedBy = userId;
            attendance.RecordedByUserId = userId;
            await _context.SaveChangesAsync(cancellationToken);

            return MapTeacherRow(teacher, attendance);
        }

        public async Task DeleteAttendanceAsync(string attendanceId, CancellationToken cancellationToken)
        {
            var attendanceGuid = ParseGuid(attendanceId, "Attendance id is required.");
            var attendance = await _context.TeacherAttendances
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

        private IQueryable<TeacherAttendanceReportRowViewModel> GetTeacherAttendanceReportQuery()
        {
            return _context.TeacherAttendances
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Select(x => new TeacherAttendanceReportRowViewModel
                {
                    AttendanceId = x.Id,
                    AcademicYearId = x.AcademicYearId,
                    TeacherId = x.TeacherId,
                    TeacherName = (x.Teacher.FirstName + " " + x.Teacher.LastName).Trim(),
                    EmployeeCode = x.Teacher.EmployeeCode,
                    Designation = x.Teacher.Designation,
                    AttendanceDateEn = x.AttendanceDateEn,
                    AttendanceDateNp = x.AttendanceDateNp,
                    Status = x.Status,
                    StatusName = string.Empty,
                    CheckInTime = x.CheckInTime,
                    CheckOutTime = x.CheckOutTime,
                    Remarks = x.Remarks,
                    IsTeacherActive = x.Teacher.IsActive
                });
        }

        private async Task<Teacher> GetCurrentTeacherAsync(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive && !x.IsDeleted, cancellationToken);

            if (teacher == null)
            {
                throw new Exception("Current user is not linked to an active teacher.");
            }

            return teacher;
        }

        private static void ValidateBatchRequest(TeacherAttendanceBatchDto request)
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
                throw new Exception("At least one teacher attendance entry is required.");
            }
        }

        private static void ValidateEntries(IEnumerable<TeacherAttendanceEntryDto> entries)
        {
            foreach (var entry in entries)
            {
                if (!Enum.IsDefined(typeof(TeacherAttendanceStatus), entry.Status))
                {
                    throw new Exception("Invalid teacher attendance status.");
                }

                if (entry.CheckInTime.HasValue && entry.CheckOutTime.HasValue && entry.CheckOutTime.Value < entry.CheckInTime.Value)
                {
                    throw new Exception("Check-out time cannot be earlier than check-in time.");
                }
            }
        }

        private static void ValidateCheckInOutRequest(TeacherCheckInOutDto request)
        {
            if (request == null)
            {
                throw new Exception("Check-in/out request is required.");
            }

            if (string.IsNullOrWhiteSpace(request.AttendanceDateNp))
            {
                throw new Exception("Nepali attendance date is required.");
            }
        }

        private static AttendanceSummaryViewModel BuildTeacherSummary(List<TeacherAttendanceStatus> statuses)
        {
            var total = statuses.Count;
            var present = statuses.Count(x => x == TeacherAttendanceStatus.Present);
            var absent = statuses.Count(x => x == TeacherAttendanceStatus.Absent);
            var late = statuses.Count(x => x == TeacherAttendanceStatus.Late);
            var leave = statuses.Count(x => x == TeacherAttendanceStatus.Leave);
            var halfDay = statuses.Count(x => x == TeacherAttendanceStatus.HalfDay);
            var workFromHome = statuses.Count(x => x == TeacherAttendanceStatus.WorkFromHome);
            var weightedPresent = present + late + workFromHome + (halfDay * 0.5m);

            return new AttendanceSummaryViewModel
            {
                Total = total,
                Present = present,
                Absent = absent,
                Late = late,
                Leave = leave,
                HalfDay = halfDay,
                WorkFromHome = workFromHome,
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

        private static TeacherAttendanceRowViewModel MapTeacherRow(Teacher teacher, TeacherAttendance attendance)
        {
            return new TeacherAttendanceRowViewModel
            {
                AttendanceId = attendance.Id,
                TeacherId = teacher.Id,
                TeacherName = BuildTeacherName(teacher),
                EmployeeCode = teacher.EmployeeCode,
                Designation = teacher.Designation,
                Status = attendance.Status,
                StatusName = attendance.Status.ToString(),
                CheckInTime = attendance.CheckInTime,
                CheckOutTime = attendance.CheckOutTime,
                Remarks = attendance.Remarks,
                HasAttendance = true,
                IsTeacherActive = teacher.IsActive
            };
        }

        private static void FillStatusNames(IEnumerable<TeacherAttendanceReportRowViewModel> rows)
        {
            foreach (var row in rows)
            {
                row.StatusName = row.Status.ToString();
            }
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

        private static string BuildTeacherName(Teacher teacher)
        {
            return $"{teacher.FirstName} {teacher.LastName}".Trim();
        }
    }
}
