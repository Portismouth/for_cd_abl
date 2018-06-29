using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace TheWall.Models
{
  public class User : IdentityUser
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Message> Messages { get; set; }

    public User ()
    {
      Messages = new List<Message> ();
    }
  }
}