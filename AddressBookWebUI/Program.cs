using AddressBookBL.EmailSenderProcess;
using AddressBookBL.ImplementationofManagers;
using AddressBookBL.InterfacesOfManagers;
using AddressBookDL.ContextInfo;
using AddressBookEL.AllEnums;
using AddressBookEL.Entities;
using AddressBookEL.IdentityModels;
using AddressBookEL.Mappings;
using AddressBookEL.ViewModels;
using AddressBookWebUI.Areas.Admin.Models;
using AddressBookWebUI.CreateDefaultData;
using AutoMapper.Extensions.ExpressionMapping;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;

namespace AddressBookWebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // serilog logger ayarlari

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            //context ayarı
            builder.Services.AddDbContext<AddressBookContext>(options =>
            {

                //.net framework mvc projesinde connection stringi web.configten almıştık. Core projesinde webconfig dosyası yok! Bu durumda biz connectionstringi appsettings.json aldı dosyadan alacağız.
                options.UseSqlServer(builder.Configuration.GetConnectionString("BetulHoca"));
                //options.UseSqlServer(builder.Configuration.GetConnectionString("Omer"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            });

            //identity ayarı
            builder.Services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                //kullanıcının parola ayarları içindir
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
                //options.User.AllowedUserNameCharacters = "abcdefgğhijklmnopqrstuüvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/";
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<AddressBookContext>();

            //cookie ayarı
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";

    }
    );
            builder.Services.ConfigureApplicationCookie(opt =>
            {
                opt.Cookie.HttpOnly = true;
                //opt.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                opt.ExpireTimeSpan = TimeSpan.FromHours(1);
                opt.LoginPath = "/Account/Login";
                opt.AccessDeniedPath = "/Account/Login";
                //Sliding expiration: Belirli bir süre boyunca işlem yapılmazsa cachedeki veri expire olur, eğer işlem yapılırsa süre tekrar uzar.
                opt.SlidingExpiration = true;

            });



            //automapper

            builder.Services.AddAutoMapper(opt =>
            {
                opt.AddExpressionMapping();
                //opt.CreateMap<City, CityDTO>().ReverseMap();
                // opt.CreateMap<District, DistrictDTO>().ReverseMap();
                // opt.CreateMap<UserAddress, UserAddressVM>().ReverseMap();
                opt.CreateMap<AppUser, AllUsersViewModel>().ReverseMap();
                opt.AddProfile(typeof(Maps));
            });


            //Managerlarımızı controllerlarımız DI olarak kullanbilmemiz için buraya ayar kodlarınız yazmalıyız
            //DI yaşam döngüsü

            builder.Services.AddScoped<IEmailManager, EmailManager>();
            builder.Services.AddScoped<ICityManager, CityManager>();
            builder.Services.AddScoped<IDistrictManager, DistrictManager>();
            builder.Services.AddScoped<IUserAddressManager, UserAddressManager>();
            builder.Services.AddScoped<INeigborhoodManager, NeigborhoodManager>();
            builder.Services.AddScoped<ILoggerManager, LoggerManager>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();


            var trCulture = new CultureInfo("tr-TR");
            var localizationOptions = new RequestLocalizationOptions()
            {
                SupportedCultures = new List<CultureInfo>()
        {
            trCulture
        },
                SupportedUICultures = new List<CultureInfo>()
        {
            trCulture
        },
                DefaultRequestCulture = new RequestCulture(trCulture),
                FallBackToParentCultures = false,
                FallBackToParentUICultures = false,
                RequestCultureProviders = null
            };

            app.UseRequestLocalization(localizationOptions);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles(); // wwwroot klasörünün dahil olması için yazılması gerekli olan ayar.

            //tema kullanacaksanız wwwroot içine yerleştirmelisiniz.
            app.UseRouting();  // aşağıdaki app.MappControllerRoute'u kullanabilmemiz için yazılması gereken ayar

            app.UseAuthentication();// login ve logout
            app.UseAuthorization(); // yetki



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });

            //Route.Config
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            using (var appScope = app.Services.CreateScope())
            {
                var serviceProvider = appScope.ServiceProvider;

                //#region Yontem 1 
                //var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();

                //var emailManager = serviceProvider.GetRequiredService<IEmailManager>();

                //CreateData createData = new CreateData();
                //createData.CreateAllRoles(roleManager, emailManager);

                //#endregion


                #region Yontem 2 
                CreateData createData = new CreateData(builder.Configuration);

                if (Convert.ToBoolean(builder.Configuration.GetSection("CreateRoles").Value))
                {
                    createData.CreateAllRoles(serviceProvider);
                }
                if (Convert.ToBoolean(builder.Configuration.GetSection("CreateCities").Value))
                {
                    var cityManager = serviceProvider.GetRequiredService<ICityManager>();
                    createData.SaveAllCitytoDBViaExcel(cityManager);
                }

                if (Convert.ToBoolean(builder.Configuration.GetSection("CreateDistricts").Value))
                {
                    var cityManager = serviceProvider.GetRequiredService<ICityManager>();
                    var districtManager = serviceProvider.GetRequiredService<IDistrictManager>();
                    createData.SaveAllDistricttoDBViaExcel(districtManager,cityManager);
                }
                #endregion
            }

            LoggerManager logmanager = new LoggerManager();
            logmanager.LogMessage(LoggerLevel.Info, "****Program başladı*****");


            app.Run(); // app çalıştırır.
        }
    }
}