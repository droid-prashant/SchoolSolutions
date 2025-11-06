using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Students.Dtos
{
    public class StudentCertificateDto
    {
        public string StudentEnrollmentId { get; set; }
        public int CertificateNumber { get; set; }
        public CertificateType certificateType { get; set; }
    }
}
