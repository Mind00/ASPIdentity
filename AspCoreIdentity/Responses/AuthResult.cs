using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Responses
{
    public class AuthResult
    {
        public string  token { get; set; }
        public string refreshToken { get; set; }
        public bool success { get; set; }
        public List<string> Errors { get; set; }
    }
}
