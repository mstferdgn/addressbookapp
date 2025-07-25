using AddressBookEL.IdentityModels;
using AddressBookWebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AddressBookEL.AllEnums;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using AddressBookBL.EmailSenderProcess;
using ClosedXML;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Spreadsheet;
using AddressBookBL.ImplementationofManagers;
using AddressBookBL.InterfacesOfManagers;
using Serilog;
using AddressBookEL.ViewModels;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AddressBookWebUI.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IEmailManager _emailManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILoggerManager _loggerManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IEmailManager emailManager, SignInManager<AppUser> signInManager, ILoggerManager loggerManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailManager = emailManager;
            _signInManager = signInManager;
            _loggerManager = loggerManager;
            _logger = logger;
        }

        public IActionResult Login()
        {
            _logger.LogInformation("Login sayfası çağrıldı!");
            _loggerManager.LogMessage(LoggerLevel.Info, "Login sayfası çağrıldı!");
            //giriş yapmış olan kişi varsa bu sayfayı çağıramaz!
            var username = User.Identity?.Name;
            if (username != null) // bu ife asla girmez çünkü zaten authorize var :D
            {
                return RedirectToAction("Address", "Home");
            }

            return View();
        }
        public IActionResult Login2()
        {
            //giriş yapmış olan kişi varsa bu sayfayı çağıramaz!
            var username = User.Identity?.Name;
            if (username != null) // bu ife asla girmez çünkü zaten authorize var :D
            {
                return RedirectToAction("Address", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login2(string? emailorUsername, string? password)
        {
            try
            {
                if (string.IsNullOrEmpty(emailorUsername) || string.IsNullOrEmpty(password))
                {
                    ModelState.AddModelError("", "Email ya da kullanıcı adı ve şifre alanlarını giriniz!");
                    return View("Login2");
                }
                //emailorUsername gelen parametreye ait kullanııc var mı?
                var user = _userManager.FindByEmailAsync(emailorUsername).Result;
                if (user == null)
                {
                    user = _userManager.FindByNameAsync(emailorUsername).Result;
                }
                if (user == null)
                {
                    ModelState.AddModelError("", "Lütfen sisteme kayıtlı email ya da kullanıcı adınızla giriş yapınız! ");
                    return View("Login2");
                }

                //Role bakmamız
                #region Yontem 2
                if (_userManager.IsInRoleAsync(user, Roles.WAITforVALIDATION.ToString()).Result)
                {
                    ModelState.AddModelError("", "DİKKAT! Sisteme giriş yapabilmek için emailinize gelen aktivasyon linki ile hesabınızı aktive etmelisiniz !  ");
                    return View("Login2");
                }
                else if (!_userManager.IsInRoleAsync(user, Roles.MEMBER.ToString()).Result &&
                    !_userManager.IsInRoleAsync(user, Roles.ADMIN.ToString()).Result)
                {
                    ModelState.AddModelError("", "DİKKAT! Rol tanımlanız olmadığı için sisteme giriş yapamazsınız!  ");
                    return View("Login2");
                }
                //şifre kontrolü
                var signInResult = _signInManager.PasswordSignInAsync(user, password, true, false).Result;
                if (!signInResult.Succeeded)
                {
                    ModelState.AddModelError("", "Email / kullanıcı adı ve şifrenizi doğru girdiğinize emin olunuz!");
                    return View("Login2");
                }

                else if (_userManager.IsInRoleAsync(user, Roles.ADMIN.ToString()).Result)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (_userManager.IsInRoleAsync(user, Roles.MEMBER.ToString()).Result)
                {
                    return RedirectToAction("Address", "Home");
                }
                else if (_userManager.IsInRoleAsync(user, Roles.DELETED.ToString()).Result)
                {
                    return RedirectToAction("DeletedUsertoActive", "Account");
                }
                //assignment : Eğer bu kişi sistemde deleted olmuşsa rolü deleted şeklindedir ozaman giriş yapmaya çalıştığında farklı bir sayfaya yönlendirilip o sayfada "Beni tekrar AKtif yap" butonu olsun ve o butona tıklarsa rolü deleted --> member

                #endregion



                return View("Login2");

            }

            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Beklenmedik hata oluştu!");
                return View("Login2");
                //ex'i loglayacağız
            }
        }



        [HttpPost]
        public IActionResult Login(string? emailorUsername, string? password)
        {
            try
            {
                if (string.IsNullOrEmpty(emailorUsername) || string.IsNullOrEmpty(password))
                {
                    ModelState.AddModelError("", "Email ya da kullanıcı adı ve şifre alanlarını giriniz!");
                    return View("Login");
                }
                //emailorUsername gelen parametreye ait kullanııc var mı?
                var user = _userManager.FindByEmailAsync(emailorUsername).Result;
                if (user == null)
                {
                    user = _userManager.FindByNameAsync(emailorUsername).Result;
                }
                if (user == null)
                {
                    ModelState.AddModelError("", "Lütfen sisteme kayıtlı email ya da kullanıcı adınızla giriş yapınız! ");
                    return View("Login");
                }

                //Role bakmamız
                #region Yontem 1
                IList<string> roles = _userManager.GetRolesAsync(user).Result;
                if (roles.Count == 0)
                {
                    ModelState.AddModelError("", "DİKKAT! Rol tanımlanız olmadığı için sisteme giriş yapamazsınız!  ");
                    return View("Login");
                }

                else if (roles.Contains(Roles.WAITforVALIDATION.ToString()))
                {
                    ModelState.AddModelError("", "DİKKAT! Sisteme giriş yapabilmek için emailinize gelen aktivasyon linki ile hesabınızı aktive etmelisiniz !  ");
                    return View("Login");
                }






                var signInResult = _signInManager.PasswordSignInAsync(user, password, true, false).Result;
                if (!signInResult.Succeeded)
                {
                    ModelState.AddModelError("", "Email / kullanıcı adı ve şifrenizi doğru girdiğinize emin olunuz!");
                    return View("Login");
                }

                else if (roles.Contains(Roles.ADMIN.ToString()))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else if (roles.Contains(Roles.MEMBER.ToString()))
                {

                    _loggerManager.LogMessage(LoggerLevel.Info, $"{user.Email} kullanıcısı sisteme giriş yaptı");

                    return RedirectToAction("Address", "Home");
                }
                else if (roles.Contains(Roles.DELETED.ToString()))
                {

                    _loggerManager.LogMessage(LoggerLevel.Info, $"{user.Email} (deleted)kullanıcısı sisteme giriş yaptı");

                    return RedirectToAction("DeletedUsertoActive", "Account");
                }

                #endregion

                //#region Yontem 2
                //if (_userManager.IsInRoleAsync(user, Roles.WAITforVALIDATION.ToString()).Result)
                //{
                //	ModelState.AddModelError("", "DİKKAT! Sisteme giriş yapabilmek için emailinize gelen aktivasyon linki ile hesabınızı aktive etmelisiniz !  ");
                //	return View(emailorUsername);
                //}

                //#endregion





                //şifre kontrolü




                return View("Login");

            }

            catch (Exception ex)
            {
                _logger.LogError($"HATA: Account/Login httpost. parametreler: emailorUsername:{emailorUsername} - password:{password} ex:{ex}");
                ModelState.AddModelError("", $"Beklenmedik hata oluştu!");
                return View();
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                _logger.LogInformation("Çıkış yapıldı");
                _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                //NOT: buraya daha sonra dönelim
                return RedirectToAction("Login", "Account");

            }
        }


        public IActionResult Register()
        {
            //giriş yapmış olan kişi varsa bu sayfayı çağıramaz!
            var username = User.Identity?.Name;
            if (username != null) // bu ife asla girmez çünkü zaten authorize var :D
            {
                return RedirectToAction("Address", "Home");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Girişleri düzgün yapınız!");
                    return View(model);
                }

                //Aynı usernameden var mı??

                var sameUserName = _userManager.FindByNameAsync(model.Username).Result;

                if (sameUserName != null)
                {
                    ModelState.AddModelError("", $"{model.Username} adlı bir kullanıcı adı zaten sistemde kayıtlıdır!");
                    return View(model);
                }

                //burayı program.cs içindeki                 options.User.RequireUniqueEmail = true; ile yaptık.

                ////Aynı emailden var mı?
                //var sameEmail = _userManager.FindByEmailAsync(model.Email).Result;

                //if (sameEmail != null)
                //{
                //    ModelState.AddModelError("", $"{model.Email} emaili sistemde kayıtlıdır!");
                //    return View(model);
                //}

                //Artık kullanıcımı kayıt edebilirim :D
                //not: acaba ben burada mapper ile dönüşüm yapabilir miyim?
                AppUser user = new AppUser()
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    UserName = model.Username,
                    Gender = model.Gender,
                    BirthDate = model.Birthdate,
                    EmailConfirmed = false // daha sonra buraya bakacağız
                };

                var result = _userManager.CreateAsync(user, model.Password).Result;
                if (!result.Succeeded)
                {
                    string hataMesaji = string.Empty;
                    if (result.Errors != null)
                    {
                        foreach (var item in result.Errors)
                        {
                            hataMesaji += $"{item.Description}\n";
                        }
                    }
                    ModelState.AddModelError("", $"Kayıt başarısızdır! {hataMesaji}");
                    return View(model);

                }

                //Sisteme kayıt oldu

                //Role ataması yapılabilir
                var roleResult = _userManager.AddToRoleAsync(user, Roles.WAITforVALIDATION.ToString()).Result;
                if (!roleResult.Succeeded)
                {
                    // burayı düşünmeliyiz!
                }

                //Hoşgeldin emaili atılabilir
                //Email confirm 

                //localhost.com/Account/Activation?u=userId&t=token
                var token = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                var encToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var buNedir = Request.Scheme;

                var url = Url.Action("Activation", "Account", new { u = user.Id, t = encToken }, protocol: Request.Scheme);

                //bool mailResult = _emailManager.SendEmail(new EmailMessageModel()
                //{
                //    Subject = $"AddrebookEdu - HOŞGELDİNİZ - AKTİVASYON İŞLEMİ",
                //    To = user.Email,
                //    Body = $"<b>Merhaba {user.Name} {user.Surname}, </b><br/>" +
                //        $"<h4>Sisteme kaydınız başarılıdır!</h4><br/>" +
                //        $"Sistemi kullanabilmeniz için aktivasyon yapmanız gerekiyor. <a href='{url}' target='_blank'>Buraya</a> tıklayarak aktivasyonu yapabilirsiniz."
                //});
                //TempData["RegisterSuccessMsg"] = $"Sayın {user.Name} {user.Surname}, kaydınız başarılıdır. Aktivasyon mailiniz gönderilmiştir!";
                //Not: Async ile de yapalım
                _emailManager.SendMailAsync(new EmailMessageModel()
                {
                    Subject = $"AddrebookEdu - HOŞGELDİNİZ - AKTİVASYON İŞLEMİ",
                    To = user.Email,
                    Body = $"<b>Merhaba {user.Name} {user.Surname}, </b><br/>" +
                        $"<h4>Sisteme kaydınız başarılıdır!</h4><br/>" +
                        $"Sistemi kullanabilmeniz için aktivasyon yapmanız gerekiyor. <a href='{url}' target='_blank'>Buraya</a> tıklayarak aktivasyonu yapabilirsiniz."
                });

                TempData["RegisterSuccessMsg"] = $"Sayın {user.Name} {user.Surname}, kaydınız başarılıdır. Aktivasyon mailiniz birkaç dakika içinde gönderilecektir!";



                //return ile anasayfaya yönlendirebiliriz.

                return RedirectToAction("Address", "Home");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "Beklenmedik hata oldu!");
                return View();
                // ex loglanmalı SErilog
            }

        }



        [HttpPost]
        public IActionResult Register2(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Girişleri düzgün yapınız!");
                    return View("Login2", model);
                }

                //Aynı usernameden var mı??

                var sameUserName = _userManager.FindByNameAsync(model.Username).Result;

                if (sameUserName != null)
                {
                    ModelState.AddModelError("", $"{model.Username} adlı bir kullanıcı adı zaten sistemde kayıtlıdır!");
                    return View("Login2", model);
                }

                //burayı program.cs içindeki  options.User.RequireUniqueEmail = true; ile yaptık.

                ////Aynı emailden var mı?
                //var sameEmail = _userManager.FindByEmailAsync(model.Email).Result;

                //if (sameEmail != null)
                //{
                //    ModelState.AddModelError("", $"{model.Email} emaili sistemde kayıtlıdır!");
                //    return View(model);
                //}

                //Artık kullanıcımı kayıt edebilirim :D
                //not: acaba ben burada mapper ile dönüşüm yapabilir miyim?
                AppUser user = new AppUser()
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    UserName = model.Username,
                    Gender = model.Gender,
                    BirthDate = model.Birthdate,
                    EmailConfirmed = false
                };

                var result = _userManager.CreateAsync(user, model.Password).Result;
                if (!result.Succeeded)
                {
                    string hataMesaji = string.Empty;
                    if (result.Errors != null)
                    {
                        foreach (var item in result.Errors)
                        {
                            hataMesaji += $"{item.Description}\n";
                        }
                    }
                    ModelState.AddModelError("", $"Kayıt başarısızdır! {hataMesaji}");
                    return View("Login2", model);

                }

                //Sisteme kayıt oldu

                //Role ataması yapılabilir
                var roleResult = _userManager.AddToRoleAsync(user, Roles.WAITforVALIDATION.ToString()).Result;
                if (!roleResult.Succeeded)
                {
                    // burayı düşünmeliyiz!
                }

                //Hoşgeldin emaili atılabilir
                //Email confirm 

                //localhost.com/Account/Activation?u=userId&t=token
                var token = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                var encToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var url = Url.Action("Activation", "Account", new { u = user.Id, t = encToken }, protocol: Request.Scheme);


                _emailManager.SendMailAsync(new EmailMessageModel()
                {
                    Subject = $"AddrebookEdu - HOŞGELDİNİZ - AKTİVASYON İŞLEMİ",
                    To = user.Email,
                    Body = $"<b>Merhaba {user.Name} {user.Surname}, </b><br/>" +
                        $"<h4>Sisteme kaydınız başarılıdır!</h4><br/>" +
                        $"Sistemi kullanabilmeniz için aktivasyon yapmanız gerekiyor. <a href='{url}' target='_blank'>Buraya</a> tıklayarak aktivasyonu yapabilirsiniz."
                });

                TempData["Register2SuccessMsg"] = $"Sayın {user.Name} {user.Surname}, kaydınız başarılıdır. Aktivasyon mailiniz birkaç dakika içinde gönderilecektir!";


                return RedirectToAction("Login2", "Account");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "Beklenmedik hata oldu!");
                return View("Login2", model);
                // ex loglanmalı SErilog
            }

        }





        public IActionResult Activation(string u, string t)
        {

            try
            {
                var user = _userManager.FindByIdAsync(u).Result;
                if (user == null)
                {
                    TempData["ActivationFailedMsg"] = $"Kullanıcı bulunamadı! İşlem başarısız!";
                    return RedirectToAction("Login", "Account");
                }

                if (user.EmailConfirmed)
                {
                    TempData["ActivationFailedMsg"] = $"Email aktivasyonu zaten tamamlanmıştır! Giriş yapabilirsiniz!";
                    return RedirectToAction("Login", "Account");
                }

                var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(t));


                var result = _userManager.ConfirmEmailAsync(user, token).Result;

                if (!result.Succeeded)
                {
                    string hataMesaji = string.Empty;
                    if (result.Errors != null)
                    {
                        foreach (var item in result.Errors)
                        {
                            hataMesaji += $"{item.Description}\n";
                        }
                    }
                    TempData["ActivationFailedMsg"] = $"Başarısız oldu.\n {hataMesaji}";
                    return RedirectToAction("Login", "Account");
                }

                //Rolünü değiştirelim
                //NOT: deleteRole ile addRole başarılı oldu mu diye bakılmalıdır :)) 
                var deleteRole = _userManager.RemoveFromRoleAsync(user, Roles.WAITforVALIDATION.ToString()).Result;

                var addRole = _userManager.AddToRoleAsync(user, Roles.MEMBER.ToString()).Result;



                TempData["ActivationSuccessMsg"] = $"Aktivasyon tamamlandı! Giriş yapabilirsiniz!";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                TempData["ActivationFailedMsg"] = $"Aktivasyon işleminiz beklenmedik bir hata nedeniyle başarısız oldu!";
                // ex loglanlamalı

                return RedirectToAction("Login", "Account");
            }
        }



        [Authorize]
        public IActionResult Profile()
        {
            try
            {
                //giriş yapmış olan kişinin bilgileri
                var username = User.Identity?.Name;
                if (username == null) // bu ife asla girmez çünkü zaten authorize var :D
                {
                    return RedirectToAction("Login2", "Account");
                }

                var user = _userManager.FindByNameAsync(username).Result;
                //deleted olan kullanıcı bu sayfaya erişemez!
                if (_userManager.IsInRoleAsync(user,Roles.DELETED.ToString()).Result)
                {
                    return RedirectToAction("Logout", "Account");
                }


                //NOT: automapperla dönüştürebilri miyiz??

                ProfileViewModel model = new ProfileViewModel()
                {
                    Birthdate = user.BirthDate,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Gender = user.Gender,
                    ProfilePicture = user.ProfilePicture,
                    Username = user.UserName,
                    NewUsername = string.Empty
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _loggerManager.LogMessage(LoggerLevel.Error, $"HATA:  Account/Profile httpost hata oldu! ex:{ex}");
                return View();
            }
        }


        [HttpPost]
        [Authorize]
        public IActionResult Profile(ProfileViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Girişleri düzgün yapınız!");
                    return View(model);
                }
                #region Yontem 1
                //giriş yapmış olan kişinin bilgileri
                var username = User.Identity?.Name;
                var user = _userManager.FindByNameAsync(username).Result;

                #endregion



                //Artık username'i de güncelleme isteği geldi. (assignment)
                if (!string.IsNullOrEmpty(model.NewUsername))
                {
                    var isSameUsername = _userManager.FindByNameAsync(model.NewUsername.ToLower()).Result;
                    if (isSameUsername != null)
                    {
                        ModelState.AddModelError("", "Bu kullanıcı adını alamazsınız! Tekrar deneyiniz");
                        return View(model);
                    }
                    user.UserName = model.NewUsername;
                }
                user.Name = model.Name;
                user.Surname = model.Surname;
                user.BirthDate = model.Birthdate;
                user.Gender = model.Gender;

                //1) Acaba sen resmi bana hangi yöntemden gönderdin??
                if (!string.IsNullOrEmpty(model.ProfilePicture))
                {
                    user.ProfilePicture = model.ProfilePicture;

                }
                else if (model.PictureFile != null)
                {
                    //Acaba sen bana formatına uygun bir dosya mı seçtin??
                    if (!model.PictureFile.ContentType.StartsWith("image"))
                    {
                        ModelState.AddModelError("", "Seçmiş olduğunuz dosya resim değil! Lütfen png ya da jpg ya da jpeg ya da webp uzantısına uygun resim seçiniz!");
                        return View(model);
                    }
                    if (model.PictureFile.Length == 0)
                    {
                        ModelState.AddModelError("", "Dosyanızın boyutu sıfır bunu yüklemem! Tekrar deneyiniz!");
                        return View(model);
                    }

                    string extention = Path.GetExtension(model.PictureFile.FileName);

                    string pictureName = $"{user.Name}_{user.Surname}_{Guid.NewGuid().ToString().Replace("-", "")}{extention}";

                    pictureName = GeneralManager.ReplaceTurkishCharactesTo(pictureName);

                    string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/ProfilePictures/{pictureName}");
                    string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProfilePictures");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        model.PictureFile.CopyTo(stream);
                        model.ProfilePicture = $"/ProfilePictures/{pictureName}";
                        user.ProfilePicture = model.ProfilePicture;
                    }
                }



                var result = _userManager.UpdateAsync(user).Result;
                if (!result.Succeeded)
                {

                    ModelState.AddModelError("", "Bilgileri güncelleyemedik! Tekrar deneyiniz!");
                    return View(model);
                }


                if (!string.IsNullOrEmpty(model.NewUsername) && model.NewUsername != model.Username)
                {
                    //sistemden logout yap
                    _logger.LogInformation($"Username değişti. Önceki username:{model.Username} mevcut username: {model.NewUsername}");
                    _signInManager.SignOutAsync();
                    TempData["ProfileUserNameChangedSuccessMsg"] = $"Sayın {user.Name} {user.Surname}, kullanıcı adınız değiştiği için tekrar giriş yapmanız gerekiyor. Kullanıcı Adı:{user.UserName}";
                    return RedirectToAction("Login", "Account");
                }

                ViewBag.ProfileSuccessMsg = "Bilgileriniz güncellendi!";
                return View(model);
            }
            catch (Exception)
            {
                return View();
            }
        }
        public IActionResult ForgetPassword()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {

                return View();
            }
        }



        [HttpPost]
        public IActionResult ForgetPassword(string? emailorUsername)
        {
            try
            {
                if (string.IsNullOrEmpty(emailorUsername))
                {
                    ModelState.AddModelError("", "Email / Kullanıcı adı girmediniz!!!");
                    return View();
                }

                //kullanıcıyı bulmaya çalışacağız?
                var user = _userManager.FindByEmailAsync(emailorUsername).Result;
                if (user == null)
                {
                    user = _userManager.FindByNameAsync(emailorUsername).Result;
                }

                if (user == null)
                {
                    ModelState.AddModelError("", "Sisteme kayıtlı olduğunuza emin misiniz....");
                    return View();
                }

                var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                var encodeToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var useridToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Id));

                var url = Url.Action("RecoverPassword", "Account", new { u = useridToken, t = encodeToken }, protocol: Request.Scheme);

                bool emailResult = _emailManager.SendEmailviaGmail(new EmailMessageModel()
                {
                    To = user.Email,
                    Subject = "AddressBookEdu - Şifremi UNUTTUM!",
                    Body = $"<b>Merhaba {user.Name} {user.Surname},</b><br/>"
                              + $"Şifrenizi unuttuğunuz için bu emaili gönderdik.<a href='{url}'> Buraya </a>tıklayarak yeni şifre alabilirsiniz."
                });
                if (emailResult)
                {
                    ViewBag.ForgetPasswordSuccessMsg = $"Sayın  {user.Name} {user.Surname} emailinize şifre yenileme maili gönderdik.";
                }
                else
                {
                    ViewBag.ForgetPasswordFailedMsg = $"Email gönderilemedi! Tekrar deneyiniz!";
                }
                return View();
            }
            catch (Exception ex)
            {

                ViewBag.ForgetPasswordFailedMsg = $"Beklenmedik bir hata oldu! {ex.Message}";
                //ex loglansın
                return View();
            }
        }


        [HttpGet]
        public IActionResult RecoverPassword(string? u, string? t)
        {
            try
            {
                if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(t))
                {
                    ModelState.AddModelError("", "Gerekli parametreler yok devam edemesiniz!");
                    return View();
                }
                var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(t));
                var userid = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(u));


                var user = _userManager.FindByIdAsync(userid).Result;
                if (user == null)
                {
                    ModelState.AddModelError("", "Kullanıcı bulunmadı! Devam edemezsiniz!");
                    return View();
                }

                RecoverPasswordVM model = new RecoverPasswordVM()
                {
                    Userid = userid,
                    User = user,
                    Token = token
                };
                return View(model);
            }
            catch (Exception)
            {

                ModelState.AddModelError("", "Beklenmedik sorun oldu!");
                return View(new RecoverPasswordVM());

            }
        }


        [HttpPost]
        public IActionResult RecoverPassword(RecoverPasswordVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", $"Girişlerinizi düzgüm yapınız!");
                    return View(model);
                }


                var user = _userManager.FindByIdAsync(model.Userid).Result;
                //tablondalki validdate < datetimenow
                var result = _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword).Result;

                if (!result.Succeeded)
                {
                    string hata = string.Empty;
                    if (result.Errors != null)
                    {
                        foreach (var item in result.Errors)
                        {
                            if (item.Code == "InvalidToken")
                            {
                                hata += $"Bu Link kullanılmıştır. \n";
                                continue;
                            }
                            hata += $"{item.Description} \n";
                        }
                    }
                    ModelState.AddModelError("", $"Şifrenizi kaydedemedik! \n{hata}");
                    return View(model);
                }

                TempData["RecoverPasswordSuccessMsg"] = $"{user.Name} {user.Surname} güncellendi! Giriş yapabilirsiniz.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Beklenmedik sorun oldu!");
                return View(model);
                //ex logla

            }
        }



        public JsonResult DeleteUser(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Kullanıcı adı bilgisini alamadığım için işlemi yapamadım! Tekrar deneyiniz!"
                    });
                }


                var user = _userManager.FindByNameAsync(username).Result;
                if (user == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Kullanıcıyı bulamadığım için işlemi yapamadım! Tekrar deneyiniz!"
                    });
                }

                //rol değişikliği
                var roles = _userManager.GetRolesAsync(user).Result;
                var roleDeleteResult = _userManager.RemoveFromRolesAsync(user, roles).Result;
                if (!roleDeleteResult.Succeeded)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"İşlem başarısızdır! Tekrar deneyiniz!"
                    });
                }

                var deletedRole = _userManager.AddToRoleAsync(user, Roles.DELETED.ToString()).Result;
                if (!deletedRole.Succeeded)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Deleted role ataması başarısızdır! Sistem yetkilisine ulaşınız!"
                    });
                    // log???
                } 
                return Json(new
                {
                    success = true,
                    message = $"Kaydınız sistemde DELETED olarak işlendi! Login sayfasına yönlendirileceksiniz. Hesabınızı tekrar kullanmak isterseniz lütfen login sayfasından giriş yapıp hesabı aktifleştiriniz.",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Beklenmedik bir hata oldu!{ex.Message}"
                });
                //ex loglanacak
            }
        }


        [Authorize]
        public IActionResult DeletedUsertoActive()
        {
            try
            {
                // giriş yapan kullanıcı bilgisi
                //giriş yapmış olan kişinin bilgileri
                var username = User.Identity?.Name;
                if (username == null) // bu ife asla girmez çünkü zaten authorize var :D
                {
                    return RedirectToAction("Login2", "Account");
                }

                var user = _userManager.FindByNameAsync(username).Result;
                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Beklenmedik sorun oldu!");
                _logger.LogError(ex, $"HATA: Account DeletedUsertoActive sayfası : {ex}");
                return View();
              

            }
        }



        [Authorize]
        [HttpPost]
        public IActionResult DeletedUsertoActive(AppUser model)
        {
            try
            {
                var user = _userManager.FindByNameAsync(model.UserName).Result;
                var deletedRoleResult = _userManager.RemoveFromRoleAsync(user, Roles.DELETED.ToString()).Result;
                if (!deletedRoleResult.Succeeded) 
                {
                    ModelState.AddModelError("", "İşlem başarısız tekrar deneyiniz!");
                    return View(model);
                }

                var memberRoleResult = _userManager.AddToRoleAsync(user, Roles.MEMBER.ToString()).Result;
                if (!memberRoleResult.Succeeded)
                {
                    ModelState.AddModelError("", "Rol atmasında sorun oluştu! Lütfen yetkili kişilerle görüşünüz!");
                    return View(model);
                }
                TempData["DeletedUsertoActiveSuccessMsg"] = $"{user.Name} {user.Surname}, rol güncellendi ve sistemi tekrar kullanbilirsiniz.";
                return RedirectToAction("Profile", "Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Beklenmedik sorun oldu!");
                _logger.LogError(ex, $"HATA: Account DeletedUsertoActive sayfası : {ex}");
                return View();


            }
        }

    }
}
