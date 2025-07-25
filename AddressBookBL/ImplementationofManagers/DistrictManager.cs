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
    public class DistrictManager:BaseManager<District, DistrictDTO,int>,IDistrictManager
    {
        public DistrictManager(AddressBookContext ctx, IMapper mapper):base(ctx, mapper,new string[] {"City"})
        {
                
        }
    }
}
