using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Application.Common.Interfaces;
using Application.Exams.Dtos;
using Application.Exams.Interfaces;
using Application.Exams.ViewModels;
using Domain;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.SubjectMarks
{
    public class ExamService : IExamService
    {
        private readonly IApplicationDbContext _context;
        private readonly UserResolver _userResolver;
        public ExamService(IApplicationDbContext context, UserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        private Guid GetCurrentAcademicYearId()
        {
            return _userResolver.GetAcademicYearGuidOrThrow();
        }

        private async Task EnsureEnrollmentBelongsToCurrentAcademicYear(Guid studentEnrollmentId, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var isValidEnrollment = await _context.StudentEnrollments
                .AnyAsync(x => x.Id == studentEnrollmentId &&
                               x.AcademicYearId == currentAcademicYearId &&
                               x.IsActive &&
                               !x.IsDeleted &&
                               !x.Student.IsDeleted,
                    cancellationToken);

            if (!isValidEnrollment)
            {
                throw new Exception("The selected active student enrollment does not belong to the current academic year.");
            }
        }

        public async Task AddSubjectMarks(SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken)
        {
            var subjectMarkList = subjectMarkDto.StudentMarksLists;
            var studentEnrollmentId = Guid.Parse(subjectMarkDto.StudentId);

            await EnsureEnrollmentBelongsToCurrentAcademicYear(studentEnrollmentId, cancellationToken);

            var gpaAndTotalCreaditHour = calculateGPAAndTotalCreditHour(subjectMarkDto.StudentMarksLists);
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var studentResult = new ExamResult
                {
                    StudentEnrollmentId = studentEnrollmentId,
                    ExamType = subjectMarkDto.ExamType,
                    GPA = gpaAndTotalCreaditHour.GPA,
                    Attendance = subjectMarkDto.Attendance,
                    TotalSchoolDays = subjectMarkDto.TotalSchoolDays,
                    TotalCredit = gpaAndTotalCreaditHour.TotalCreditHour
                };

                await _context.ExamResults.AddAsync(studentResult);
                await _context.SaveChangesAsync(cancellationToken);

                foreach (var subjectMarkObj in subjectMarkList)
                {
                    var calculation = CalculateSubjectPerformance(subjectMarkObj);

                    var subjectMark = new SubjectMark
                    {
                        StudentEnrollmentId = Guid.Parse(subjectMarkDto.StudentId),
                        ClassCourseId = Guid.Parse(subjectMarkObj.ClassCourseId),
                        ExamResultId = studentResult.Id,
                        FullTheoryMarks = subjectMarkObj.IsTheoryRequired ? (subjectMarkObj.TheoryFullMarks ?? 0m) : 0m,
                        FullPracticalMarks = subjectMarkObj.IsPracticalRequired ? (subjectMarkObj.PracticalFullMarks ?? 0m) : 0m,
                        ObtainedTheoryMarks = subjectMarkObj.IsTheoryRequired ? (subjectMarkObj.ObtainedTheoryMarks ?? 0m) : 0m,
                        ObtainedPracticalMarks = subjectMarkObj.IsPracticalRequired ? (subjectMarkObj.ObtainedPracticalMarks ?? 0m) : 0m,
                        GradeTheory = calculation.GradeTheory,
                        GradePointTheory = calculation.TheoryGradePoint,
                        GradePractical = calculation.GradePractical,
                        GradePointPractical = calculation.PracticalGradePoint,
                        FinalGrade = calculation.FinalGrade,
                        FinalGradePoint = calculation.FinalGradePoint
                    };
                    await _context.SubjectMarks.AddAsync(subjectMark);
                }
                await _context.SaveChangesAsync(cancellationToken);
                scope.Complete();
            }
        }

        public async Task UpdateSubjectMarks(SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken)
        {
            var studentEnrollmentId = Guid.Parse(subjectMarkDto.StudentId);
            var subjectMarkList = subjectMarkDto.StudentMarksLists;

            await EnsureEnrollmentBelongsToCurrentAcademicYear(studentEnrollmentId, cancellationToken);

            var gpaAndTotalCreaditHour = calculateGPAAndTotalCreditHour(subjectMarkDto.StudentMarksLists);

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var existingExamResult = await _context.ExamResults
                    .FirstOrDefaultAsync(x =>
                        x.StudentEnrollmentId == studentEnrollmentId &&
                        x.ExamType == subjectMarkDto.ExamType,
                        cancellationToken);

                if (existingExamResult == null)
                {
                    throw new Exception("Exam result not found for update.");
                }

                existingExamResult.Attendance = subjectMarkDto.Attendance;
                existingExamResult.TotalSchoolDays = subjectMarkDto.TotalSchoolDays;
                existingExamResult.GPA = gpaAndTotalCreaditHour.GPA;
                existingExamResult.TotalCredit = gpaAndTotalCreaditHour.TotalCreditHour;

                var existingSubjectMarks = await _context.SubjectMarks
                    .Where(x => x.ExamResultId == existingExamResult.Id)
                    .ToListAsync(cancellationToken);

                foreach (var subjectMarkObj in subjectMarkList)
                {
                    var classCourseId = Guid.Parse(subjectMarkObj.ClassCourseId);
                    var calculation = CalculateSubjectPerformance(subjectMarkObj);

                    var existingSubjectMark = existingSubjectMarks
                        .FirstOrDefault(x => x.ClassCourseId == classCourseId);

                    if (existingSubjectMark != null)
                    {
                        existingSubjectMark.FullTheoryMarks = subjectMarkObj.IsTheoryRequired ? (subjectMarkObj.TheoryFullMarks ?? 0m) : 0m;
                        existingSubjectMark.FullPracticalMarks = subjectMarkObj.IsPracticalRequired ? (subjectMarkObj.PracticalFullMarks ?? 0m) : 0m;
                        existingSubjectMark.ObtainedTheoryMarks = subjectMarkObj.IsTheoryRequired ? (subjectMarkObj.ObtainedTheoryMarks ?? 0m) : 0m;
                        existingSubjectMark.ObtainedPracticalMarks = subjectMarkObj.IsPracticalRequired ? (subjectMarkObj.ObtainedPracticalMarks ?? 0m) : 0m;
                        existingSubjectMark.GradeTheory = calculation.GradeTheory;
                        existingSubjectMark.GradePointTheory = calculation.TheoryGradePoint;
                        existingSubjectMark.GradePractical = calculation.GradePractical;
                        existingSubjectMark.GradePointPractical = calculation.PracticalGradePoint;
                        existingSubjectMark.FinalGrade = calculation.FinalGrade;
                        existingSubjectMark.FinalGradePoint = calculation.FinalGradePoint;
                    }
                    else
                    {
                        var newSubjectMark = new SubjectMark
                        {
                            StudentEnrollmentId = studentEnrollmentId,
                            ClassCourseId = classCourseId,
                            ExamResultId = existingExamResult.Id,
                            FullTheoryMarks = subjectMarkObj.IsTheoryRequired ? (subjectMarkObj.TheoryFullMarks ?? 0m) : 0m,
                            FullPracticalMarks = subjectMarkObj.IsPracticalRequired ? (subjectMarkObj.PracticalFullMarks ?? 0m) : 0m,
                            ObtainedTheoryMarks = subjectMarkObj.IsTheoryRequired ? (subjectMarkObj.ObtainedTheoryMarks ?? 0m) : 0m,
                            ObtainedPracticalMarks = subjectMarkObj.IsPracticalRequired ? (subjectMarkObj.ObtainedPracticalMarks ?? 0m) : 0m,
                            GradeTheory = calculation.GradeTheory,
                            GradePointTheory = calculation.TheoryGradePoint,
                            GradePractical = calculation.GradePractical,
                            GradePointPractical = calculation.PracticalGradePoint,
                            FinalGrade = calculation.FinalGrade,
                            FinalGradePoint = calculation.FinalGradePoint
                        };

                        await _context.SubjectMarks.AddAsync(newSubjectMark, cancellationToken);
                    }
                }

                var incomingClassCourseIds = subjectMarkList
                    .Select(x => Guid.Parse(x.ClassCourseId))
                    .ToHashSet();

                var subjectMarksToDelete = existingSubjectMarks
                    .Where(x => !incomingClassCourseIds.Contains(x.ClassCourseId))
                    .ToList();

                if (subjectMarksToDelete.Any())
                {
                    _context.SubjectMarks.RemoveRange(subjectMarksToDelete);
                }

                await _context.SaveChangesAsync(cancellationToken);
                scope.Complete();
            }
        }

        public async Task UpsertTeacherSubjectMarks(SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken)
        {
            var studentEnrollmentId = Guid.Parse(subjectMarkDto.StudentId);
            var subjectMarkList = subjectMarkDto.StudentMarksLists;

            if (subjectMarkList.Count != 1)
            {
                throw new Exception("Teacher marks entry must contain exactly one subject.");
            }

            await EnsureEnrollmentBelongsToCurrentAcademicYear(studentEnrollmentId, cancellationToken);
            var classCourseId = Guid.Parse(subjectMarkList[0].ClassCourseId);
            await EnsureCurrentTeacherCanEnterMarks(studentEnrollmentId, classCourseId, cancellationToken);

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var existingExamResult = await _context.ExamResults
                    .Include(x => x.SubjectMarks)
                    .FirstOrDefaultAsync(x =>
                        x.StudentEnrollmentId == studentEnrollmentId &&
                        x.ExamType == subjectMarkDto.ExamType,
                        cancellationToken);

                if (existingExamResult == null)
                {
                    existingExamResult = new ExamResult
                    {
                        Id = Guid.NewGuid(),
                        StudentEnrollmentId = studentEnrollmentId,
                        ExamType = subjectMarkDto.ExamType,
                        Attendance = subjectMarkDto.Attendance,
                        TotalSchoolDays = subjectMarkDto.TotalSchoolDays,
                        GPA = 0m,
                        TotalCredit = 0m
                    };

                    await _context.ExamResults.AddAsync(existingExamResult, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    existingExamResult.Attendance = subjectMarkDto.Attendance;
                    existingExamResult.TotalSchoolDays = subjectMarkDto.TotalSchoolDays;
                }

                var subjectMarkObj = subjectMarkList[0];
                var calculation = CalculateSubjectPerformance(subjectMarkObj);
                var existingSubjectMark = await _context.SubjectMarks
                    .FirstOrDefaultAsync(x =>
                        x.ExamResultId == existingExamResult.Id &&
                        x.ClassCourseId == classCourseId,
                        cancellationToken);

                if (existingSubjectMark == null)
                {
                    existingSubjectMark = new SubjectMark
                    {
                        Id = Guid.NewGuid(),
                        StudentEnrollmentId = studentEnrollmentId,
                        ClassCourseId = classCourseId,
                        ExamResultId = existingExamResult.Id
                    };
                    await _context.SubjectMarks.AddAsync(existingSubjectMark, cancellationToken);
                }

                ApplySubjectMarkValues(existingSubjectMark, subjectMarkObj, calculation);
                await _context.SaveChangesAsync(cancellationToken);
                await RecalculateExamResult(existingExamResult.Id, cancellationToken);
                scope.Complete();
            }
        }

        public async Task DeleteTeacherSubjectMarks(string studentEnrollmentId, int examType, string classCourseId, CancellationToken cancellationToken)
        {
            var studentEnrollmentGuid = Guid.Parse(studentEnrollmentId);
            var classCourseGuid = Guid.Parse(classCourseId);

            await EnsureEnrollmentBelongsToCurrentAcademicYear(studentEnrollmentGuid, cancellationToken);
            await EnsureCurrentTeacherCanEnterMarks(studentEnrollmentGuid, classCourseGuid, cancellationToken);

            var examResult = await _context.ExamResults
                .Include(x => x.SubjectMarks)
                .FirstOrDefaultAsync(x =>
                    x.StudentEnrollmentId == studentEnrollmentGuid &&
                    x.ExamType == examType,
                    cancellationToken);

            if (examResult == null)
            {
                throw new Exception("Exam result not found.");
            }

            var subjectMark = examResult.SubjectMarks.FirstOrDefault(x => x.ClassCourseId == classCourseGuid);
            if (subjectMark == null)
            {
                throw new Exception("Subject marks entry not found.");
            }

            _context.SubjectMarks.Remove(subjectMark);
            await _context.SaveChangesAsync(cancellationToken);

            var hasRemainingMarks = await _context.SubjectMarks.AnyAsync(x => x.ExamResultId == examResult.Id, cancellationToken);
            if (!hasRemainingMarks)
            {
                _context.ExamResults.Remove(examResult);
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            await RecalculateExamResult(examResult.Id, cancellationToken);
        }

        public async Task<SubjectMarksViewModel> GetStudentMarks(string studentEnrollmentId, int examType, CancellationToken cancellationToken)
        {
            var studentEnrollmentGuid = Guid.Parse(studentEnrollmentId);
            await EnsureEnrollmentBelongsToCurrentAcademicYear(studentEnrollmentGuid, cancellationToken);

            var subjectMarks = await _context.ExamResults.Where(x => x.StudentEnrollmentId == studentEnrollmentGuid && x.ExamType == examType)
                                                         .Select(s => new SubjectMarksViewModel
                                                         {
                                                             ExamType = s.ExamType,
                                                             Attendance = s.Attendance,
                                                             TotalSchoolDays = s.TotalSchoolDays,
                                                             StudentId = s.StudentEnrollmentId.ToString(),
                                                             StudentMarksLists = s.SubjectMarks.Select(sm => new StudentMarksList
                                                             {
                                                                 ClassCourseId = sm.ClassCourseId.ToString(),
                                                                 IsTheoryRequired = sm.ClassCourse.IsTheoryRequired,
                                                                 IsPracticalRequired = sm.ClassCourse.IsPracticalRequired,
                                                                 ObtainedTheoryMarks = sm.ClassCourse.IsTheoryRequired ? sm.ObtainedTheoryMarks : null,
                                                                 ObtainedPracticalMarks = sm.ClassCourse.IsPracticalRequired ? sm.ObtainedPracticalMarks : null,
                                                                 TheoryCredit = sm.ClassCourse.TheoryCreditHour,
                                                                 PracticalFullMarks = sm.ClassCourse.IsPracticalRequired ? sm.FullPracticalMarks : null,
                                                                 TheoryFullMarks = sm.ClassCourse.IsTheoryRequired ? sm.FullTheoryMarks : null,
                                                                 PracticalCredit = sm.ClassCourse.PracticalCreditHour
                                                             }).ToList()
                                                         }).FirstOrDefaultAsync();

            return subjectMarks;
        }

        public async Task<SubjectMarksViewModel?> GetTeacherStudentSubjectMarks(string studentEnrollmentId, int examType, string classCourseId, CancellationToken cancellationToken)
        {
            var studentEnrollmentGuid = Guid.Parse(studentEnrollmentId);
            var classCourseGuid = Guid.Parse(classCourseId);

            await EnsureEnrollmentBelongsToCurrentAcademicYear(studentEnrollmentGuid, cancellationToken);
            await EnsureCurrentTeacherCanEnterMarks(studentEnrollmentGuid, classCourseGuid, cancellationToken);

            return await _context.ExamResults
                .Where(x => x.StudentEnrollmentId == studentEnrollmentGuid && x.ExamType == examType)
                .Select(s => new SubjectMarksViewModel
                {
                    ExamType = s.ExamType,
                    Attendance = s.Attendance,
                    TotalSchoolDays = s.TotalSchoolDays,
                    StudentId = s.StudentEnrollmentId.ToString(),
                    StudentMarksLists = s.SubjectMarks
                        .Where(sm => sm.ClassCourseId == classCourseGuid)
                        .Select(sm => new StudentMarksList
                        {
                            ClassCourseId = sm.ClassCourseId.ToString(),
                            IsTheoryRequired = sm.ClassCourse.IsTheoryRequired,
                            IsPracticalRequired = sm.ClassCourse.IsPracticalRequired,
                            ObtainedTheoryMarks = sm.ClassCourse.IsTheoryRequired ? sm.ObtainedTheoryMarks : null,
                            ObtainedPracticalMarks = sm.ClassCourse.IsPracticalRequired ? sm.ObtainedPracticalMarks : null,
                            TheoryCredit = sm.ClassCourse.TheoryCreditHour,
                            PracticalFullMarks = sm.ClassCourse.IsPracticalRequired ? sm.FullPracticalMarks : null,
                            TheoryFullMarks = sm.ClassCourse.IsTheoryRequired ? sm.FullTheoryMarks : null,
                            PracticalCredit = sm.ClassCourse.PracticalCreditHour
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<TeacherMarksAssignmentViewModel>> GetTeacherMarksAssignments(CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();

            var assignmentQuery = _context.TeacherClassSections
                .Where(x =>
                    x.AcademicYearId == currentAcademicYearId &&
                    x.IsActive &&
                    !x.IsDeleted &&
                    x.Teacher.IsActive &&
                    !x.Teacher.IsDeleted);

            if (!_userResolver.IsSuperAdmin)
            {
                var teacherId = await GetCurrentTeacherId(cancellationToken);
                assignmentQuery = assignmentQuery.Where(x => x.TeacherId == teacherId);
            }

            return await assignmentQuery
                .Join(_context.ClassCourses.Where(x => !x.IsDeleted),
                    assignment => new { assignment.CourseId, ClassRoomId = assignment.ClassSection.ClassId },
                    classCourse => new { classCourse.CourseId, classCourse.ClassRoomId },
                    (assignment, classCourse) => new TeacherMarksAssignmentViewModel
                    {
                        AssignmentId = assignment.Id,
                        TeacherId = assignment.TeacherId,
                        TeacherName = (assignment.Teacher.FirstName + " " + assignment.Teacher.LastName).Trim(),
                        ClassSectionId = assignment.ClassSectionId,
                        ClassRoomId = assignment.ClassSection.ClassId,
                        ClassRoomName = assignment.ClassSection.ClassRoom.Name,
                        SectionId = assignment.ClassSection.SectionId,
                        SectionName = assignment.ClassSection.Section.Name,
                        CourseId = assignment.CourseId,
                        ClassCourseId = classCourse.Id,
                        CourseName = assignment.Course.Name,
                        IsTheoryRequired = classCourse.IsTheoryRequired,
                        IsPracticalRequired = classCourse.IsPracticalRequired,
                        TheoryFullMarks = classCourse.TheoryFullMarks,
                        PracticalFullMarks = classCourse.PracticalFullMarks,
                        TheoryCreditHour = classCourse.TheoryCreditHour,
                        PracticalCreditHour = classCourse.PracticalCreditHour
                    })
                .OrderBy(x => x.ClassRoomName)
                .ThenBy(x => x.SectionName)
                .ThenBy(x => x.TeacherName)
                .ThenBy(x => x.CourseName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TeacherSubjectStudentMarksViewModel>> GetTeacherSubjectStudentMarks(string classSectionId, string classCourseId, int examType, string? keyword, CancellationToken cancellationToken)
        {
            var classSectionGuid = Guid.Parse(classSectionId);
            var classCourseGuid = Guid.Parse(classCourseId);
            var currentAcademicYearId = GetCurrentAcademicYearId();

            if (_userResolver.IsSuperAdmin)
            {
                await EnsureClassCourseBelongsToClassSection(classSectionGuid, classCourseGuid, cancellationToken);
            }
            else
            {
                var teacherId = await GetCurrentTeacherId(cancellationToken);
                var isAssigned = await IsTeacherAssignedToSubject(teacherId, classSectionGuid, classCourseGuid, cancellationToken);
                if (!isAssigned)
                {
                    throw new UnauthorizedAccessException("You are not assigned to this subject.");
                }
            }

            var query = _context.StudentEnrollments
                .Where(x =>
                    x.AcademicYearId == currentAcademicYearId &&
                    x.ClassSectionId == classSectionGuid &&
                    x.IsActive &&
                    !x.IsDeleted &&
                    !x.Student.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var cleanKeyword = keyword.Trim();
                query = query.Where(x =>
                    x.Student.FirstName.Contains(cleanKeyword) ||
                    x.Student.LastName.Contains(cleanKeyword) ||
                    (x.RegistrationNumber != null && x.RegistrationNumber.Contains(cleanKeyword)) ||
                    (x.SymbolNumber != null && x.SymbolNumber.Contains(cleanKeyword)));
            }

            return await query
                .OrderBy(x => x.RollNumber)
                .ThenBy(x => x.Student.FirstName)
                .Select(x => new TeacherSubjectStudentMarksViewModel
                {
                    StudentEnrollmentId = x.Id,
                    StudentName = x.Student.FirstName + " " + x.Student.LastName,
                    RollNumber = x.RollNumber,
                    Attendance = x.ExamResults.Where(r => r.ExamType == examType).Select(r => (int?)r.Attendance).FirstOrDefault(),
                    TotalSchoolDays = x.ExamResults.Where(r => r.ExamType == examType).Select(r => (int?)r.TotalSchoolDays).FirstOrDefault(),
                    HasMarksEntry = x.ExamResults
                        .Where(r => r.ExamType == examType)
                        .SelectMany(r => r.SubjectMarks)
                        .Any(m => m.ClassCourseId == classCourseGuid),
                    Marks = x.ExamResults
                        .Where(r => r.ExamType == examType)
                        .SelectMany(r => r.SubjectMarks)
                        .Where(m => m.ClassCourseId == classCourseGuid)
                        .Select(m => new StudentMarksList
                        {
                            ClassCourseId = m.ClassCourseId.ToString(),
                            IsTheoryRequired = m.ClassCourse.IsTheoryRequired,
                            IsPracticalRequired = m.ClassCourse.IsPracticalRequired,
                            TheoryCredit = m.ClassCourse.TheoryCreditHour,
                            PracticalCredit = m.ClassCourse.PracticalCreditHour,
                            TheoryFullMarks = m.ClassCourse.IsTheoryRequired ? m.FullTheoryMarks : null,
                            PracticalFullMarks = m.ClassCourse.IsPracticalRequired ? m.FullPracticalMarks : null,
                            ObtainedTheoryMarks = m.ClassCourse.IsTheoryRequired ? m.ObtainedTheoryMarks : null,
                            ObtainedPracticalMarks = m.ClassCourse.IsPracticalRequired ? m.ObtainedPracticalMarks : null
                        })
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);
        }

        private (decimal GPA, decimal TotalCreditHour) calculateGPAAndTotalCreditHour(List<StudentMarksList> subjectMarkList)
        {
            decimal weightedGP = 0.00m;
            decimal totalCreditHour = 0.00m;
            foreach (var subjectMarkObj in subjectMarkList)
            {
                var calculation = CalculateSubjectPerformance(subjectMarkObj);

                if (calculation.TotalCredit <= 0m)
                {
                    continue;
                }

                weightedGP += (calculation.TotalCredit * calculation.FinalGradePoint);
                totalCreditHour += calculation.TotalCredit;

            }
            if (totalCreditHour <= 0m)
            {
                return (0m, 0m);
            }

            var gpaFloat = weightedGP / totalCreditHour;
            var gpa = Math.Floor(gpaFloat * 100) / 100;
            return (gpa, totalCreditHour);
        }

        private async Task<Guid> GetCurrentTeacherId(CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userResolver.UserId, out var userId))
            {
                throw new UnauthorizedAccessException("Current user is not available.");
            }

            var teacherId = await _context.Teachers
                .Where(x => x.UserId == userId && x.IsActive && !x.IsDeleted)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (!teacherId.HasValue)
            {
                throw new UnauthorizedAccessException("Current user is not linked to an active teacher.");
            }

            return teacherId.Value;
        }

        private async Task EnsureCurrentTeacherCanEnterMarks(Guid studentEnrollmentId, Guid classCourseId, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();

            var enrollment = await _context.StudentEnrollments
                .Include(x => x.Student)
                .Include(x => x.ClassSection)
                .FirstOrDefaultAsync(x => x.Id == studentEnrollmentId &&
                                          x.AcademicYearId == currentAcademicYearId &&
                                          x.IsActive &&
                                          !x.IsDeleted &&
                                          !x.Student.IsDeleted,
                    cancellationToken);

            if (enrollment == null)
            {
                throw new Exception("The selected active student enrollment does not belong to the current academic year.");
            }

            if (_userResolver.IsSuperAdmin)
            {
                await EnsureClassCourseBelongsToClassSection(enrollment.ClassSectionId, classCourseId, cancellationToken);
                return;
            }

            var teacherId = await GetCurrentTeacherId(cancellationToken);
            var isAssigned = await IsTeacherAssignedToSubject(teacherId, enrollment.ClassSectionId, classCourseId, cancellationToken);
            if (!isAssigned)
            {
                throw new UnauthorizedAccessException("You are not assigned to enter marks for this subject.");
            }
        }

        private async Task EnsureClassCourseBelongsToClassSection(Guid classSectionId, Guid classCourseId, CancellationToken cancellationToken)
        {
            var isValidClassCourse = await _context.ClassSections
                .Where(x => x.Id == classSectionId)
                .Join(_context.ClassCourses.Where(x => x.Id == classCourseId && !x.IsDeleted),
                    classSection => classSection.ClassId,
                    classCourse => classCourse.ClassRoomId,
                    (classSection, classCourse) => classCourse)
                .AnyAsync(cancellationToken);

            if (!isValidClassCourse)
            {
                throw new UnauthorizedAccessException("The selected subject is not mapped to this class section.");
            }
        }

        private async Task<bool> IsTeacherAssignedToSubject(Guid teacherId, Guid classSectionId, Guid classCourseId, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();

            return await _context.TeacherClassSections
                .Where(x =>
                    x.TeacherId == teacherId &&
                    x.AcademicYearId == currentAcademicYearId &&
                    x.ClassSectionId == classSectionId &&
                    x.IsActive &&
                    !x.IsDeleted)
                .Join(_context.ClassCourses.Where(x => x.Id == classCourseId && !x.IsDeleted),
                    assignment => new { assignment.CourseId, ClassRoomId = assignment.ClassSection.ClassId },
                    classCourse => new { classCourse.CourseId, classCourse.ClassRoomId },
                    (assignment, classCourse) => assignment)
                .AnyAsync(cancellationToken);
        }

        private void ApplySubjectMarkValues(SubjectMark subjectMark, StudentMarksList subjectMarkObj, SubjectCalculationResult calculation)
        {
            subjectMark.FullTheoryMarks = subjectMarkObj.IsTheoryRequired ? (subjectMarkObj.TheoryFullMarks ?? 0m) : 0m;
            subjectMark.FullPracticalMarks = subjectMarkObj.IsPracticalRequired ? (subjectMarkObj.PracticalFullMarks ?? 0m) : 0m;
            subjectMark.ObtainedTheoryMarks = subjectMarkObj.IsTheoryRequired ? (subjectMarkObj.ObtainedTheoryMarks ?? 0m) : 0m;
            subjectMark.ObtainedPracticalMarks = subjectMarkObj.IsPracticalRequired ? (subjectMarkObj.ObtainedPracticalMarks ?? 0m) : 0m;
            subjectMark.GradeTheory = calculation.GradeTheory;
            subjectMark.GradePointTheory = calculation.TheoryGradePoint;
            subjectMark.GradePractical = calculation.GradePractical;
            subjectMark.GradePointPractical = calculation.PracticalGradePoint;
            subjectMark.FinalGrade = calculation.FinalGrade;
            subjectMark.FinalGradePoint = calculation.FinalGradePoint;
        }

        private async Task RecalculateExamResult(Guid examResultId, CancellationToken cancellationToken)
        {
            var examResult = await _context.ExamResults
                .Include(x => x.SubjectMarks)
                    .ThenInclude(x => x.ClassCourse)
                .FirstOrDefaultAsync(x => x.Id == examResultId, cancellationToken);

            if (examResult == null)
            {
                return;
            }

            decimal weightedGP = 0m;
            decimal totalCreditHour = 0m;
            foreach (var subjectMark in examResult.SubjectMarks)
            {
                var theoryCredit = subjectMark.ClassCourse.IsTheoryRequired ? (subjectMark.ClassCourse.TheoryCreditHour ?? 0m) : 0m;
                var practicalCredit = subjectMark.ClassCourse.IsPracticalRequired ? (subjectMark.ClassCourse.PracticalCreditHour ?? 0m) : 0m;
                var subjectCredit = theoryCredit + practicalCredit;
                if (subjectCredit <= 0m)
                {
                    continue;
                }

                weightedGP += subjectCredit * subjectMark.FinalGradePoint;
                totalCreditHour += subjectCredit;
            }

            examResult.TotalCredit = totalCreditHour;
            examResult.GPA = totalCreditHour <= 0m ? 0m : Math.Floor((weightedGP / totalCreditHour) * 100) / 100;
            await _context.SaveChangesAsync(cancellationToken);
        }

        private string calculateGrade(decimal obtainedMarks, decimal fullMarks)
        {
            decimal percent = (obtainedMarks / fullMarks) * 100;
            string grade = percent switch
            {
                >= 90 => "A+",
                >= 80 => "A",
                >= 70 => "B+",
                >= 60 => "B",
                >= 50 => "C+",
                >= 40 => "C",
                >= 35 => "D+",
                >= 0 => "NQ",   // Fail
                _ => "N/A"      // Invalid safeguard
            };
            return grade;
        }

        private string calculateFinalGrade(decimal gradePoint)
        {
            var grade = gradePoint switch
            {
                >= 4.0m => "A+",
                >= 3.6m => "A",
                >= 3.2m => "B+",
                >= 2.8m => "B",
                >= 2.4m => "C+",
                >= 2.0m => "C",
                >= 1.6m => "D+",
                >= 0.8m => "NQ",   // Fail
                _ => "N/A"
            };
            return grade;
        }

        private (string Grade, decimal GradePoint) GetGradeAndPoint(decimal obtainedMarks, decimal fullMarks)
        {
            decimal percent = (obtainedMarks / fullMarks) * 100;

            return percent switch
            {
                >= 90 => ("A+", 4.0m),
                >= 80 => ("A", 3.6m),
                >= 70 => ("B+", 3.2m),
                >= 60 => ("B", 2.8m),
                >= 50 => ("C+", 2.4m),
                >= 40 => ("C", 2.0m),
                >= 35 => ("D+", 1.6m),
                >= 0 => ("NQ", 0.8m),  // Fail
                _ => ("N/A", 0.0m)  // Invalid safeguard
            };
        }

        private decimal GetFinalGradePoint(decimal calculatedPoint)
        {
            if (calculatedPoint >= 4.0m) return 4.0m;
            if (calculatedPoint >= 3.6m) return 3.6m;
            if (calculatedPoint >= 3.2m) return 3.2m;
            if (calculatedPoint >= 2.8m) return 2.8m;
            if (calculatedPoint >= 2.4m) return 2.4m;
            if (calculatedPoint >= 2.0m) return 2.0m;
            if (calculatedPoint >= 1.6m) return 1.6m;
            if (calculatedPoint >= 1.2m) return 1.2m;
            return 0.8m;
        }

        private SubjectCalculationResult CalculateSubjectPerformance(StudentMarksList subjectMarkObj)
        {
            var theory = CalculateComponentResult(
                subjectMarkObj.IsTheoryRequired,
                subjectMarkObj.ObtainedTheoryMarks,
                subjectMarkObj.TheoryFullMarks,
                subjectMarkObj.TheoryCredit);

            var practical = CalculateComponentResult(
                subjectMarkObj.IsPracticalRequired,
                subjectMarkObj.ObtainedPracticalMarks,
                subjectMarkObj.PracticalFullMarks,
                subjectMarkObj.PracticalCredit);

            var totalCredit = theory.Credit + practical.Credit;
            if (totalCredit <= 0m)
            {
                return new SubjectCalculationResult
                {
                    GradeTheory = theory.Grade,
                    TheoryGradePoint = theory.GradePoint,
                    GradePractical = practical.Grade,
                    PracticalGradePoint = practical.GradePoint,
                    FinalGrade = "N/A",
                    FinalGradePoint = 0m,
                    TotalCredit = 0m
                };
            }

            // A student must pass every required component. If theory or practical
            // fails individually, the subject final result must also be NQ.
            var hasFailedRequiredTheory = subjectMarkObj.IsTheoryRequired && theory.Grade == "NQ";
            var hasFailedRequiredPractical = subjectMarkObj.IsPracticalRequired && practical.Grade == "NQ";

            if (hasFailedRequiredTheory || hasFailedRequiredPractical)
            {
                return new SubjectCalculationResult
                {
                    GradeTheory = theory.Grade,
                    TheoryGradePoint = theory.GradePoint,
                    GradePractical = practical.Grade,
                    PracticalGradePoint = practical.GradePoint,
                    FinalGrade = "NQ",
                    FinalGradePoint = 0.8m,
                    TotalCredit = totalCredit
                };
            }

            decimal finalGradePointFloat = ((theory.GradePoint * theory.Credit) + (practical.GradePoint * practical.Credit)) / totalCredit;
            decimal finalGradePointCalculated = Math.Floor(finalGradePointFloat * 100) / 100;
            decimal finalGradePoint = GetFinalGradePoint(finalGradePointCalculated);

            return new SubjectCalculationResult
            {
                GradeTheory = theory.Grade,
                TheoryGradePoint = theory.GradePoint,
                GradePractical = practical.Grade,
                PracticalGradePoint = practical.GradePoint,
                FinalGrade = calculateFinalGrade(finalGradePoint),
                FinalGradePoint = finalGradePoint,
                TotalCredit = totalCredit
            };
        }

        private SubjectComponentResult CalculateComponentResult(bool isRequired, decimal? obtainedMarks, decimal? fullMarks, decimal? credit)
        {
            if (!isRequired || !fullMarks.HasValue || !credit.HasValue || fullMarks.Value <= 0m || credit.Value <= 0m)
            {
                return new SubjectComponentResult
                {
                    Grade = string.Empty,
                    GradePoint = 0m,
                    Credit = 0m
                };
            }

            var safeObtainedMarks = obtainedMarks ?? 0m;
            var gradeAndPoint = GetGradeAndPoint(safeObtainedMarks, fullMarks.Value);

            return new SubjectComponentResult
            {
                Grade = calculateGrade(safeObtainedMarks, fullMarks.Value),
                GradePoint = gradeAndPoint.GradePoint,
                Credit = credit.Value
            };
        }

        public async Task<ResultViewModel> GetResult(Guid studentEnrollmentId, int? examType, CancellationToken cancellationToken)
        {
            await EnsureEnrollmentBelongsToCurrentAcademicYear(studentEnrollmentId, cancellationToken);

            var resultQuery = _context.ExamResults.Where(x => x.StudentEnrollmentId == studentEnrollmentId);

            if (examType.HasValue)
            {
                resultQuery = resultQuery.Where(x => x.ExamType == examType.Value);
            }

            var result = await resultQuery.OrderByDescending(x => x.CreatedDate)
                                                   .Select(x => new ResultViewModel
                                                   {
                                                       ExamType = x.ExamType,
                                                       TotalCredit = x.TotalCredit,
                                                       GPA = x.GPA,
                                                        StudentName = x.StudentEnrollment.Student.FirstName + ' ' + x.StudentEnrollment.Student.LastName,
                                                        FatherName = x.StudentEnrollment.Student.FatherName,
                                                        MotherName = x.StudentEnrollment.Student.MotherName,
                                                        DateOfBirth = x.StudentEnrollment.Student.DateOfBirthNp,
                                                        Address = x.StudentEnrollment.Student.Municipality.MunicipalityName + " - " + x.StudentEnrollment.Student.WardNo,
                                                        ClassRoom = x.StudentEnrollment.ClassSection.ClassRoom.Name,
                                                        Section = x.StudentEnrollment.ClassSection.Section.Name,
                                                       RollNo = (int)(x.StudentEnrollment.RollNumber != null ? x.StudentEnrollment.RollNumber : 0),
                                                       WardNo = x.StudentEnrollment.Student.WardNo,
                                                       Attendance = x.Attendance,
                                                       TotalSchoolDays = x.TotalSchoolDays,
                                                       StudentMarks = x.SubjectMarks.Where(x => x.ClassCourseId == x.ClassCourseId)
                                                                                   .OrderBy(s => s.ClassCourse.Course.Name == "English" ? 0 :
                                                                                                 s.ClassCourse.Course.Name == "Nepali" ? 1 :
                                                                                                 s.ClassCourse.Course.Name == "Mathematics" ? 2 :
                                                                                                 s.ClassCourse.Course.Name == "Science" ? 3 : 4)
                                                                                   .ThenBy(s => s.ClassCourse.Course.Name)
                                                                                   .Select(s => new StudentMarksViewModel
                                                                                    {
                                                                                        CourseName = s.ClassCourse.Course.Name,
                                                                                        TheoryCreditHour = s.ClassCourse.TheoryCreditHour,
                                                                                        PracticalCreditHour = s.ClassCourse.PracticalCreditHour,
                                                                                        FinalGrade = s.FinalGrade,
                                                                                        FinalGradePoint = s.FinalGrade == "NQ" || s.FinalGrade == "NG"
                                                                                            ? null
                                                                                            : s.FinalGradePoint,
                                                                                        GradePractical = s.ClassCourse.IsPracticalRequired ? s.GradePractical : string.Empty,
                                                                                        GradeTheory = s.ClassCourse.IsTheoryRequired ? s.GradeTheory : string.Empty
                                                                                    }).ToList(),
                                                       IssueDate = x.CreatedDate
                                                   }).FirstOrDefaultAsync();
            return result;
        }

        private sealed class SubjectCalculationResult
        {
            public string GradeTheory { get; set; } = string.Empty;
            public decimal TheoryGradePoint { get; set; }
            public string GradePractical { get; set; } = string.Empty;
            public decimal PracticalGradePoint { get; set; }
            public string FinalGrade { get; set; } = string.Empty;
            public decimal FinalGradePoint { get; set; }
            public decimal TotalCredit { get; set; }
        }

        private sealed class SubjectComponentResult
        {
            public string Grade { get; set; } = string.Empty;
            public decimal GradePoint { get; set; }
            public decimal Credit { get; set; }
        }
    }
}
