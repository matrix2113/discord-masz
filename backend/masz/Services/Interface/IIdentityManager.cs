﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Services
{
    public interface IIdentityManager
    {
        Task<Identity> GetIdentity(HttpContext httpContext);
    }
}
