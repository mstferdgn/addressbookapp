using AddressBookAPI.JWTProcess;
using AddressBookBL.InterfacesOfManagers;
using AddressBookEL.IdentityModels;
using AddressBookEL.ViewModels;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AddressBookAPI.Controllers
{
    [Route("api/adres")]
    [ApiController]
    [Authorize]
    public class AddressControler:ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly IUserAddressManager _userAddressManager;
        private readonly ILogger<AddressControler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddressControler(UserManager<AppUser> userManager, IUserAddressManager userAddressManager, ILogger<AddressControler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _userAddressManager = userAddressManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("adresilerigetir")]
        public IActionResult GetAdress(string username) // burada parametre alındığı için acabaaaa BAŞKA birinin usernameini yazmış olabilir mi kontrol etmek gerekli... 
        {
            try
            {
                // not: claimsleri httpcontext üzerinden alırız. 
                // ya DI yaparak alabiliriz aşağıdaki gibi
                var claimsYontem1=   _httpContextAccessor.HttpContext.User.Claims;

                //ya da DI yapmaya gerek kalamadan alabiliriz aşağıdaki gibi
                var claimsYontem2 = HttpContext.User.Claims;


                //not: claims yöntem 1 ya da yöntem 2 ile alındıktan sonra o claims içinde username e erişmek yine 2 yolla yapılabilir

                // 1. yol
                bool isTrueUser = false;
                foreach (var item in claimsYontem1) //claimsYontem2
                {
                    if (item.Type == "username")
                    {
                        isTrueUser = item.Value == username.ToLower() ? true : false;
                        break;
                    }
                }
                if (!isTrueUser)
                {
                    return Ok("Lütfen kullanıcı adınız DOĞRU YAZINIZ!!");
                }

                // 2. yol
                var IsclaimUserNameTrue = HttpContext.User.Claims.ToArray()[1].Value.ToString()==username.ToLower() ?
                    true: false;

                if (!IsclaimUserNameTrue)
                {
                    return Ok("Lütfen kullanıcı adınız DOĞRU YAZINIZ!!");
                }


                List<UserAddressVM> data = new List<UserAddressVM>();
                var user = _userManager.FindByNameAsync(username.ToLower()).Result;
                if (user==null)
                {
                    return Ok("Kullanıcı bulunamadı");
                }

                data = _userAddressManager.GetSomeAll(x => x.UserId == user.Id).Data.ToList();
                List<string> address = new List<string>();
                foreach (var item in data)
                {
                    address.Add($"Adres: {item.AddressTitle} --> {item.City.Name} - {item.District.Name} - {item.Neigborhood.Name} {item.FullAddress} ");
                }
                return Ok(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"HATA: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}Authcontroller login post ex:{ex.Message}");
                return Ok("Beklenmedik hata");
            }
        }






        [HttpGet("adresilerigetir2")]
        public IActionResult GetAdress2() // parametre alınmıyor mecburen kendisinin adresleri gelir..
        {
            try
            {
                // not: claimsleri httpcontext üzerinden alırız. 
                // ya DI yaparak alabiliriz aşağıdaki gibi
                var claimsYontem1 = _httpContextAccessor.HttpContext.User.Claims;

                //ya da DI yapmaya gerek kalamadan alabiliriz aşağıdaki gibi
                var claimsYontem2 = HttpContext.User.Claims;


                //not: claims yöntem 1 ya da yöntem 2 ile alındıktan sonra o claims içinde username e erişmek yine 2 yolla yapılabilir

                // 1. yol
                string theUsername = string.Empty;
                foreach (var item in claimsYontem1) //claimsYontem2
                {
                    if (item.Type == "username")
                    {
                        theUsername= item.Value;
                        break;
                    }
                }

                // 2. yol
                var claimUserName = HttpContext.User.Claims.ToArray()[1].Value.ToString();


                List<UserAddressVM> data = new List<UserAddressVM>();
                var user = _userManager.FindByNameAsync(theUsername).Result; //claimUserName
                if (user == null)
                {
                    return Ok("Kullanıcı bulunamadı");
                }

                data = _userAddressManager.GetSomeAll(x => x.UserId == user.Id).Data.ToList();
                List<string> address = new List<string>();
                foreach (var item in data)
                {
                    address.Add($"Adres: {item.AddressTitle} --> {item.City.Name} - {item.District.Name} - {item.Neigborhood.Name} {item.FullAddress} ");
                }
                return Ok(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"HATA: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}Authcontroller login post ex:{ex.Message}");
                return Ok("Beklenmedik hata");
            }
        }

    }
}
