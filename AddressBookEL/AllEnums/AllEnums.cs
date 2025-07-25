using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.AllEnums
{
    public class AllEnums
    {
    }

    public enum Roles
    {
        MEMBER=2,
        ADMIN=1,
        DELETED=3,
        WAITforVALIDATION=4
    }

    public enum LoggerLevel
    {
        Info,
        Error
    }
}
