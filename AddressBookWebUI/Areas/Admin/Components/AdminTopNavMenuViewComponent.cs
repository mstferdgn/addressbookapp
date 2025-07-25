using AddressBookEL.AllEnums;
using AddressBookEL.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AddressBookWebUI.Areas.Admin.Components
{
    public class AdminTopNavMenuViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminTopNavMenuViewComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            try
            {
                //giriş yapmış kullanıcı var mı?
                var username = User.Identity?.Name;
                if (username==null)
                {
                    return View(new AppUser());
                }
                var user = _userManager.FindByNameAsync(username).Result;
                if (user == null) 
                {
                    return View(new AppUser());
                }

                //giriş yapan Admin rolünde midir
                if(!_userManager.IsInRoleAsync(user, Roles.ADMIN.ToString()).Result)
                {
                    return View(new AppUser());
                }

                return View(user); // Default.cshtml
                               
            }
            catch (Exception ex)
            {
                // ex loglansın
                return View(new AppUser()); // Default.cshtml
            }
        }
    }
}
