using System;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using API.Data;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;

namespace API.Controllers;
public class ClientAdminController(UserManager<AppUser> userManager) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("admin-users")]
    public async Task<ActionResult> GetAdminUsers() 
    {
        var users = await userManager.Users
            .OrderBy(x => x.UserName)
            .Where(x => x.UserRoles.Any(y => y.Role.NormalizedName == "ADMIN") || x.UserRoles.Any(y => y.Role.NormalizedName == "CLIENTADMIN"))
            .Select(x => new
            {
                x.Id,
                Username = x.UserName,
                Roles = x.UserRoles
                    .Select(r => r.Role.Name)
                    .ToList()
            })
            .ToListAsync();
        
        return Ok(users);
    }

    [HttpPost("create-client")]
    public async Task<ActionResult<ClientDto>> CreateClient(DataContext context, CreateClientDto createClientDto)
    {
        var rClient = await context.Clients
            .FirstOrDefaultAsync(x => x.ClientName == createClientDto.ClientName);

        if (rClient != null)
			return BadRequest("Client Name " +  createClientDto.ClientName + " already exists!");

        rClient = new Client
        {
            ClientName = createClientDto.ClientName
        };
        await context.Clients.AddAsync(rClient);
        await context.SaveChangesAsync();
        
        var results = new ClientDto{
            Id = rClient.Id,
            ClientName = rClient.ClientName,
            ClientUsers = rClient.AppUsers
        };

        return Ok(results);
    }
}
