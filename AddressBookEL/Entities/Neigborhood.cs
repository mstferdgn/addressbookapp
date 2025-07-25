using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.Entities
{
    [Table("NEIGBORHOOD")]
    public class Neigborhood : IBaseEntity<int>, INameColumn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public byte CityId { get; set; }
        public int DistrictId { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        [ForeignKey("DistrictId")]
        public virtual District District { get; set; }

    }
}
