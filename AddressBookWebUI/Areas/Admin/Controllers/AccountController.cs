using AddressBookBL.EmailSenderProcess;
using AddressBookEL.AllEnums;
using AddressBookEL.IdentityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AddressBookWebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IEmailManager _emailManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IEmailManager emailManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailManager = emailManager;
            _signInManager = signInManager;
        }
        public bool IsAdminLoggedIn()
        {
            try
            {
                //giriş yapmış kullanıcı var mı?
                var username = User.Identity?.Name;
                if (username == null)
                {
                    return false;
                }
                var user = _userManager.FindByNameAsync(username).Result;
                if (user == null)
                {
                    return false;
                }

                //giriş yapan Admin rolünde midir
                if (!_userManager.IsInRoleAsync(user, Roles.ADMIN.ToString()).Result)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetUserRole()
        {
            try
            {
                //giriş yapmış kullanıcı var mı?
                var username = User.Identity?.Name;
                if (username == null)
                {
                    return string.Empty;
                }
                var user = _userManager.FindByNameAsync(username).Result;
                if (user == null)
                {
                    return string.Empty;
                }

                //giriş yapan Admin rolünde midir
                if (_userManager.IsInRoleAsync(user, Roles.ADMIN.ToString()).Result)
                {
                    return Roles.ADMIN.ToString();
                }
                else if ((_userManager.IsInRoleAsync(user, Roles.MEMBER.ToString()).Result))
                {
                    return Roles.MEMBER.ToString();
                }
                else if ((_userManager.IsInRoleAsync(user, Roles.WAITforVALIDATION.ToString()).Result))
                {
                    return Roles.WAITforVALIDATION.ToString();
                }
            
            
                return string.Empty;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IActionResult Login()
        {
            try
            {
                if (IsAdminLoggedIn())
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else if(GetUserRole()==Roles.MEMBER.ToString())
                {
                    return View("AccessDenied", "Rolünüz ADMIN olmadığı için bu sayfaya gitmeye yetkinz yokk.");// farklı bir view return edilebilir.
                }
                
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Login(string? usernameOrEmail, string? password)
        {
            try
            {
                if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
                {
                    ViewBag.LoginFailedMsg= "Email ya da kullanıcı adı ve şifre alanlarını giriniz!";
                    return View();
                }
                //emailorUsername gelen parametreye ait kullanııc var mı?
                var user = _userManager.FindByEmailAsync(usernameOrEmail).Result;
                if (user == null)
                {
                    user = _userManager.FindByNameAsync(usernameOrEmail).Result;
                }
                if (user == null)
                {
                    ViewBag.LoginFailedMsg = "Lütfen sisteme kayıtlı email ya da kullanıcı adınızla giriş yapınız! ";
                    return View();
                }

                if (!_userManager.IsInRoleAsync(user, Roles.ADMIN.ToString()).Result)
                {
                    ViewBag.LoginFailedMsg = "Admin rolünde değilsiniz! ";
                    return View();
                }
                //şifre kontrolü
                var signInResult = _signInManager.PasswordSignInAsync(user, password, true, false).Result;
                if (!signInResult.Succeeded)
                {
                    ViewBag.LoginFailedMsg = "Email / kullanıcı adı ve şifrenizi doğru girdiğinize emin olunuz!";
                    return View();
                }

                return RedirectToAction("Index","Home",new {area="Admin"});

            }
            catch (Exception ex)
            {
                ViewBag.LoginFailedMsg = "Beklenmedik hata oluştu!";
                return View();
            }
        }

        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account", new { area = "Admin" });

            }
            catch (Exception ex)
            {
                //NOT: buraya daha sonra dönelim
                return RedirectToAction("Login", "Account", new { area = "Admin" });

            }
        }
    }
}
