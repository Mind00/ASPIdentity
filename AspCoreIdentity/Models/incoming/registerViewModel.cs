using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Models
{
    public class registerViewModel
    {
        public string id { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string fullname { get; set; }
        public string passwordHash { get; set; }
    }
}
