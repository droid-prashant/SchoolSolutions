using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Students.Dtos;
using Application.Students.ViewModels;
using Domain.Enums;

namespace Application.Students.Interfaces
{
    public interface IStudentService
    {
        Task AddStudentAsync(StudentDto addStudent, CancellationToken cancellationToken);
        Task AssignRegistrationAndSymbolNumber(StudentEnrollmentDto studentEnrollmentDto, string studentEnrollmentId, CancellationToken cancellationToken);
        Task UpdateStudentAsync(StudentDto addStudent, CancellationToken cancellationToken);
        Task AssignRollNumbersAsync(string classSectionId, CancellationToken cancellationToken);
        Task AddStudentCertificateLog(StudentCertificateDto studentCertificateDto, CancellationToken cancellationToken);
        Task<StudentCertificateLogViewModel> GetStudentCertificateLog(CertificateType certificateType, CancellationToken cancellationToken);
        Task<List<StudentViewModel>> GetStudentAsync(CancellationToken cancellationToken);
        Task<List<StudentCertificateViewModel>> GetStudentCertificateDataAsync(string classSectionId, CancellationToken cancellationToken);
        Task<List<StudentViewModel>> GetStudentByClassIdAsync(Guid classRommId, CancellationToken cancellationToken);
        Task<List<StudentViewModel>> GetStudentByClassSectionId(string classSectionId, int? examType, CancellationToken cancellationToken);
        Task<List<StudentEnrollmentViewModel>> GetRegAndSymCompliantEnrolledStudents(string classSectionId, CancellationToken cancellationToken);
    }
}
