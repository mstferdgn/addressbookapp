using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.ViewModels
{
    public class DistrictDTO
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte CityId { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public CityDTO? City { get; set; }
    }
}
