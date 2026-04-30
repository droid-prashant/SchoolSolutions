using Application.Students.Dtos;
using Application.Students.Interfaces;
using Application.Students.ViewModels;
using Domain.Constants;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [Route("GetStudent")]
        [HasPermission(PermissionNames.StudentView)]
        public async Task<List<StudentViewModel>> Get(CancellationToken cancellationToken)
        {
            var result = await _studentService.GetStudentAsync(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetStudentCertificateData")]
        [HasPermission(PermissionNames.StudentView)]
        public async Task<List<StudentCertificateViewModel>> GetStudentCertificateData([FromQuery] string classSectionId, CancellationToken cancellationToken)
        {
            var result = await _studentService.GetStudentCertificateDataAsync(classSectionId, cancellationToken);
            return result;
        }

        [HttpGet()]
        [Route("GetStudentByClassId")]
        [HasPermission(PermissionNames.StudentView, PermissionNames.ExamMarksEntry, PermissionNames.FeeView)]
        public async Task<List<StudentViewModel>> GetStudentByClassId([FromQuery] string classId, CancellationToken cancellationToken)
        {
            var classRoomId = Guid.Parse(classId);
            var result = await _studentService.GetStudentByClassIdAsync(classRoomId, cancellationToken);
            return result;
        }

        [HttpGet()]
        [Route("GetStudentByClassSectionId")]
        [HasPermission(PermissionNames.StudentView, PermissionNames.ExamMarksEntry, PermissionNames.FeeView)]
        public async Task<List<StudentViewModel>> GetStudentByClassSectionId([FromQuery] string classSectionId, [FromQuery] int? examType, CancellationToken cancellationToken)
        {
            var result = await _studentService.GetStudentByClassSectionId(classSectionId, examType, cancellationToken);
            return result;
        }

        [HttpPost]
        [Route("AddStudent")]
        [HasPermission(PermissionNames.StudentCreate)]
        public async Task AddStudent([FromBody] StudentDto addStudent, CancellationToken cancellationToken)
        {
            await _studentService.AddStudentAsync(addStudent, cancellationToken);
        }

        [HttpPost]
        [Route("AssignRegistrationAndSymbolNumber")]
        [HasPermission(PermissionNames.StudentUpdate)]
        public async Task AssignRegistrationAndSymbolNumber([FromBody] StudentEnrollmentDto studentEnrollmentDto, [FromQuery] string studentEnrollmentId, CancellationToken cancellationToken)
        {
            await _studentService.AssignRegistrationAndSymbolNumber(studentEnrollmentDto, studentEnrollmentId, cancellationToken);
        }

        [HttpPut]
        [Route("UpdateStudent")]
        [HasPermission(PermissionNames.StudentUpdate)]
        public async Task UpdateStudent([FromBody] StudentDto addStudent, CancellationToken cancellationToken)
        {
            await _studentService.UpdateStudentAsync(addStudent, cancellationToken);
        }

        [HttpPost]
        [Route("AssignRollNumber")]
        [HasPermission(PermissionNames.StudentUpdate)]
        public async Task AssignRollNumbersAsync([FromQuery] string classSectionId, CancellationToken cancellationToken)
        {
            await _studentService.AssignRollNumbersAsync(classSectionId, cancellationToken);
        }

        [HttpGet]
        [Route("GetRegAndSymCompliantEnrolledStudents")]
        [HasPermission(PermissionNames.StudentView)]
        public async Task<List<StudentEnrollmentViewModel>> GetRegAndSymCompliantEnrolledStudents([FromQuery] string classSectionId, CancellationToken cancellationToken)
        {
            var result = await _studentService.GetRegAndSymCompliantEnrolledStudents(classSectionId, cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetPromotionCandidates")]
        [HasPermission(PermissionNames.StudentView)]
        public async Task<List<PromotionCandidateViewModel>> GetPromotionCandidates([FromQuery] string classSectionId, [FromQuery] int examType, CancellationToken cancellationToken)
        {
            return await _studentService.GetPromotionCandidates(classSectionId, examType, cancellationToken);
        }

        [HttpPost]
        [Route("PromoteStudents")]
        [HasPermission(PermissionNames.StudentUpdate)]
        public async Task<PromotionExecutionResultViewModel> PromoteStudents([FromBody] PromoteStudentsDto request, CancellationToken cancellationToken)
        {
            return await _studentService.PromoteStudentsAsync(request, cancellationToken);
        }

        [HttpPost]
        [Route("SustainStudents")]
        [HasPermission(PermissionNames.StudentUpdate)]
        public async Task<PromotionExecutionResultViewModel> SustainStudents([FromBody] PromoteStudentsDto request, CancellationToken cancellationToken)
        {
            return await _studentService.SustainStudentsAsync(request, cancellationToken);
        }

        [HttpPost]
        [Route("ManuallyPromoteStudents")]
        [HasPermission(PermissionNames.StudentUpdate)]
        public async Task<PromotionExecutionResultViewModel> ManuallyPromoteStudents([FromBody] PromoteStudentsDto request, CancellationToken cancellationToken)
        {
            return await _studentService.ManuallyPromoteStudentsAsync(request, cancellationToken);
        }

        [HttpPost]
        [Route("AddStudentCertificateLog")]
        [HasPermission(PermissionNames.StudentUpdate)]
        public async Task AddStudentCertificateLog([FromBody] StudentCertificateDto studentCertificateDto, CancellationToken cancellationToken)
        {
            await _studentService.AddStudentCertificateLog(studentCertificateDto, cancellationToken);
        }

        [HttpGet]
        [Route("GetStudentCertificateLog")]
        [HasPermission(PermissionNames.StudentView)]
        public async Task<StudentCertificateLogViewModel> GetStudentCertificateLog([FromQuery] CertificateType certificateType, CancellationToken cancellationToken)
        {
            var result = await _studentService.GetStudentCertificateLog(certificateType, cancellationToken);
            return result;
        }
    }
}
