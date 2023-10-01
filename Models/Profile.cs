using System;
using System.Collections.Generic;

namespace SaveGameManager.Models
{
  public class Profile
  {
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string CreationTime { get; set; } = DateTime.Now.ToString();
    public List<Savegame> SaveGames { get; set; } = new List<Savegame>();
  }
}
