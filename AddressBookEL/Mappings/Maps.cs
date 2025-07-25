using AddressBookEL.Entities;
using AddressBookEL.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.Mappings
{
    public class Maps:Profile
    {
        public Maps()
        {
            CreateMap<City, CityDTO>().ReverseMap(); 
            CreateMap<District, DistrictDTO>().ReverseMap(); 
            CreateMap<UserAddress, UserAddressVM>().ReverseMap(); 
            CreateMap<Neigborhood, NeigborhoodVM>().ReverseMap(); 
        }
    }
}
