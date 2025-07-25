using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookBL.ImplementationofManagers
{
    public class GeneralManager
    {

        public static string ReplaceTurkishCharactesTo(string txt)
        {
            string returnTxt = txt.Replace("ü", "u")
                                 .Replace("Ü", "U")
                                 .Replace("ş", "s")
                                 .Replace("ö", "o")
                                 .Replace("ğ", "g")
                                 .Replace("İ", "I")
                                 .Replace("ı", "i")
                                 .Replace(" ", "")
                                 .Replace("ç", "c");
            return returnTxt;
        }

        public static string GetTurkishWeekNamefromWeekDay(DayOfWeek dofw)
        {
            switch (dofw)
            {
                case DayOfWeek.Sunday:
                    return "Pazar";
                   
                case DayOfWeek.Monday:
                    return "Pazartesi";

                case DayOfWeek.Tuesday:
                    return "Salı";

                case DayOfWeek.Wednesday:
                    return "Çarşamba";
                case DayOfWeek.Thursday:
                    return "Perşembe";

                case DayOfWeek.Friday:
                    return "Cuma";
                case DayOfWeek.Saturday:
                    return "Cumartesi";
                default:
                    return string.Empty;
            }
        }
    }
}
