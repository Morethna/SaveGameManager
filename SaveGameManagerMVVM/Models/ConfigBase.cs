using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaveGameManagerMVVM.Models;
public class ConfigBase
{
    public string Gamepath { get; set; } = string.Empty;
    public string ActiveProfile { get; set; } = string.Empty;
    public List<ProfileBase> Profiles { get; set; } = new List<ProfileBase>();
}
