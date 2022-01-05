using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Models.incoming
{
    public class FamilyMemberViewModel
    {
        public string FName { get; set; }
        public int Age { get; set; }
        public string Relation { get; set; }
        public DateTime AddedTo { get; set; }
        public string AddedBy { get; set; }
        public IFormFileCollection PhotoGalleries { get; set; }
        public List<PhotoGalleryViewModel> PhotoGallery { get; set; }
    }
}
