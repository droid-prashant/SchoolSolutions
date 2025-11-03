using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShortName { get; set; }
        public string? Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }

        public ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }
        public ICollection<IdentityUserClaim<Guid>> UserClaims { get; set; }
    }
}
