using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum CertificateType
    {
        [Display(Name = "Character Certificate")]
        CharacterCertificate = 1,

        [Display(Name = "Transfer Certificate")]
        TransferCertificate = 2,

    }
}
