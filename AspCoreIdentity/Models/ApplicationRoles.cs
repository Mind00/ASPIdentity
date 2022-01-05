using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Models
{
    public class ApplicationRoles : IdentityRole
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}
