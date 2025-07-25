using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.JWTModels
{
    public class UserLoginResponse
    {
        public bool AuthenticateResult { get; set; }
        public string AuthToken { get; set; }
        public string? Description { get; set; }
        public DateTime AccessTokenExpireDate { get; set; }
    }
}
