using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddressBookEL.IdentityModels;
using AddressBookEL.JWTModels;

namespace AddressBookAPI.JWTProcess
{
    public interface ITokenManager
    {
        public Task<UserLoginResponse> GenerateToken(AppUser user);
    }
}
