using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class Client
{
    public int Id { get; set; }
    public required string ClientName { get; set; } 

    public ICollection<AppUser> AppUsers { get; set; } = [];

}
