using System;
using API.Entities;

namespace API.DTOs;

public class ClientDto
{
    public int Id { get; set; }
    public required string ClientName { get; set; }
    public ICollection<AppUser>? ClientUsers { get; internal set; }
}
