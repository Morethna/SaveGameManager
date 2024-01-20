using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace SaveGameManagerMVVM.Models;
public class Profile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string CreationTime { get; set; } = DateTime.Now.ToString();
    [JsonIgnore]
    public ObservableCollection<Savegame> SaveGames { get; set; } = new ObservableCollection<Savegame>();
}
