using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Models
{
    public class RefreshToken
    {
        
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsUserd { get; set; }  //to make sure that token is used only once
        public bool IsRevoked { get; set; } //to make sure that they are valid
        public DateTime ExpiryDate { get; set; }
        public DateTime AddedDated { get; set; }
        public DateTime UpdatedDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser ApplicationUsers { get; set; }
    }
}
