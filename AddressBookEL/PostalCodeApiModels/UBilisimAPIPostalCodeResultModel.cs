using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.PostalCodeApiModels
{
    public class UBilisimAPIPostalCodeResultModel
    {
        //"plaka": 34,
        //    "il": "İSTANBUL",
        //    "ilce": "ADALAR",
        //    "semt_bucak_belde": "BURGAZADA",
        //    "mahalle": "BURGAZADA MAH",
        //    "pk": "34975"

        public int Plaka { get; set; }
        public string Il { get; set; }
        public string Ilce { get; set; }
        public string Semt_Bucak_Belde { get; set; }
        public string Mahalle { get; set; }
        public string Pk { get; set; }
    }
}
