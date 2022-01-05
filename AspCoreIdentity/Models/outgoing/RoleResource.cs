using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Models.outgoing
{
    public class RoleResource
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string CreatedBy { get; set; }
    }
}
