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
                    TotalCredit = gpaAndTotalCreaditHour.TotalCreditHour
                };

                await _context.ExamResults.AddAsync(studentResult);
                await _context.SaveChangesAsync(cancellationToken);

                foreach (var subjectMarkObj in subjectMarkList)
                {
                    var theoryGradeAndPoint = GetGradeAndPoint(subjectMarkObj.ObtainedTheoryMarks, subjectMarkObj.TheoryFullMarks);
                    var practicalGradeAndPoint = GetGradeAndPoint(subjectMarkObj.ObtainedPracticalMarks, subjectMarkObj.PracticalFullMarks);

                    double totalCredit = subjectMarkObj.TheoryCredit + subjectMarkObj.PracticalCredit;
                    double finalGradePoint = ((theoryGradeAndPoint.GradePoint * subjectMarkObj.TheoryCredit) + (practicalGradeAndPoint.GradePoint * subjectMarkObj.PracticalCredit)) / totalCredit;

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

        private (double GPA, double TotalCreditHour) calculateGPAAndTotalCreditHour(List<StudentMarksList> subjectMarkList)
        {
            double weightedGP = 0.00;
            double totalCreditHour = 0.00;
            foreach (var subjectMarkObj in subjectMarkList)
            {
                var theoryGradeAndPoint = GetGradeAndPoint(subjectMarkObj.ObtainedTheoryMarks, subjectMarkObj.TheoryFullMarks);
                var practicalGradeAndPoint = GetGradeAndPoint(subjectMarkObj.ObtainedPracticalMarks, subjectMarkObj.PracticalFullMarks);

                double totalCredit = subjectMarkObj.TheoryCredit + subjectMarkObj.PracticalCredit;
                double finalGradePoint = ((theoryGradeAndPoint.GradePoint * subjectMarkObj.TheoryCredit) + (practicalGradeAndPoint.GradePoint * subjectMarkObj.PracticalCredit)) / totalCredit;

                string gradeTheory = calculateGrade(subjectMarkObj.ObtainedTheoryMarks, subjectMarkObj.TheoryFullMarks);
                string gradePractical = calculateGrade(subjectMarkObj.ObtainedPracticalMarks, subjectMarkObj.PracticalFullMarks);
                string finalGrade = calculateFinalGrade(finalGradePoint);

                weightedGP += (totalCredit * finalGradePoint);
                totalCreditHour += totalCredit;

            }
            var gpa = weightedGP / totalCreditHour;
            return (gpa, totalCreditHour);
        }

        private string calculateGrade(double obtainedMarks, double fullMarks)
        {
            double percent = (obtainedMarks / fullMarks) * 100;
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

        private string calculateFinalGrade(double gradePoint)
        {
            var grade = gradePoint switch
            {
                >= 4.0 => "A+",
                >= 3.6 => "A",
                >= 3.2 => "B+",
                >= 2.8 => "B",
                >= 2.4 => "C+",
                >= 2.0 => "C",
                >= 1.6 => "D+",
                >= 1.2 => "D",
                >= 0.8 => "NQ",   // Fail
                _ => "N/A"
            };
            return grade;
        }

        private (string Grade, double GradePoint) GetGradeAndPoint(double obtainedMarks, double fullMarks)
        {
            double percent = (obtainedMarks / fullMarks) * 100;

            return percent switch
            {
                >= 90 => ("A+", 4.0),
                >= 80 => ("A", 3.6),
                >= 70 => ("B+", 3.2),
                >= 60 => ("B", 2.8),
                >= 50 => ("C+", 2.4),
                >= 40 => ("C", 2.0),
                >= 35 => ("D+", 1.6),
                >= 30 => ("D", 1.2),
                >= 0 => ("E", 0.8),  // Fail
                _ => ("N/A", 0.0)  // Invalid safeguard
            };
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
                                                       StudentMarks = x.SubjectMarks.Where(x => x.ClassCourseId == x.ClassCourseId)
                                                                                    .Select(s => new StudentMarksViewModel
                                                                                    {
                                                                                        CourseName = s.ClassCourse.Course.Name,
                                                                                        CreditHour = s.ClassCourse.TheoryCreditHour + s.ClassCourse.PracticalCreditHour,
                                                                                        FinalGrade = s.FinalGrade,
                                                                                        FinalGradePoint = s.FinalGradePoint,
                                                                                        GradePractical = s.GradePractical,
                                                                                        GradeTheory = s.GradeTheory
                                                                                    }).ToList()
                                                   }).FirstOrDefaultAsync();
            return result;
        }
    }
}
