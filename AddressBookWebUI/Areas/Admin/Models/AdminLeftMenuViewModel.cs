using AddressBookEL.IdentityModels;

namespace AddressBookWebUI.Areas.Admin.Models
{
    public class AdminLeftMenuViewModel
    {
        public AppUser? User { get; set; }
        public int TotalUserCount { get; set; }
        public int TotalAddressCount { get; set; }
    }
}
