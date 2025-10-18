using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.interfaces
{
    // No internal logic: only contains signatures of functionality the interface provides.
    public interface ITokenService
    {
        string CreateToken(AppUser user);
        
    }
}