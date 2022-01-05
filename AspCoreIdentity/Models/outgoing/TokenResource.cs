using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Models.outgoing
{
    public class TokenResource
    {
        public string jwtToken { get; set; }
        public string refreshToken { get; set; }
    }
}
