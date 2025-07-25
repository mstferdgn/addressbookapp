using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.Entities
{
    [Table("CITY")]
    public class City : IBaseEntity<byte>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte Id { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(2,MinimumLength =2)] 
        public string PlateCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}
