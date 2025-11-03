using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Constants
{
    public enum UserTypeEnum
    {
        [Description("Super Admin")] SuperAdmin = 1,
        [Description("User Level")] UserLevel = 10
    }
}