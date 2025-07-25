using AddressBookAPI.JWTProcess;
using AddressBookBL.EmailSenderProcess;
using AddressBookBL.ImplementationofManagers;
using AddressBookBL.InterfacesOfManagers;
using AddressBookDL.ContextInfo;
using AddressBookEL.IdentityModels;
using AddressBookEL.Mappings;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace AddressBookAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
            //automapper

            builder.Services.AddAutoMapper(opt =>
            {
                opt.AddExpressionMapping();
                opt.AddProfile(typeof(Maps));
            });

            builder.Services.AddSingleton<ITokenManager>(new TokenManager(builder.Configuration));
            builder.Services.AddScoped<IUserAddressManager, UserAddressManager>();


            //JWT için Eklenecek olan satırlar
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["JWTSettings:ValidIssuer"],
                    ValidAudience = builder.Configuration["JWTSettings:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:Secret"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true, // tokenın süresinin dolup dolmadığını kontrol ediyor.
                    ValidateIssuerSigningKey = true,
                    ClockSkew=TimeSpan.FromSeconds(60)
                };
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
