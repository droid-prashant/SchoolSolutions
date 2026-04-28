using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Application.Common.Interfaces;
using Application.Exams.ViewModels;
using Application.SubjectMarks.Dtos;
using Application.SubjectMarks.Interfaces;
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
                .AnyAsync(x => x.Id == studentEnrollmentId && x.AcademicYearId == currentAcademicYearId, cancellationToken);

            if (!isValidEnrollment)
            {
                throw new Exception("The selected student enrollment does not belong to the current academic year.");
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
