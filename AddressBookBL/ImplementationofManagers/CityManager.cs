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
    public class CityManager:BaseManager<City,CityDTO, byte>,ICityManager
    {
        public CityManager(AddressBookContext ctx, IMapper mapper):base(ctx,mapper)
        {
                
        }
    }
}
