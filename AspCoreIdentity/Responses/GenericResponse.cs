using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Responses
{
    public class GenericResponse
    {
        public bool success { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
        public List<string> Errors { get; set; }
    }
}
