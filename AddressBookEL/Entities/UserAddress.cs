using AddressBookEL.IdentityModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.Entities
{
    [Table("USERADRESS")]
    public class UserAddress : IBaseEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(100)]
        public string AddressTitle { get; set; }
        public string UserId { get; set; }
        public byte CityId { get; set; }
        public int DistrictId { get; set; }
        public int NeigborhoodId { get; set; }

        [Required]
        [StringLength(500)]
        public string FullAddress { get; set; }

        [MaxLength(5)] //34700
        [MinLength(5)]
        public string? PostalCode { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        [ForeignKey("DistrictId")]

        public virtual District District { get; set; }
        [ForeignKey("NeigborhoodId")]

        public virtual Neigborhood Neigborhood { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }
    }
}
