using AddressBookBL.EmailSenderProcess;
using AddressBookBL.ImplementationofManagers;
using AddressBookBL.InterfacesOfManagers;
using AddressBookEL.AllEnums;
using AddressBookEL.IdentityModels;
using AddressBookEL.ViewModels;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;

namespace AddressBookWebUI.CreateDefaultData
{
    public class CreateData
    {
        private readonly IConfiguration _configuration; // appsettings.jsona gidebilmek için

        public CreateData(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void CreateAllRoles(IServiceProvider serviceProvider)
        {
            try
            {

                var devamEdeyimMi = Convert.ToBoolean(_configuration.GetSection("CreateRoles").Value);
                if (!devamEdeyimMi)
                {
                    return;
                }


                var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
                var emailManager = serviceProvider.GetRequiredService<IEmailManager>();

                var allRoleNames = Enum.GetNames(typeof(Roles));
                foreach (var item in allRoleNames)
                {
                    //eğer bu rol tabloda yoksa ekle

                    var result = roleManager.RoleExistsAsync(item).Result;
                    var result2 = roleManager.FindByNameAsync(item).Result;

                    if (!result)
                    {
                        AppRole role = new AppRole()
                        {
                            Description = $"Sistem tarafından {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff")} oluşturuldu!",
                            Name = item
                        };
                        var r = roleManager.CreateAsync(role).Result;

                    }

                    //if (result==null)
                    //{
                    //    AppRole role = new AppRole()
                    //    {
                    //        Description = $"Sistem tarafından {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff")} oluşturuldu!",
                    //        Name = item
                    //    };
                    //    var r = roleManager.CreateAsync(role).Result;
                    //}
                }
            }
            catch (Exception ex)
            {
                //log

            }

        }



        public void CreateAllRoles(RoleManager<AppRole> roleManager, IEmailManager emailManager)
        {
            try
            {

                var allRoleNames = Enum.GetNames(typeof(Roles));
                foreach (var item in allRoleNames)
                {
                    //eğer bu rol tabloda yoksa ekle

                    var result = roleManager.RoleExistsAsync(item).Result;
                    var result2 = roleManager.FindByNameAsync(item).Result;

                    if (!result)
                    {
                        AppRole role = new AppRole()
                        {
                            Description = $"Sistem tarafından {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff")} oluşturuldu!",
                            Name = item
                        };
                        var r = roleManager.CreateAsync(role).Result;

                    }

                    //if (result==null)
                    //{
                    //    AppRole role = new AppRole()
                    //    {
                    //        Description = $"Sistem tarafından {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff")} oluşturuldu!",
                    //        Name = item
                    //    };
                    //    var r = roleManager.CreateAsync(role).Result;
                    //}
                }
            }
            catch (Exception ex)
            {
                //log

            }

        }

        public void SaveAllCitytoDBViaExcel(ICityManager cityManager)
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ILLER.xlsx");
                using (var wbook = new XLWorkbook(path))
                {
                    var worksheet = wbook.Worksheet(1);
                    foreach (var item in worksheet.RowsUsed())
                    {
                        if (item.RowNumber() > 1)
                        {
                            var plakaKod = item.Cell("A").Value.ToString();
                            var ilAdi = item.Cell("B").Value.ToString();

                            //Acaba bu il CITY tablosunda var mı yok mu? yok ise ekle!!!
                            var cityExist = cityManager.GetbyCondition(x => x.PlateCode == plakaKod).Data;


                            if (cityExist == null)
                            {
                                CityDTO city = new CityDTO()
                                {
                                    CreatedDate = DateTime.Now,
                                    IsDeleted = false,
                                    Name = ilAdi,
                                    PlateCode = plakaKod
                                };
                                cityManager.Add(city);
                            }
                        }

                    }


                }
            }
            catch (Exception ex)
            {
                //loglama

            }
        }
        public void SaveAllDistricttoDBViaExcel(IDistrictManager districtManager, ICityManager cityManager)
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ilce-listesi.xlsx");
                using (var wbook = new XLWorkbook(path))
                {
                    var worksheet = wbook.Worksheet(1);
                    foreach (var item in worksheet.RowsUsed())
                    {
                        if (item.RowNumber() > 1)
                        {
                            try
                            {
                                var plakaKod = Convert.ToInt32(item.Cell("B").Value.ToString());
                                var ilceAdi = item.Cell("C").Value.ToString();

                                //ili bul
                                var city = cityManager.GetbyCondition(x => Convert.ToInt32(x.PlateCode) == plakaKod).Data;
                                if (city!=null)
                                {
                                    var districtExist = districtManager.GetbyCondition(x => x.CityId == city.Id &&
                                    x.Name.ToLower() == ilceAdi.ToLower()).Data;

                                    //ilçe yoksa ekle
                                    if (districtExist == null)
                                    {
                                        DistrictDTO district = new DistrictDTO()
                                        {
                                            CityId= city.Id,
                                            CreatedDate = DateTime.Now,
                                            IsDeleted = false,
                                            Name=ilceAdi
                                        };
                                        districtManager.Add(district);
                                    }
                                }
                            }
                            catch (Exception exc)
                            {

                              //ilçe eklenmedi kaldığı yerden devam burada log atılabilir
                            }



                            
                        }

                    }


                }
            }
            catch (Exception ex)
            {
                //loglama

            }
        }
    }
}
