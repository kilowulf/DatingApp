using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")] // account/register
        // video 012 Adding the create token logic
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // check if user exists
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
            // create instance of HMACSHA512
            using var hmac = new HMACSHA512();
            // create new user
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            // save user to context
            _context.Users.Add(user);
            // save user to DB
            await _context.SaveChangesAsync();
            // return user
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")] // account/login
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // Check DB for user
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
            // If user not found, return Unauthorized
            if (user == null) return Unauthorized("Invalid username");

            // If user found, verify password
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDto.Password));
            // Compare computedHash to stored hash
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }
            // If password matches, return user
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        // Helper method to check if user exists
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
        
    }
}