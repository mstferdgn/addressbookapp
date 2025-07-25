using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.ViewModels
{
    public class CityDTO
    {
        public byte Id { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string PlateCode { get; set; }
    }
}
