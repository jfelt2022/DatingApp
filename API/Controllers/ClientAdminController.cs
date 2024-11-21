using System;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using API.Data;

namespace API.Controllers;
public class ClientAdminController(UserManager<AppUser> _userManager) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("admin-users")]
    public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetAdminUsers() 
    {
        var users = await _userManager.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.Any(r => 
                r.Role.NormalizedName == "ADMIN" || 
                r.Role.NormalizedName == "CLIENTADMIN"))
            .OrderBy(u => u.UserName)
            .Select(u => new AdminUserDto
            {
                Id = u.Id,
                Username = u.UserName,
                Roles = u.UserRoles
                    .Select(r => r.Role.Name).ToList()
            })
            .ToListAsync();
        
        return Ok(users);
    }

    [HttpPost("create-client")]
    public async Task<ActionResult<ClientDto>> CreateClient(
        [FromServices] DataContext context, 
        [FromBody] CreateClientDto createClientDto)
    {
        if (context.Clients != null && await context.Clients.AnyAsync(c => c.ClientName == createClientDto.ClientName))
            return BadRequest($"Client Name {createClientDto.ClientName} already exists!");

        var newClient = new Client
        {
            ClientName = createClientDto.ClientName
        };

        await context.Clients!.AddAsync(newClient);
        await context.SaveChangesAsync();
        
        return Ok(new ClientDto {
            Id = newClient.Id,
            ClientName = newClient.ClientName
        });
    }

    // DTOs for better separation of concerns
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public List<string?>? Roles { get; set; }
    }
}
