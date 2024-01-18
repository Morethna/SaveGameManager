using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaveGameManagerMVVM.Models;
public class Config
{
    public string Gamepath { get; set; } = string.Empty;
    public string ActiveProfile { get; set; } = string.Empty;
    public ObservableCollection<Profile> Profiles { get; set; } = new ObservableCollection<Profile>();
}
