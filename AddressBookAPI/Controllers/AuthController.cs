using AddressBookEL.IdentityModels;
using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AddressBookAPI.JWTProcess;
using AddressBookEL.JWTModels;
using AddressBookEL.ResultModels;
using Microsoft.AspNetCore.Authorization;

namespace AddressBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenManager _tokenManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<AppUser> userManager, ITokenManager tokenManager, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _tokenManager = tokenManager;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Login(string? emailorUsername, string? password)
        {
            UserLoginResponse response = new UserLoginResponse();
            try
            {
                if (string.IsNullOrEmpty(emailorUsername) || string.IsNullOrEmpty(password))
                {
                    return BadRequest("Email ya da kullanıcı adı ve şifre alanlarını giriniz!");
                }
                //emailorUsername gelen parametreye ait kullanııc var mı?
                var user = _userManager.FindByEmailAsync(emailorUsername).Result;
                if (user == null)
                {
                    user = _userManager.FindByNameAsync(emailorUsername).Result;
                }
                if (user == null)
                {
                    return BadRequest("Lütfen sisteme kayıtlı email ya da kullanıcı adınızla giriş yapınız! ");
                    
                }

                // user elimde
                var result =  _tokenManager.GenerateToken(user).Result;
               
                response.AuthenticateResult = true;
                response.AuthToken = result.AuthToken;
                response.AccessTokenExpireDate = result.AccessTokenExpireDate;

                _logger.LogInformation($"Token oluştu!username: {user.UserName} token:{result.AuthToken}");

                return Ok(response);
            }

            catch (Exception ex)
            {
                response.AuthenticateResult = false;
                response.AuthToken = string.Empty;
                response.AccessTokenExpireDate = DateTime.Now;
                response.Description = $"Beklenmedik hata: {ex.Message}";
                _logger.LogError(ex, $"HATA: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}Authcontroller login post ex:{ex.Message}");
                return Ok(response);
            }
        }

        [HttpGet("kullanicisayisi")]
        public IActionResult UserCount()
        {
            try
            {
               int result= _userManager.Users.Count();
                return Ok(new
                {
                    Message="Sistemdeki toplam kullanıcı sayısı",
                    Count=result
                });

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Message = $"Beklenmedik bir hata oldu! ex:{ex.Message}",
                    Count = -1
                });
            }
        }
        [HttpGet("getusers")]
        [Authorize]
        public IActionResult GetUsers()
        {
            try
            {
                var result = _userManager.Users.ToList();
                var msg = string.Empty;
                result.ForEach(x => msg += $"{x.Name} {x.Surname} ");
                return Ok(new
                {
                    Message = msg,
                });

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Message = $"Beklenmedik bir hata oldu! ex:{ex.Message}",
                });
            }
        }
    }
}
