using System;

namespace Poke.Server.Data.Models;

public class User
{
    public int UserID { get; set; }
    public required string Name { get; set; }
}
