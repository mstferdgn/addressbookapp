using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.PostalCodeApiModels
{
    public class UbilisimAPIResultModel
    {
        //    success": true,
        //"status": "ok",
        //"dataupdatedate": "2024-08-16",
        //"description": "PTT tarafından günlük olarak çekilerek güncellenen posta kodlarıdır.",
        //"pagecreatedate": "2024-08-16 00:00:00",
        //"postakodu": [
        //NOT: apideki response json keyleri küçük harfli biz classta propları büyük yazdık ve sorun olmadı jsonconvert ile deserialize edebildik.
        public bool Success { get; set; }
        public string Status { get; set; }
        public DateTime DataUpdateDate { get; set; }
        public DateTime PageCreatedate { get; set; }
        public string Description { get; set; }
        public List<UBilisimAPIPostalCodeResultModel> PostaKodu { get; set; }

    }
}
