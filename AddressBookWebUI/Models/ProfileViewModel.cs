using AddressBookEL.IdentityModels;
using System.ComponentModel.DataAnnotations;

namespace AddressBookWebUI.Models
{
    public class ProfileViewModel
    {

        [Required(ErrorMessage = "İsim alanı gereklidir!")]
        [StringLength(150)]
        public string Name { get; set; }
        [Required]
        [StringLength(150)]
        public string Surname { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Username { get; set; }
        public string? NewUsername { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? Birthdate { get; set; }

        public string? ProfilePicture { get; set; }
     //   HttpPostedFileBase--> (.net framework aspnet mvc) de bunu kullanıyoruz
        public IFormFile? PictureFile { get; set; }
    }
}
