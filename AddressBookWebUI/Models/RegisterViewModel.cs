using AddressBookEL.IdentityModels;
using System.ComponentModel.DataAnnotations;

namespace AddressBookWebUI.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="İsim alanı gereklidir!")]
        [StringLength(150)]
        public string Name { get; set; }
        [Required]
        [StringLength(150)]
        public string Surname { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        [Compare("Password",ErrorMessage ="Şifreler uyuşmuyor!")]
        public string ConfirmPassword { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? Birthdate { get; set; }
        
    }
}
