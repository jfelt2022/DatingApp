using System;

namespace API.Entities;

public class UserClient
{
    public required int AppUserId { get; set; }
    public required int ClientId { get; set; }
}
