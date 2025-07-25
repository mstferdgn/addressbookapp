using AddressBookEL.AllEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookBL.InterfacesOfManagers
{
  public interface ILoggerManager
    {
        public void LogMessage(LoggerLevel level, params string[] messages);
    }
}
