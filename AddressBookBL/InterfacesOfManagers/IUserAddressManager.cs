using AddressBookEL.Entities;
using AddressBookEL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookBL.InterfacesOfManagers
{
    public interface IUserAddressManager:IBaseManager<UserAddress, UserAddressVM, int>
    {
        public ChartViewModel BolgelereGoreAdresSayilari(string labelName, params byte[] platecodes);
    }
}
