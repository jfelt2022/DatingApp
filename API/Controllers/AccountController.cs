using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Data.Migrations;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService, 
        IMapper mapper) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
        {
            try
            {
                if (await UserExists(registerDto.Username)) return BadRequest("User name exists.  Try again.");

                var user = mapper.Map<AppUser>(registerDto);

                user.UserName = registerDto.Username.ToLower();

                var results = await userManager.CreateAsync(user, registerDto.Password);
                
                if (!results.Succeeded) return BadRequest(results.Errors);

                return Ok(new UserDto{
                    Username = user.UserName,
                    Token = await tokenService.CreateToken(user), 
                    KnownAs = user.KnownAs
                });
            }
            catch (Exception ex)
            {                
                throw new Exception("4787a82d-3777-4fd2-8029-8e6538dc5435 " + ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) 
        {
            var rUser = await userManager.Users.Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.NormalizedUserName == loginDto.Username.ToUpper());

            if (rUser == null) return Unauthorized("Invalid username or password");

            if (rUser.UserName == null) throw new Exception("No username for user");
            
            return new UserDto
            {
                Username = rUser.UserName,
                Token = await tokenService.CreateToken(rUser),
                PhotoUrl = rUser.Photos.FirstOrDefault(x => x.IsMain)?.Url, 
                KnownAs = rUser.KnownAs
            };
        }

        private async Task<bool> UserExists(string username) 
        {
            if (username == null) throw new Exception("username is blank");
            return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
        }
    }

}
