using SaveGameManager.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SaveGameManager.Models;
public class Config : OberservableObject
{
    private string _gamepath = string.Empty;
    private string _activeProfile = string.Empty;

    public string Gamepath
    {
        get => _gamepath;
        set
        {
            if (_gamepath == value)
                return;

            _gamepath = value;
            OnPropertyChanged(nameof(Gamepath));
        }
    }
    public string ActiveProfile
    {
        get => _activeProfile;
        set
        {
            if (_activeProfile == value)
                return;

            _activeProfile = value;
            OnPropertyChanged(nameof(ActiveProfile));
        }
    }
    public ObservableCollection<Profile> Profiles { get; set; } = new ObservableCollection<Profile>();
}
