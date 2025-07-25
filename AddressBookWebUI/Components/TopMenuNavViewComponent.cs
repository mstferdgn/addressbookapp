using AddressBookEL.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AddressBookWebUI.Components
{
    public class TopMenuNavViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        public TopMenuNavViewComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            //giriş yapmış olan kişinin bilgileri
            var username = User.Identity?.Name;
            if (username == null) // bu ife asla girmez çünkü zaten authorize var :D
            {
                return View(null); // burada Default.cshtml'e gider
                //return View("TopMenu");
            }

            var user = _userManager.FindByNameAsync(username).Result;
            return View(user); // burada Default.cshtml'e gider
            //return View("TopMenu",user); // burada TopMenu.cshtml'e gider
        }
    }
}
