using AddressBookEL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.ViewModels
{
    public class NeigborhoodVM
    {
        public int Id { get; set; }
        public byte CityId { get; set; }
        public int DistrictId { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public CityDTO? City { get; set; }
        public DistrictDTO? District { get; set; }
    }
}
