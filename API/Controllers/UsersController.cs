using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() 
    {
        var users = await userRepository.GetMembersAsync();

          return Ok(users);
    }

    [HttpGet("{userName}")]
    public  async Task<ActionResult<MemberDto>> GetUser(string userName) 
    {
        var user = await userRepository.GetMemberAsync(userName);
        if (user == null) return NotFound();

        return user;
    }
}
