using AddressBookBL.InterfacesOfManagers;
using AddressBookEL.AllEnums;
using AddressBookEL.IdentityModels;
using AddressBookWebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AddressBookWebUI.Areas.Admin.Components
{
   
    public class AdminLeftSideBarMenuViewComponent:ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserAddressManager    _userAddressManager;

        public AdminLeftSideBarMenuViewComponent(UserManager<AppUser> userManager, IUserAddressManager userAddressManager)
        {
            _userManager = userManager;
            _userAddressManager = userAddressManager;
        }

        public IViewComponentResult Invoke()
        {

            //giriş yapmış kullanıcı var mı?
            var username = User.Identity?.Name;
            if (username == null)
            {
                return View(); //burada Default.cshtml çağırır
            }
            var user = _userManager.FindByNameAsync(username).Result;
            if (user == null)
            {
                return View();  //burada Default.cshtml çağırır
            }

            //giriş yapan Admin rolünde midir
            if (!_userManager.IsInRoleAsync(user, Roles.ADMIN.ToString()).Result)
            {
                return View();  //burada Default.cshtml çağırır
            }

            //#region Yontem 1
            //TempData["TotalUserCount"] = _userManager.Users.Count();
            //TempData["TotalAddressCount"] = _userAddressManager.GetAll().Data.Count();
            //return View("AdminLeftMenu", user); //AdminLeftMenu.cshtml 
            //#endregion



            #region Yontem 2
            AdminLeftMenuViewModel model = new AdminLeftMenuViewModel()
            {
                User = user,
                TotalAddressCount = _userAddressManager.GetAll().Data.Count(),
                TotalUserCount = _userManager.Users.Count()
            };
            return View("AdminLeftMenu2", model); //AdminLeftMenu.cshtml  
            #endregion
        }
    }
}
