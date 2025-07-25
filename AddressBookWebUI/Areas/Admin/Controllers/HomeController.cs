using AddressBookBL.ImplementationofManagers;
using AddressBookBL.InterfacesOfManagers;
using AddressBookEL.AllEnums;
using AddressBookEL.IdentityModels;
using AddressBookEL.ViewModels;
using AddressBookWebUI.Areas.Admin.Models;
using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Resources;

namespace AddressBookWebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {

        private readonly ICityManager _cityManager;
        private readonly IDistrictManager _districtManager;
        private readonly INeigborhoodManager _neigborhoodManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserAddressManager _userAddressManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<AppRole> _roleManager;
        public HomeController(ICityManager cityManager, IDistrictManager districtManager, INeigborhoodManager neigborhoodManager, UserManager<AppUser> userManager, IUserAddressManager userAddressManager, IMapper mapper, RoleManager<AppRole> roleManager)
        {
            _cityManager = cityManager;
            _districtManager = districtManager;
            _neigborhoodManager = neigborhoodManager;
            _userManager = userManager;
            _userAddressManager = userAddressManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {

            try
            {
                if (!IsAdminLoggedIn())
                {
                    return RedirectToAction("Login", "Account", new { area = "Admin" });

                }


                ViewBag.TotalUserCount = _userManager.Users.Count();
                ViewBag.TotalAddressCount = _userAddressManager.GetAll().Data.Count();
                //hafta başı pzt
                //hafta bitiş pazar
                var date = DateTime.Now;

                DateTime weekStart = new DateTime();
                DateTime weekEnd = new DateTime();

                switch (date.DayOfWeek)
                {

                    case DayOfWeek.Monday:
                        weekStart = DateTime.Now;
                        weekEnd = DateTime.Now.AddDays(6);
                        break;
                    case DayOfWeek.Tuesday:
                        weekStart = DateTime.Now.AddDays(-1);
                        weekEnd = DateTime.Now.AddDays(5);
                        break;
                    case DayOfWeek.Wednesday:
                        weekStart = DateTime.Now.AddDays(-2);
                        weekEnd = DateTime.Now.AddDays(4);
                        break;
                    case DayOfWeek.Thursday:
                        weekStart = DateTime.Now.AddDays(-3);
                        weekEnd = DateTime.Now.AddDays(3);
                        break;
                    case DayOfWeek.Friday:
                        weekStart = DateTime.Now.AddDays(-4);
                        weekEnd = DateTime.Now.AddDays(2);
                        break;
                    case DayOfWeek.Saturday:
                        weekStart = DateTime.Now.AddDays(-5);
                        weekEnd = DateTime.Now.AddDays(1);
                        break;
                    case DayOfWeek.Sunday:
                        weekStart = DateTime.Now.AddDays(-6);
                        weekEnd = DateTime.Now;
                        break;
                    default:
                        break;
                }


                ViewBag.ThisWeekAddedAdress = _userAddressManager.GetSomeAll(
                    x =>
                x.CreatedDate.Year == DateTime.Now.Year
                && x.CreatedDate.Month == DateTime.Now.Month
                && x.CreatedDate.Day >= weekStart.Day
                && x.CreatedDate.Day <= weekEnd.Day).Data.Count();


                var lastDayofMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                    DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

                ViewBag.ThisMonthAddedAdress = _userAddressManager.GetSomeAll(
                 x =>
             x.CreatedDate.Year == DateTime.Now.Year
             && x.CreatedDate.Month == DateTime.Now.Month
             && x.CreatedDate.Day >= 1
             && x.CreatedDate.Day <= lastDayofMonth.Day).Data.Count();

                return View();
            }
            catch (Exception ex)
            {

                ViewBag.TotalUserCount = 0;
                ViewBag.TotalAddressCount = 0;
                ViewBag.ThisWeekAddedAdress = 0;
                return View();
            }
        }


        public IActionResult NeighAdd()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account", new { area = "Admin" });

            }
            try
            {
                ViewBag.Cities = _cityManager.GetSomeAll(x => !x.IsDeleted).Data.ToList();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Cities = new List<CityDTO>();
                ModelState.AddModelError("", $"Beklenmedik sorun oluştu! {ex.Message}");
                return View();
            }
        }

        [HttpPost]
        public IActionResult NeighAdd(NeigborhoodVM model)
        {
            try
            {
                ViewBag.Cities = _cityManager.GetSomeAll(x => !x.IsDeleted).Data.ToList();

                if (model.DistrictId == 0)
                {
                    //    ModelState.AddModelError("", $"Lütfen ilçe seçiniz!");
                    ViewBag.NeighAddFailedMsg = "Lütfen ilçe seçiniz!!";
                    return View();
                }
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", $"Gerekli alanları doldurunuz!");
                    return View();
                }

                Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
                model.Name = model.Name.ToUpper();
                model.CreatedDate = DateTime.Now;
                if (_neigborhoodManager.Add(model).IsSuccess)
                    ViewBag.NeighAddSuccessMsg = $"{model.Name} adlı mahalle eklenmiştir!";
                else
                    ViewBag.NeighAddFailedMsg = "Mahalle ekleme işlemi başarısız oldu!";

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Cities = new List<CityDTO>();
                ModelState.AddModelError("", $"Beklenmedik sorun oluştu! {ex.Message}");
                return View();
            }
        }
        public IActionResult NeighAddExcel()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account", new { area = "Admin" });

            }
            ViewBag.NeighAddExcelSuccessMsg = string.Empty;
            return View();
        }

        [HttpPost]
        public IActionResult NeighAddExcel(IFormFile excelFile)
        {
            try
            {

                //1) null
                if (excelFile == null)
                {
                    ModelState.AddModelError("", "Dosya eklediğinize emin olunuz!");
                    return View();
                }
                //2) content type
                #region Yontem 1
                //if (excelFile.ContentType!= "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                if (!excelFile.ContentType.Contains("spreadsheetml"))
                {
                    ModelState.AddModelError("", "Dosya türü excel olmalıdır!");
                    return View();
                }

                #endregion
                #region Yontem 2
                // content type a bakmak yerine FileName üzerinden uzantı alınıp kontrol edilebilir. best practice

                var fileExtension = Path.GetExtension(excelFile.FileName);
                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                {
                    ModelState.AddModelError("", "Dosya türü excel olmalıdır!");
                    return View();
                }
                #endregion
                //3)length
                if (excelFile.Length == 0)
                {
                    ModelState.AddModelError("", "Yüklediğiniz dosya tam olarak içeriğe sahip değil okuyamam!");
                    return View();
                }
                //exceli okuyacağız

                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), excelFile.FileName);

                int counter = 0;
                using (var wbook = new XLWorkbook(path))
                {
                    var worksheet = wbook.Worksheet(1);
                    foreach (var item in worksheet.RowsUsed())
                    {
                        if (item.RowNumber() == 1)
                            continue;
                        var platecode = item.Cell("B").Value.ToString().Trim();
                        var districtname = item.Cell("C").Value.ToString().Trim();
                        var neighName = item.Cell("D").Value.ToString().Trim();

                        //plaka kodundan ili bulucam
                        var city = _cityManager.GetbyCondition(x => x.PlateCode == platecode
                        && !x.IsDeleted).Data;
                        if (city != null)
                        {
                            //ilçe adından ve ilid ile ilçeyi bulucağım

                            var district = _districtManager.GetbyCondition(x =>
                            x.CityId == city.Id && x.Name == districtname && !x.IsDeleted).Data;


                            if (district != null)
                            {
                                //eğer mahalle tabloda ekli değilse ekleyeceğim

                                var neigh = _neigborhoodManager.GetbyCondition(x => !x.IsDeleted
                                 && x.DistrictId == district.Id
                                 && x.Name == neighName).Data;

                                if (neigh == null) // mahalle yoksa DBde
                                {
                                    //Mahalleyi ekle
                                    NeigborhoodVM n = new NeigborhoodVM()
                                    {
                                        DistrictId = district.Id,
                                        Name = neighName,
                                        CreatedDate = DateTime.Now,
                                        CityId = city.Id,
                                        IsDeleted = false
                                    };
                                    if (_neigborhoodManager.Add(n).IsSuccess)
                                        counter++;
                                }
                            }


                        }

                    } // foreach rowsused bitti
                } // workbook


                ViewBag.NeighAddExcelSuccessMsg = $"{counter} adet mahalle eklendi.";
                return View();

            }
            catch (Exception ex)
            {
                //ex loglanacak
                ViewBag.NeighAddExcelSuccessMsg = string.Empty;
                ModelState.AddModelError("", $"Beklenmedik sorun oluştu! {ex.Message}");
                return View();
            }
        }



        public JsonResult GetIndexStatisticsChartData()
        {
            StatisticsChartViewModel model = new StatisticsChartViewModel();

            try
            {
                //hafta başı pzt
                //hafta bitiş pazar
                var date = DateTime.Now;

                DateTime weekStart = new DateTime();
                DateTime weekEnd = new DateTime();

                switch (date.DayOfWeek)
                {

                    case DayOfWeek.Monday:
                        weekStart = DateTime.Now.AddDays(-7);
                        weekEnd = DateTime.Now.AddDays(-1);
                        break;
                    case DayOfWeek.Tuesday:
                        weekStart = DateTime.Now.AddDays(-8);
                        weekEnd = DateTime.Now.AddDays(-2);
                        break;
                    case DayOfWeek.Wednesday:
                        weekStart = DateTime.Now.AddDays(-9);
                        weekEnd = DateTime.Now.AddDays(-3);
                        break;
                    case DayOfWeek.Thursday:
                        weekStart = DateTime.Now.AddDays(-10);
                        weekEnd = DateTime.Now.AddDays(-4);
                        break;
                    case DayOfWeek.Friday:
                        weekStart = DateTime.Now.AddDays(-11);
                        weekEnd = DateTime.Now.AddDays(-5);
                        break;
                    case DayOfWeek.Saturday:
                        weekStart = DateTime.Now.AddDays(-5);
                        weekEnd = DateTime.Now.AddDays(1);
                        break;
                    case DayOfWeek.Sunday:
                        weekStart = DateTime.Now.AddDays(-13);
                        weekEnd = DateTime.Now.AddDays(-7);
                        break;
                    default:
                        break;
                }

                //yöntem 1
                //Dictionary<DayOfWeek, string> weekdaysTR = new Dictionary<DayOfWeek, string>();
                //weekdaysTR.Add(DayOfWeek.Monday, "Pazartesi");
                //weekdaysTR.Add(DayOfWeek.Tuesday, "Salı");
                //weekdaysTR.Add(DayOfWeek.Wednesday, "Çarşamba");
                //weekdaysTR.Add(DayOfWeek.Thursday, "Perşembe");
                //weekdaysTR.Add(DayOfWeek.Friday, "Cuma");
                //weekdaysTR.Add(DayOfWeek.Saturday, "Cumartesi");
                //weekdaysTR.Add(DayOfWeek.Sunday, "Pazar");

                //for (int i = 0; i < 7; i++)
                //{
                //    var theDate = weekStart.AddDays(i);
                //    model.Days[i] = $"{theDate.ToString("dd-MM-yyyy")} - {weekdaysTR[theDate.DayOfWeek]}";
                //}

                //2. yöntem
                //for (int i = 0; i < 7; i++)
                //{
                //    var theDate = weekStart.AddDays(i);
                //    model.Days[i] = $"{theDate.ToString("dd-MM-yyyy")} - {GeneralManager.GetTurkishWeekNamefromWeekDay(theDate.DayOfWeek)}";
                //}

                //3. yöntem harikaaaaaa 
                CultureInfo turkishCulture = new CultureInfo("tr-TR");
                DateTimeFormatInfo dtfi = turkishCulture.DateTimeFormat;
                for (int i = 0; i < 7; i++)
                {
                    var theDate = weekStart.AddDays(i);
                    string dayName = dtfi.GetDayName(theDate.DayOfWeek);
                    model.Days[i] = $"{theDate.ToString("dd-MM-yyyy")} - {dayName}";
                }

                var data = _userAddressManager.GetSomeAll(
                 x => x.CreatedDate.Year == DateTime.Now.Year
             && x.CreatedDate.Month == DateTime.Now.Month
             && x.CreatedDate.Day >= weekStart.Day
             && x.CreatedDate.Day <= weekEnd.Day).Data.GroupBy(y =>

             y.CreatedDate.Day).Select(g =>
             new
             {
                 day = g.Key,
                 count = g.Count()
             }
        ).ToList();
                var days = Enum.GetValues(typeof(DayOfWeek));
                Dictionary<DayOfWeek, int> datam = new Dictionary<DayOfWeek, int>();

                foreach (int item in days) //7 kere dönecek
                {
                    datam.Add((DayOfWeek)item, 0);
                    foreach (var subitem in data)
                    {

                        if ((DayOfWeek)item == new DateTime(weekStart.Year, weekStart.Month, subitem.day).DayOfWeek)
                        {
                            datam[(DayOfWeek)item] = subitem.count;
                            break;
                        }
                    }
                }


                model.Label1 = new StatisticsChartLabelViewModel()
                {
                    LabelName = "Toplam Adres Sayısı",
                };

                for (int i = 1; i <= 7; i++)
                {
                    if (i == 7)
                    {
                        model.Label1.Data.Add(datam.Values.ToList()[0]);
                    }
                    else
                    {
                        model.Label1.Data.Add(datam.Values.ToList()[i]);
                    }
                }


                var data2 = _userAddressManager.GetSomeAll(
                 x => x.CreatedDate.Year == DateTime.Now.Year
             && x.CreatedDate.Month == DateTime.Now.Month
             && x.CreatedDate.Day >= weekStart.Day
             && x.CreatedDate.Day <= weekEnd.Day
             && x.User.Gender == Gender.KADIN
             , joinTables: new string[] { "User" }
            ).Data.GroupBy(y =>
             y.CreatedDate.Day).Select(g =>
             new
             {
                 day = g.Key,
                 count = g.Count()
             }
        ).ToList();

                Dictionary<DayOfWeek, int> dataGenderFemale = new Dictionary<DayOfWeek, int>();

                foreach (int item in days) //7 kere dönecek
                {
                    dataGenderFemale.Add((DayOfWeek)item, 0);
                    foreach (var subitem in data2)
                    {
                        if ((DayOfWeek)item == new DateTime(weekStart.Year, weekStart.Month, subitem.day).DayOfWeek)
                        {
                            dataGenderFemale[(DayOfWeek)item] = subitem.count;
                            break;
                        }
                    }
                }

                model.Label2 = new StatisticsChartLabelViewModel()
                {
                    LabelName = "Kadınların Eklediği Adres Sayısı"
                };


                for (int i = 1; i <= 7; i++)
                {
                    if (i == 7)
                    {
                        model.Label2.Data.Add(dataGenderFemale.Values.ToList()[0]);
                    }
                    else
                    {
                        model.Label2.Data.Add(dataGenderFemale.Values.ToList()[i]);
                    }
                }


                var data3 = _userAddressManager.GetSomeAll(
               x => x.CreatedDate.Year == DateTime.Now.Year
           && x.CreatedDate.Month == DateTime.Now.Month
           && x.CreatedDate.Day >= weekStart.Day
           && x.CreatedDate.Day <= weekEnd.Day
           && x.User.Gender == Gender.ERKEK
           , joinTables: new string[] { "User" }
          ).Data.GroupBy(y =>
           y.CreatedDate.Day).Select(g =>
           new
           {
               day = g.Key,
               count = g.Count()
           }
      ).ToList();

                Dictionary<DayOfWeek, int> dataGenderMale = new Dictionary<DayOfWeek, int>();

                foreach (int item in days) //7 kere dönecek
                {
                    dataGenderMale.Add((DayOfWeek)item, 0);
                    foreach (var subitem in data3)
                    {
                        if ((DayOfWeek)item == new DateTime(weekStart.Year, weekStart.Month, subitem.day).DayOfWeek)
                        {
                            dataGenderMale[(DayOfWeek)item] = subitem.count;
                            break;
                        }
                    }
                }
                model.Label3 = new StatisticsChartLabelViewModel()
                {
                    LabelName = "Erkeklerin Eklediği Adres Sayısı"

                };


                for (int i = 1; i <= 7; i++)
                {
                    if (i == 7)
                    {
                        model.Label3.Data.Add(dataGenderMale.Values.ToList()[0]);
                    }
                    else
                    {
                        model.Label3.Data.Add(dataGenderMale.Values.ToList()[i]);
                    }
                }
                return Json(new
                {
                    success = true,
                    message = "Veriler geldi",
                    data = model
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Beklenmedik hata oldu!",
                    data = model
                });
            }
        }
        public JsonResult GetBarChartData()

        {
            List<ChartViewModel> model = new List<ChartViewModel>();
            try
            {
                byte[] marmaraBoglesi = new byte[] { 10, 11, 16, 17, 22, 34, 39, 41, 54, 59, 77 };
                model.Add(_userAddressManager.BolgelereGoreAdresSayilari("Marmara Bölgesi", marmaraBoglesi));

                model.Add(_userAddressManager.BolgelereGoreAdresSayilari("İç Anadolu Bölgesi", 6, 18, 26, 38, 40, 42, 50, 51, 58, 66, 68, 70, 71));

                model.Add(_userAddressManager.BolgelereGoreAdresSayilari("Karadeniz Bölgesi", 5, 8, 14, 19, 28, 29, 37, 52, 53, 55, 57, 60, 61, 67, 69, 74, 78, 81));
                model.Add(_userAddressManager.BolgelereGoreAdresSayilari("Akdeniz Bölgesi", 1, 7, 15, 31, 32, 33, 46, 80));

                model.Add(_userAddressManager.BolgelereGoreAdresSayilari("Doğu Anadolu Bölgesi", 4, 12, 13, 23, 24, 25, 30, 36, 44, 49, 62, 65, 75, 76));

                model.Add(_userAddressManager.BolgelereGoreAdresSayilari("Ege Bölgesi", 3, 9, 20, 35, 43, 45, 48, 64));

                model.Add(_userAddressManager.BolgelereGoreAdresSayilari("Güneydoğu Anadolu Bölgesi", 2, 21, 27, 47, 56, 63, 72, 73, 79));

                var regions = new List<string>();
                var counts = new List<int>();
                model.ForEach(x =>
                {
                    regions.Add(x.LabelName);
                    counts.Add(x.LabelValue);

                });

                return Json(new
                {
                    success = true,
                    message = $"Veriler geldi!",
                    regions,
                    counts

                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Beklenmedik hata oldu!",
                    data = model
                });
            }
        }

        public JsonResult GetBubleChartData()
        {
            List<BubleChartViewModel> model = new List<BubleChartViewModel>();
            try
            {


                var allroles = _roleManager.Roles.ToList();
                foreach (var role in allroles)
                { 
                    BubleChartViewModel m = new BubleChartViewModel();
                    var roleUserCount = _userManager.GetUsersInRoleAsync(role.Name).Result.Count();
                    if (roleUserCount>0)
                    {
                        m.Label = role.Name;

                        m.Data = new Dictionary<char, int>
                    {
                        { 'x', roleUserCount },
                        { 'y', roleUserCount },
                        { 'r', 10 },
                    };
                        model.Add(m);
                    }
                   
                }

                return Json(new
                {
                    success = true,
                    message = $"Veriler geldi!",
                    data=model

                });


            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Beklenmedik hata oldu!",
                    data = model
                });
            }
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

        public JsonResult GetDoughnutChartData()
        {
            List<ChartViewModel> model = new List<ChartViewModel>();
            try
            {
                //var results = from p in persons
                //              group p.car by p.PersonId into g
                //              select new { PersonId = g.Key, Cars = g.ToList() };
                model = _userManager.Users.ToList().
                    GroupBy(x=> x.Gender).Select(g =>
                new ChartViewModel()
                {
                    LabelName=g.Key.ToString(),
                    LabelValue = g.Count()

                }).ToList();

                var genders = new List<string>();
                var counts = new List<int>();
                model.ForEach(x =>
                {
                    genders.Add(x.LabelName);
                    counts.Add(x.LabelValue);

                });


                return Json(new
                {
                    success = true,
                    message = $"Veriler geldi!",
                    genders,
                    counts

                });


            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Beklenmedik hata oldu!",
                    genders = string.Empty,
                    counts=0
                });
            }
        }



        public IActionResult AllUsers()
        {
            try
            {
                var allusers = _userManager.Users.ToList();
                //   var data = new List<AllUsersViewModel>();
                // not: mapper ile mappelem yapabilirsek dıştaki foreache gerek kalmaz ve kod daha kolay yazılır
                //foreach (var user in allusers) 
                //{
                //    var theUser = new AllUsersViewModel()
                //    {
                //        BirthDate = user.BirthDate,
                //        Gender = user.Gender,
                //        Name = user.Name,
                //        ProfilePicture = user.ProfilePicture,
                //        Surname = user.Surname,
                //        Email= user.Email,
                //        Username=user.UserName
                //    };
                //    foreach (var item in _userManager.GetRolesAsync(user).Result)
                //    {
                //        theUser.RolesStatus += $"{item} , ";
                //    }
                //    theUser.RolesStatus = theUser.RolesStatus?.Trim().Trim(',');
                //    data.Add(theUser);
                //}

                var data = _mapper.Map<List<AppUser>, List<AllUsersViewModel>>(allusers);
                foreach (var item in data)
                {
                    var user = _userManager.FindByNameAsync(item.Username).Result;
                    var userRoles = _userManager.GetRolesAsync(user).Result;
                    foreach (var role in userRoles)
                    {
                        item.RolesStatus += $"{role} , ";
                    }
                    item.RolesStatus = item.RolesStatus?.Trim().Trim(',');
                }

                return View(data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Beklenmedik sorun oluştu! {ex.Message}");
                return View();
            }
        }

        public IActionResult AllAddress()
        {
            try
            {
                var data = _userAddressManager.GetAll(joinTables:
                    new string[] {"City","District", "Neigborhood","User" }).Data.ToList();
                return View(data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Beklenmedik sorun oluştu! {ex.Message}");
                return View();
            }
        }


        public IActionResult UpdateAddress(int? id)
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

                var result = _userAddressManager.GetbyCondition(x => x.Id == id.Value);  //admin herkesin adresini görüyor

                if (!result.IsSuccess)
                {
                    ModelState.AddModelError("", "Adres bulunamadı!");
                    return View(new UserAddressVM());
                }


                ViewBag.TheCities = _cityManager.GetAll().Data.ToList();

                ViewBag.TheDistricts = _districtManager.GetSomeAll(x => x.CityId == result.Data.CityId).Data.ToList();

                ViewBag.TheNeighs = _neigborhoodManager.GetSomeAll(x => x.CityId == result.Data.CityId && x.DistrictId == result.Data.DistrictId).Data.ToList();


                return View(result.Data);


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik hata oldu!");
                // log
              

                ViewBag.TheCities = _cityManager.GetAll().Data.ToList();

                ViewBag.TheDistricts = new List<DistrictDTO>();

                ViewBag.TheNeighs = new List<NeigborhoodVM>();

                return View(new UserAddressVM());
            }

        }


        [HttpPost]
        public IActionResult UpdateAddress(UserAddressVM model)
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

                if (model.CityId == 0 || model.DistrictId == 0 || model.NeigborhoodId == 0)
                {
                    ModelState.AddModelError("", "İl ilçe mahalle seçmedin! Güncelleme yapamam!");
                    return View(new UserAddressVM());
                }

                address.Data.CityId = model.CityId;
                address.Data.DistrictId = model.DistrictId;
                address.Data.NeigborhoodId = model.NeigborhoodId;
                address.Data.AddressTitle = model.AddressTitle;
                address.Data.FullAddress = model.FullAddress;

                if (model.PostalCode == "0")
                    address.Data.PostalCode = null;
                else if (model.PostalCode != address.Data.PostalCode)
                    address.Data.PostalCode = model.PostalCode;


                if (!_userAddressManager.Update(address.Data).IsSuccess)
                {
                    ModelState.AddModelError("", "Güncelleme BAŞARISIZDIR!");
                    return View(new UserAddressVM());
                }

                ViewBag.TheCities = _cityManager.GetAll().Data.ToList();

                ViewBag.TheDistricts = _districtManager.GetSomeAll(x => x.CityId == address.Data.CityId).Data.ToList();

                ViewBag.TheNeighs = _neigborhoodManager.GetSomeAll(x => x.CityId == address.Data.CityId && x.DistrictId == address.Data.DistrictId).Data.ToList();

                //1. yol kendi sayfasına gönderelim
                //ViewBag.AdminAddressEditSuccessMsg = "ADRES GÜNCELLENDİ!";
                //return View(address.Data);

                ////2. yol redirect edres indexe göndermek
                //TempData["AdminAddressEditSuccessMsg"] = "ADRES GÜNCELLENDİ!";
                //return RedirectToAction("AllAddress", "Home", new { area = "Admin" });

                //3. yol
                address.Data.AddressEditSuccessMsg = "Güncelle başarılıdır!";
                return View(address.Data);
            }
            catch (Exception)
            {

                throw;
            }

        }



    }
}
