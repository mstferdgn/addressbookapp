using AddressBookBL.InterfacesOfManagers;
using AddressBookDL.ContextInfo;
using AddressBookEL.Entities;
using AddressBookEL.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookBL.ImplementationofManagers
{
    public class UserAddressManager:BaseManager<UserAddress,UserAddressVM, int>,IUserAddressManager
    {

        public UserAddressManager(AddressBookContext ctx,IMapper mapper):base(ctx,mapper, new string[] { "City", "District" , "Neigborhood","User" })
        {
                
        }

        public ChartViewModel BolgelereGoreAdresSayilari(string labelName,params byte[] platecodes)
        {
            try
            {
                ChartViewModel region = new ChartViewModel();
                region.LabelName = labelName;
               
           
                var cities = (from c in _context.CityTable
                             where platecodes.Contains(Convert.ToByte(c.PlateCode))
                             select c.Id).ToList();

                //select * from Useradres where CityId in(34,....)
                region.LabelValue = this.GetSomeAll(x => cities.Contains(x.CityId),joinTables:null).Data.Count();
                return region;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
