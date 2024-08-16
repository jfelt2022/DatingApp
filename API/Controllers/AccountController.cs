using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Data.Migrations;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
        {
            try
            {
                if (await UserExists(registerDto.Username)) return BadRequest("User name exists.  Try again.");

                return Ok();
                // using var hmac = new HMACSHA512();
                // var user = new AppUser 
                // {
                //     UserName = registerDto.Username.ToLower(),
                //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                //     PasswordSalt = hmac.Key
                // };

                // await context.Users.AddAsync(user);
                // await context.SaveChangesAsync();
                // return Ok(new UserDto{
                //     Username = user.UserName,
                //     Token = tokenService.CreateToken(user)
                // });
            }
            catch (Exception ex)
            {                
                throw new Exception("4787a82d-3777-4fd2-8029-8e6538dc5435 " + ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) 
        {
            var rUser = await context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == loginDto.Username.ToLower());
            if (rUser == null) return Unauthorized("Invalid username or password");
            if (!rUser.PasswordHash.Any()) return Unauthorized("Invalid password hash");
            HMACSHA512 hMACSHA512 = new HMACSHA512(rUser.PasswordSalt);
            using var hmac = hMACSHA512;
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            if (rUser.PasswordHash.Any())
            {
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != rUser.PasswordHash[i]) return Unauthorized("Invalid password");
                
                }
            }

            return new UserDto
            {
                Username = rUser.UserName,
                Token = tokenService.CreateToken(rUser)
            };
        }

        private async Task<bool> UserExists(string username) 
        {
            return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }
    }

}
