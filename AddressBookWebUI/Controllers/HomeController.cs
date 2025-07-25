using AddressBookBL.ImplementationofManagers;
using AddressBookBL.InterfacesOfManagers;
using AddressBookEL.AllEnums;
using AddressBookEL.Entities;
using AddressBookEL.IdentityModels;
using AddressBookEL.PostalCodeApiModels;
using AddressBookEL.ResultModels;
using AddressBookEL.ViewModels;
using AddressBookWebUI.Models;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace AddressBookWebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICityManager _cityManager;
        private readonly IDistrictManager _districtManager;
        private readonly INeigborhoodManager _neighborhoodManager;
        private readonly IUserAddressManager _userAddressManager;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ICityManager cityManager, IDistrictManager districtManager, INeigborhoodManager neighborhoodManager, IUserAddressManager userAddressManager, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _cityManager = cityManager;
            _districtManager = districtManager;
            _neighborhoodManager = neighborhoodManager;
            _userAddressManager = userAddressManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        public IActionResult Address()
        {
            try
            {
                var username = User.Identity?.Name;
                var user = _userManager.FindByNameAsync(username).Result;

                //deleted olan kullanıcı bu sayfaya erişemez!
                if (_userManager.IsInRoleAsync(user, Roles.DELETED.ToString()).Result)
                {
                    return RedirectToAction("Logout", "Account");
                }

                var model = new UserAddressVM()
                {
                    UserId = user.Id
                };

                ViewBag.Cities = _cityManager.GetAll().Data.ToList();
                ViewBag.AddressSuccessMsg = string.Empty;
                var userAdress = _userAddressManager.GetSomeAll(x =>
                !x.IsDeleted &&
                x.UserId == user.Id, new string[] { "City", "District", "Neigborhood", "User" }).Data;

                ViewBag.UserAddress = userAdress;
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Cities = new List<CityDTO>();
                ViewBag.AddressSuccessMsg = string.Empty;
                ViewBag.UserAddress = new List<UserAddressVM>(); ;
                return View(new UserAddressVM());
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Address(UserAddressVM model)
        {
            try
            {

                ViewBag.Cities = _cityManager.GetAll().Data.ToList();
                ViewBag.AddressSuccessMsg = string.Empty;
                var userAdress = _userAddressManager.GetSomeAll(x => !x.IsDeleted &&
            x.UserId == model.UserId, new string[] { "City", "District", "Neigborhood", "User" }).Data;

                ViewBag.UserAddress = userAdress;
                if (model.PostalCode == "0") model.PostalCode = null;



                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Girişleri düzgün yapınız!");
                    return View();
                }
                model.CreatedDate = DateTime.Now;
                if (!_userAddressManager.Add(model).IsSuccess)
                {
                    ModelState.AddModelError("", "Ekleme başarısız!");
                    return View(model);
                }
                ViewBag.AddressSuccessMsg = "Yeni adres eklendi!";

                userAdress = _userAddressManager.GetSomeAll(x => !x.IsDeleted &&
            x.UserId == model.UserId, new string[] { "City", "District", "Neigborhood", "User" }).Data;

                ViewBag.UserAddress = userAdress;
                return View();
            }
            catch (Exception ex)
            {
                var userAdress = _userAddressManager.GetSomeAll(x => !x.IsDeleted &&
             x.UserId == model.UserId, new string[] { "City", "District", "Neigborhood", "User" }).Data;

                ViewBag.UserAddress = userAdress;
                ModelState.AddModelError("", "Beklenmedik hata oldu!");
                return View(model);
            }
        }
        public IActionResult Deneme()
        {
            return View();
        }

        public JsonResult GetDistrictsofCity(int cityid)
        {
            try
            {
                if (cityid > 0)
                {
                    var data = _districtManager.GetSomeAll(x => x.CityId == cityid).Data.OrderBy(x => x.Name).ToList();
                    return Json(new
                    {
                        success = true,
                        message = $"{data.Count} adet ilçe geldi",
                        data

                    });
                }
                return Json(new
                {
                    success = false,
                    message = $"İlçeleri getiremedi",
                    data = new List<DistrictDTO>()

                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Beklenmedik bir hata oldu!{ex.Message}",
                    data = new List<DistrictDTO>()

                });
                //ex loglanacak
            }
        }
        public JsonResult GetNeigborhoodOfDistrict(int id)
        {
            try
            {
                if (id > 0)
                {
                    var data = _neighborhoodManager.GetSomeAll(x => x.DistrictId == id).Data.
                        OrderBy(x => x.Name).ToList();
                    return Json(new
                    {
                        success = true,
                        message = $"{data.Count} adet mahalle geldi",
                        data

                    });
                }
                return Json(new
                {
                    success = false,
                    message = $"Mahalleri getiremedi",
                    data = new List<NeigborhoodVM>()

                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Beklenmedik bir hata oldu!{ex.Message}",
                    data = new List<NeigborhoodVM>()

                });
                //ex loglanacak
            }
        }

        [Authorize]
        public IActionResult AddressDelete(int? id)
        {
            try
            {
                if (id != null)
                {
                    var adress = _userAddressManager.GetById(id.Value).Data;

                    if (adress != null)
                    {
                        var username = User.Identity?.Name;
                        var user = _userManager.FindByNameAsync(username).Result;
                        if (adress.UserId != user.Id)
                        {
                            TempData["AddressDeleteFailedMsg"] = "Silinmesi istenen adresin kullanıcı id bilgisi ile sizin id bilginiz uyuşmamaktadır! Başkasına ait adresi silemezsiniz!";
                            return RedirectToAction("Address", "Home");
                        }
                        // bu adres sana ait mi?

                        adress.IsDeleted = true; // soft delete
                        if (!_userAddressManager.Update(adress).IsSuccess)
                        {
                            TempData["AddressDeleteFailedMsg"] = "Adres silindi!";
                            return RedirectToAction("Address", "Home");
                        }
                        TempData["AddressDeleteSuccessMsg"] = "Adres silindi!";
                        return RedirectToAction("Address", "Home");
                    }
                }
                TempData["AddressDeleteFailedMsg"] = "Gerekli parametre gelmediği için işlem yapılamaz";
                return RedirectToAction("Address", "Home");
            }
            catch (Exception ex)
            {
                TempData["AddressDeleteFailedMsg"] = "Beklenmedik hata oluştu!";
                return RedirectToAction("Address", "Home");
            }
        }



        //public JsonResult GetPostalCode(int cid, int did, int nid)
        //{
        //    try
        //    {

        //        var city = _cityManager.GetById((byte)cid).Data;
        //        if (city == null)
        //        {
        //            return Json(new { issuccess = false, postalcode = 0 });

        //        }
        //        var district = _districtManager.GetById(did).Data;

        //        if (district == null)
        //        {
        //            return Json(new { issuccess = false, postalcode = 0 });

        //        }
        //        var neigh = _neighborhoodManager.GetById(nid).Data;
        //        if (neigh == null)
        //        {
        //            return Json(new { issuccess = false, postalcode = 0 });

        //        }
        //        using (WebClient client = new WebClient())
        //        {
        //            client.BaseAddress = $"http://localhost:52605/api/Neigbor/GetPostalCode?cName={city.Name}&dName={district.Name}&nName={neigh.Name}";

        //            var response = client.DownloadString(client.BaseAddress);

        //            if (response.Contains("Status"))
        //            {
        //                return Json(new { issuccess = false, postalcode = 0, message = "posta kodunu bulamadım" });


        //            }

        //            var result = JsonConvert.DeserializeObject<DataResult<string>>(response);
        //            return Json(new { issuccess = true, postalcode = result.Data, message = "posta kodunu getirdi" });

        //            //ardından classımı if ile sorgulayarak ilgili mahalleye ulaşacağım
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(new { issuccess = false, postalcode = 0, message = "Beklenmedik hata!" });

        //    }
        //}




        public JsonResult GetPostalCode(int cid, int did, int nid)
        {
            try
            {

                var city = _cityManager.GetById((byte)cid).Data;
                if (city == null)
                {
                    return Json(new { issuccess = false, postalcode = 0 });

                }
                using (WebClient client = new WebClient())
                {
                    var plaka = Convert.ToInt32(city.PlateCode);
                    client.BaseAddress = "https://api.ubilisim.com/postakodu/il/" + plaka;

                    var response = client.DownloadString(client.BaseAddress);
                    //apinin bana verdiği json sonucu deserialize ederek classımın içine atacapğım
                    var result = JsonConvert.DeserializeObject<UbilisimAPIResultModel>(response);

                    var district = _districtManager.GetById(did).Data;
                    var neigh = _neighborhoodManager.GetById(nid).Data;
                    if (district == null || neigh == null)
                    {
                        return Json(new { issuccess = false, postalcode = 0 });

                    }

                    var n = neigh.Name.ToUpper();
                    //ılgaz  
                    //ılgaz mah.
                    //ılgaz mah
                    //ılgaz mh 
                    //ılgaz mh.
                    //ılgaz mahalle
                    //ılgaz mahallesi 
                    //şehit ahmet ılgaz mahallesi 


                    //kelime sayısından bakalım
                    string[] mahallem = n.Split(' ');
                    //en sonuncusuna gidelim
                    var mahalle_son = mahallem[mahallem.Length - 1];
                    bool idealMi = false;
                    if (mahalle_son == "MAH")
                    {
                        idealMi = true;
                    }
                    if (mahalle_son == "MAHALLESİ")
                    {
                        mahalle_son = mahalle_son.Replace("MAHALLESİ", "MAH").ToUpper();
                    }
                    else if (mahalle_son == "MAHALLE")
                    {
                        mahalle_son = mahalle_son.Replace("MAHALLE", "MAH").ToUpper();
                    }

                    else if (mahalle_son == "MAH.")
                    {
                        mahalle_son = mahalle_son.Replace("MAH.", "MAH").ToUpper();
                    }
                    else if (mahalle_son == "MH")
                    {
                        mahalle_son = mahalle_son.Replace("MH", "MAH").ToUpper();
                    }
                    else if (mahalle_son == "MH.")
                    {
                        mahalle_son = mahalle_son.Replace("MH.", "MAH").ToUpper();
                    }
                    else if (mahalle_son != "MAH")
                    {
                        n = $"{n} MAH";
                    }

                    var neighSearch = string.Empty;
                    if (mahalle_son == "MAH" && !idealMi)
                    {
                        for (int i = 0; i < mahallem.Length; i++)
                        {
                            if (i < mahallem.Length - 1)
                            {
                                neighSearch += $"{mahallem[i]} ";
                            }
                            else
                            {
                                neighSearch += mahalle_son;
                            }
                        }

                    }
                    else
                    {
                        neighSearch = n;
                    }
                    foreach (var item in result.PostaKodu)
                    {

                        if (item.Ilce == district.Name.ToUpper() && item.Mahalle == neighSearch.Trim())
                        {
                            return Json(new { issuccess = true, postalcode = item.Pk, message = "Posta kodu geldi" });
                        }
                    }

                    //ardından classımı if ile sorgulayarak ilgili mahalleye ulaşacağım
                }

                return Json(new { issuccess = false, postalcode = 0, message = "posta kodunu bulamadım" });
            }
            catch (Exception ex)
            {

                return Json(new { issuccess = false, postalcode = 0, message = "Beklenmedik hata!" });

            }
        }


        [Authorize]
        public IActionResult AddressEdit(int? id)
        {
            try
            {
                ViewBag.TheCities = _cityManager.GetAll().Data.ToList();

                ViewBag.TheDistricts = new List<DistrictDTO>();

                ViewBag.TheNeighs = new List<NeigborhoodVM>();
                //id null mı
                if (id == null)
                {
                    ModelState.AddModelError("", "Adres id null olduğunda editlenecek bir adres yok demektir!");
                    return View(new UserAddressVM());
                }
                if (id <= 0)
                {
                    ModelState.AddModelError("", "id değeri sıfırdan büyük olmalıdır!");
                    return View(new UserAddressVM());
                }

                var username = User.Identity?.Name;
                var user = _userManager.FindByNameAsync(username).Result;

                var result = _userAddressManager.GetbyCondition(x => x.Id == id.Value && x.UserId==user.Id);  //gelen adres sana ait mi?
               
                if (!result.IsSuccess)
                {
                    ModelState.AddModelError("", "Adres bulunamadı!");
                    return View(new UserAddressVM());
                }

              
                ViewBag.TheCities = _cityManager.GetAll().Data.ToList();

                ViewBag.TheDistricts = _districtManager.GetSomeAll(x => x.CityId == result.Data.CityId ).Data.ToList();

                ViewBag.TheNeighs = _neighborhoodManager.GetSomeAll(x => x.CityId == result.Data.CityId && x.DistrictId== result.Data.DistrictId).Data.ToList();


                return View(result.Data);


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik hata oldu!");
                // log
                _logger.LogError(ex, $"{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")} - HATA: Home controller EditAddress httpget: {ex}");

                ViewBag.TheCities = _cityManager.GetAll().Data.ToList();

                ViewBag.TheDistricts = new List<DistrictDTO>();

                ViewBag.TheNeighs = new List<NeigborhoodVM>();

                return View(new UserAddressVM());
            }
        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddressEdit(UserAddressVM model)
        {
            try
            {
                ViewBag.TheCities = _cityManager.GetAll().Data.ToList();

                ViewBag.TheDistricts = new List<DistrictDTO>();

                ViewBag.TheNeighs = new List<NeigborhoodVM>();

                //veri tabanından adresi çekeceğim
                var address = _userAddressManager.GetById(model.Id);
                if (!address.IsSuccess)
                {
                    ModelState.AddModelError("", "Adres bulunamadı İşlem başarısız");
                    return View(new UserAddressVM());
                }

                if (model.CityId==0 || model.DistrictId==0 || model.NeigborhoodId==0)
                {
                    ModelState.AddModelError("", "İl ilçe mahalle seçmedin! Güncelleme yapamam!");
                    return View(new UserAddressVM());
                }

                address.Data.CityId=model.CityId;
                address.Data.DistrictId=model.DistrictId;
                address.Data.NeigborhoodId=model.NeigborhoodId;
                address.Data.AddressTitle=model.AddressTitle;
                address.Data.FullAddress=model.FullAddress;
             
                if (model.PostalCode == "0") 
                    address.Data.PostalCode = null;
                else if (model.PostalCode!=address.Data.PostalCode)
                    address.Data.PostalCode = model.PostalCode;
                

                if(!_userAddressManager.Update(address.Data).IsSuccess)
                {
                    ModelState.AddModelError("", "Güncelleme BAŞARISIZDIR!");
                    return View(new UserAddressVM());
                }

                ViewBag.TheCities = _cityManager.GetAll().Data.ToList();

                ViewBag.TheDistricts = _districtManager.GetSomeAll(x => x.CityId == address.Data.CityId).Data.ToList();

                ViewBag.TheNeighs = _neighborhoodManager.GetSomeAll(x => x.CityId == address.Data.CityId && x.DistrictId == address.Data.DistrictId).Data.ToList();

                //1. yol kendi sayfasına gönderelim
                //ViewBag.AddressEditSuccessMsg = "ADRES GÜNCELLENDİ!";
                //return View(address.Data);

                //2. yol redirect edres indexe göndermek
                TempData["AddressEditSuccessMsg"] = "ADRES GÜNCELLENDİ!";
                return RedirectToAction("Address", "Home");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
