using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Models
{
    public class PhotoGallery
    {
        public int Id { get; set; }
        public int FId { get; set; }
        public string Name { get; set; }
        public string UrlPath { get; set; }

        public FamilyMember FamilyMember { get; set; }
    }
}
