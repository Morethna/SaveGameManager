using SaveGameManager.Core;
using SaveGameManager.Interfaces;

namespace SaveGameManager.Services;
public class UiSettingsService : OberservableObject, IUiSettingsService
{
    private bool _mainUiEnabled = false;
    private bool _profileUiEnabled = false;

    public UiSettingsService() { }
    public bool MainUiEnabled
    {
        get => _mainUiEnabled;
        set
        {
            if (_mainUiEnabled == value)
                return;

            _mainUiEnabled = value;
            OnPropertyChanged(nameof(MainUiEnabled));
        }
    }

    public bool ProfileUiEnabled
    {
        get => _profileUiEnabled;
        set
        {
            if (_profileUiEnabled == value)
                return;

            _profileUiEnabled = value;
            OnPropertyChanged(nameof(ProfileUiEnabled));
        }
    }
}
