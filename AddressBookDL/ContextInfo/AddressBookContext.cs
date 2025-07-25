using AddressBookEL.Entities;
using AddressBookEL.IdentityModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookDL.ContextInfo
{
    public class AddressBookContext:IdentityDbContext<AppUser,AppRole,string>
    {
        public AddressBookContext()
        {
                
        }

        public AddressBookContext(DbContextOptions<AddressBookContext> options)
       : base(options)
        {
        }

        public virtual DbSet<City> CityTable { get; set; }
        public virtual DbSet<District> DistrictTable { get; set; }
        public virtual DbSet<Neigborhood> NeigborhoodTable { get; set; }
        public virtual DbSet<UserAddress> UserAddressTable { get; set; }

      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<District>().ToTable("DISTRICT");
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }


            base.OnModelCreating(modelBuilder);
        }
    }
}
