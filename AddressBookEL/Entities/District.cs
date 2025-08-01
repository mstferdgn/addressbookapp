﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.Entities
{
    //tablo adını contextte verebiliriz
    public class District : IBaseEntity<int>, 
        INameColumn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte CityId { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }
    }
}
