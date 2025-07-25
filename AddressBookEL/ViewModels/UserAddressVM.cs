using AddressBookEL.Entities;
using AddressBookEL.IdentityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.ViewModels
{
    public class UserAddressVM
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
        public byte CityId { get; set; }
        public int DistrictId { get; set; }
        public int NeigborhoodId { get; set; }
        [Required]
        [StringLength(100)]
        public string AddressTitle { get; set; }

        [Required]
        [StringLength(500)]
        public string FullAddress { get; set; }

        [MaxLength(5)] //34700
        [MinLength(5)]
        public string? PostalCode { get; set; }
        public bool IsDeleted { get; set; }

        public  CityDTO? City { get; set; }

        public  DistrictDTO? District { get; set; }

        public  NeigborhoodVM? Neigborhood { get; set; }

        public AppUser? User { get; set; }
        public string? AddressEditSuccessMsg { get; set; } // ViewBag kullanmak şart değildir! Başarılı mesajı sayfanın modelindeki property de atabilirdik!
    }
}
