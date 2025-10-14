using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Students.Dtos;
using Application.Students.ViewModels;

namespace Application.Students.Interfaces
{
    public interface IStudentService
    {
        Task AddStudentAsync(StudentDto addStudent, CancellationToken cancellationToken);
        Task<List<StudentViewModel>> GetStudentAsync(CancellationToken cancellationToken);
        Task<List<StudentViewModel>> GetStudentByClassIdAsync(Guid classRommId, CancellationToken cancellationToken);
    }
}
