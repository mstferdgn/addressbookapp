﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.IdentityModels
{
    public class AppRole : IdentityRole
    {

        public string? Description { get; set; }
    }
}
