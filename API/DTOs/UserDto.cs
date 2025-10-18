using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    // video 013 Creating a user DTO and returning the token
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}