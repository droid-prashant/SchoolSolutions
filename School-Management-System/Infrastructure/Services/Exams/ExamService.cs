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
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.SubjectMarks
{
    public class ExamService : IExamService
    {
        private readonly IApplicationDbContext _context;
        public ExamService(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddSubjectMarks(SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken)
        {
            var subjectMarkList = subjectMarkDto.StudentMarksLists;

            var gpaAndTotalCreaditHour = calculateGPAAndTotalCreditHour(subjectMarkDto.StudentMarksLists);
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var studentResult = new ExamResult
                {
                    StudentEnrollmentId = Guid.Parse(subjectMarkDto.StudentId),
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
                    var theoryGradeAndPoint = GetGradeAndPoint(subjectMarkObj.ObtainedTheoryMarks, subjectMarkObj.TheoryFullMarks);
                    var practicalGradeAndPoint = GetGradeAndPoint(subjectMarkObj.ObtainedPracticalMarks, subjectMarkObj.PracticalFullMarks);

                    decimal totalCredit = subjectMarkObj.TheoryCredit + subjectMarkObj.PracticalCredit;
                    decimal finalGradePointFloat = ((theoryGradeAndPoint.GradePoint * subjectMarkObj.TheoryCredit) + (practicalGradeAndPoint.GradePoint * subjectMarkObj.PracticalCredit)) / totalCredit;
                    decimal finalGradePointCalculated = Math.Floor(finalGradePointFloat * 100) / 100;
                    decimal finalGradePoint = GetFinalGradePoint(finalGradePointCalculated);
                    string gradeTheory = calculateGrade(subjectMarkObj.ObtainedTheoryMarks, subjectMarkObj.TheoryFullMarks);
                    string gradePractical = calculateGrade(subjectMarkObj.ObtainedPracticalMarks, subjectMarkObj.PracticalFullMarks);
                    string finalGrade = calculateFinalGrade(finalGradePoint);

                    var subjectMark = new SubjectMark
                    {
                        StudentEnrollmentId = Guid.Parse(subjectMarkDto.StudentId),
                        ClassCourseId = Guid.Parse(subjectMarkObj.ClassCourseId),
                        ExamResultId = studentResult.Id,
                        FullTheoryMarks = subjectMarkObj.TheoryFullMarks,
                        FullPracticalMarks = subjectMarkObj.PracticalFullMarks,
                        ObtainedTheoryMarks = subjectMarkObj.ObtainedTheoryMarks,
                        ObtainedPracticalMarks = subjectMarkObj.ObtainedPracticalMarks,
                        GradeTheory = gradeTheory,
                        GradePointTheory = theoryGradeAndPoint.GradePoint,
                        GradePractical = gradePractical,
                        GradePointPractical = practicalGradeAndPoint.GradePoint,
                        FinalGrade = finalGrade,
                        FinalGradePoint = finalGradePoint
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

                    var theoryGradeAndPoint = GetGradeAndPoint(subjectMarkObj.ObtainedTheoryMarks, subjectMarkObj.TheoryFullMarks);
                    var practicalGradeAndPoint = GetGradeAndPoint(subjectMarkObj.ObtainedPracticalMarks, subjectMarkObj.PracticalFullMarks);

                    decimal totalCredit = subjectMarkObj.TheoryCredit + subjectMarkObj.PracticalCredit;
                    decimal finalGradePointFloat = ((theoryGradeAndPoint.GradePoint * subjectMarkObj.TheoryCredit) + (practicalGradeAndPoint.GradePoint * subjectMarkObj.PracticalCredit)) / totalCredit;
                    decimal finalGradePointCalculated = Math.Floor(finalGradePointFloat * 100) / 100;
                    decimal finalGradePoint = GetFinalGradePoint(finalGradePointCalculated);
                    string gradeTheory = calculateGrade(subjectMarkObj.ObtainedTheoryMarks, subjectMarkObj.TheoryFullMarks);
                    string gradePractical = calculateGrade(subjectMarkObj.ObtainedPracticalMarks, subjectMarkObj.PracticalFullMarks);
                    string finalGrade = calculateFinalGrade(finalGradePoint);

                    var existingSubjectMark = existingSubjectMarks
                        .FirstOrDefault(x => x.ClassCourseId == classCourseId);

                    if (existingSubjectMark != null)
                    {
                        existingSubjectMark.FullTheoryMarks = subjectMarkObj.TheoryFullMarks;
                        existingSubjectMark.FullPracticalMarks = subjectMarkObj.PracticalFullMarks;
                        existingSubjectMark.ObtainedTheoryMarks = subjectMarkObj.ObtainedTheoryMarks;
                        existingSubjectMark.ObtainedPracticalMarks = subjectMarkObj.ObtainedPracticalMarks;
                        existingSubjectMark.GradeTheory = gradeTheory;
                        existingSubjectMark.GradePointTheory = theoryGradeAndPoint.GradePoint;
                        existingSubjectMark.GradePractical = gradePractical;
                        existingSubjectMark.GradePointPractical = practicalGradeAndPoint.GradePoint;
                        existingSubjectMark.FinalGrade = finalGrade;
                        existingSubjectMark.FinalGradePoint = finalGradePoint;
                    }
                    else
                    {
                        var newSubjectMark = new SubjectMark
                        {
                            StudentEnrollmentId = studentEnrollmentId,
                            ClassCourseId = classCourseId,
                            ExamResultId = existingExamResult.Id,
                            FullTheoryMarks = subjectMarkObj.TheoryFullMarks,
                            FullPracticalMarks = subjectMarkObj.PracticalFullMarks,
                            ObtainedTheoryMarks = subjectMarkObj.ObtainedTheoryMarks,
                            ObtainedPracticalMarks = subjectMarkObj.ObtainedPracticalMarks,
                            GradeTheory = gradeTheory,
                            GradePointTheory = theoryGradeAndPoint.GradePoint,
                            GradePractical = gradePractical,
                            GradePointPractical = practicalGradeAndPoint.GradePoint,
                            FinalGrade = finalGrade,
                            FinalGradePoint = finalGradePoint
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
            var subjectMarks = await _context.ExamResults.Where(x => x.StudentEnrollmentId == Guid.Parse(studentEnrollmentId) && x.ExamType == examType)
                                                         .Select(s => new SubjectMarksViewModel
                                                         {
                                                             ExamType = s.ExamType,
                                                             Attendance = s.Attendance,
                                                             TotalSchoolDays = s.TotalSchoolDays,
                                                             StudentId = s.StudentEnrollmentId.ToString(),
                                                             StudentMarksLists = s.SubjectMarks.Select(sm => new StudentMarksList
                                                             {
                                                                 ClassCourseId = sm.ClassCourseId.ToString(),
                                                                 ObtainedTheoryMarks = sm.ObtainedTheoryMarks,
                                                                 ObtainedPracticalMarks = sm.ObtainedPracticalMarks,
                                                                 TheoryCredit = sm.ClassCourse.TheoryCreditHour,
                                                                 PracticalFullMarks = sm.FullPracticalMarks,
                                                                 TheoryFullMarks = sm.FullTheoryMarks,
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
                var theoryGradeAndPoint = GetGradeAndPoint(subjectMarkObj.ObtainedTheoryMarks, subjectMarkObj.TheoryFullMarks);
                var practicalGradeAndPoint = GetGradeAndPoint(subjectMarkObj.ObtainedPracticalMarks, subjectMarkObj.PracticalFullMarks);

                decimal totalCredit = subjectMarkObj.TheoryCredit + subjectMarkObj.PracticalCredit;
                decimal finalGradePoint = ((theoryGradeAndPoint.GradePoint * subjectMarkObj.TheoryCredit) + (practicalGradeAndPoint.GradePoint * subjectMarkObj.PracticalCredit)) / totalCredit;

                string gradeTheory = calculateGrade(subjectMarkObj.ObtainedTheoryMarks, subjectMarkObj.TheoryFullMarks);
                string gradePractical = calculateGrade(subjectMarkObj.ObtainedPracticalMarks, subjectMarkObj.PracticalFullMarks);
                string finalGrade = calculateFinalGrade(finalGradePoint);

                weightedGP += (totalCredit * finalGradePoint);
                totalCreditHour += totalCredit;

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
                >= 30 => "D",
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
                >= 1.2m => "D",
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
                >= 30 => ("D", 1.2m),
                >= 0 => ("E", 0.8m),  // Fail
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

        public async Task<ResultViewModel> GetResult(Guid studentEnrollmentId, CancellationToken cancellationToken)
        {
            var result = await _context.ExamResults.Where(x => x.StudentEnrollmentId == studentEnrollmentId)
                                                   .Select(x => new ResultViewModel
                                                   {
                                                       ExamType = x.ExamType,
                                                       TotalCredit = x.TotalCredit,
                                                       GPA = x.GPA,
                                                       StudentName = x.StudentEnrollment.Student.FirstName + ' ' + x.StudentEnrollment.Student.LastName,
                                                       FatherName = x.StudentEnrollment.Student.FatherName,
                                                       MotherName = x.StudentEnrollment.Student.MotherName,
                                                       ClassRoom = x.StudentEnrollment.ClassSection.ClassRoom.Name,
                                                       Section = x.StudentEnrollment.ClassSection.Section.Name,
                                                       RollNo = (int)(x.StudentEnrollment.RollNumber != null ? x.StudentEnrollment.RollNumber : 0),
                                                       WardNo = x.StudentEnrollment.Student.WardNo,
                                                       Attendance = x.Attendance,
                                                       TotalSchoolDays = x.TotalSchoolDays,
                                                       StudentMarks = x.SubjectMarks.Where(x => x.ClassCourseId == x.ClassCourseId)
                                                                                    .Select(s => new StudentMarksViewModel
                                                                                    {
                                                                                        CourseName = s.ClassCourse.Course.Name,
                                                                                        CreditHour = s.ClassCourse.TheoryCreditHour + s.ClassCourse.PracticalCreditHour,
                                                                                        FinalGrade = s.FinalGrade,
                                                                                        FinalGradePoint = s.FinalGradePoint,
                                                                                        GradePractical = s.GradePractical,
                                                                                        GradeTheory = s.GradeTheory
                                                                                    }).ToList(),
                                                       IssueDate = DateTime.UtcNow
                                                   }).FirstOrDefaultAsync();
            return result;
        }
    }
}
