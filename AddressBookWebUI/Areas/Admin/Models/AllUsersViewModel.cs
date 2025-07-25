using AddressBookEL.IdentityModels;
using System.ComponentModel.DataAnnotations;

namespace AddressBookWebUI.Areas.Admin.Models
{
    public class AllUsersViewModel
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
        public string? RolesStatus { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
