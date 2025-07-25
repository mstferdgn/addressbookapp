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
    public class NeigborhoodManager : BaseManager<Neigborhood, NeigborhoodVM, int>, INeigborhoodManager
    {
        public NeigborhoodManager(AddressBookContext ctx, IMapper mapper) : base(ctx, mapper, new string[] { "City", "District" })
        {
            {

            }
        }
    }
}
