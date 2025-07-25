using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.Entities
{
    public interface INameColumn
    {
        [Required]
        [StringLength(100)]
        public abstract string Name { get; set; }
    }
}
