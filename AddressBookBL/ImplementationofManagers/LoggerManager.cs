using AddressBookBL.InterfacesOfManagers;
using AddressBookEL.AllEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookBL.ImplementationofManagers
{
    public class LoggerManager: ILoggerManager
    {

        string txtFileName = $"AddressBookProjLOG_{DateTime.Now.ToString("yyyyMMdd")}.txt";
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "LOGS");

        public void LogMessage(LoggerLevel level, params string[] messages)
        {
            try
            {

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                using (StreamWriter writer = new StreamWriter(Path.Combine(directoryPath, txtFileName), true))
                {
                    writer.AutoFlush = true;
                    foreach (var item in messages)
                    {
                        //writer.WriteLine($"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")} {item}");

                        writer.WriteLineAsync($"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")} {item}");
                    }
                  
                }


            }
            catch (Exception)
            {
                // mail atılabilir
                //dbye kayıt atılabilir
            }
        }



    }

   
}
