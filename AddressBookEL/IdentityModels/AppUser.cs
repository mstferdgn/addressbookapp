using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.IdentityModels
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        [Required]
        [StringLength(150)]
        public string Surname { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }
        public string? ProfilePicture { get; set; }
    }

    public enum Gender
    {
        YOK=0,
        KADIN = 1,
        ERKEK = 2
    }
}
