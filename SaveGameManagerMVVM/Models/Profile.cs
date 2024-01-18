using System;
using System.Collections.ObjectModel;

namespace SaveGameManagerMVVM.Models;
public class Profile : ProfileBase
{
    public ObservableCollection<Savegame> SaveGames { get; set; } = new ObservableCollection<Savegame>();
}
