using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Models
{
    public class FamilyMember
    {
        [Key]
        public int FId { get; set; }
        public string FName { get; set; }
        public int Age { get; set; }
        public string Relation { get; set; }
        public DateTime AddedTo{ get; set; }
        public string AddedBy { get; set; }
        public ICollection<PhotoGallery> PhotoGallery { get; set; }
    }
}
