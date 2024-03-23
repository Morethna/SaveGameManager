using SaveGameManager.Core;
using SaveGameManager.Interfaces;
using SaveGameManager.Models;
using SaveGameManager.Services;
using SaveGameManager.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SaveGameManager.Viewmodels;
public class ProfileDialogViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private ISettingsService _settingsService;
    private readonly IDirectoryService _directoryService;
    private readonly IWindowService _windowService;
    private readonly TextDialogViewModel _textDialog;
    private readonly NotifyBoxYesNoViewModel _notifyBoxYesNo;
    private Profile? _selectedProfile;

    public ProfileDialogViewModel(IDataService dataService,
        ISettingsService settingsService,
        IDirectoryService directoryService,
        IWindowService windowService,
        TextDialogViewModel textDialog,
        NotifyBoxYesNoViewModel notifyBoxYesNo)
    {
        _settingsService = settingsService;
        _dataService = dataService;
        _directoryService = directoryService;
        _windowService = windowService;
        _textDialog = textDialog;
        _notifyBoxYesNo = notifyBoxYesNo;

        BrowseCommand = new DelegateCommand(Browse);
        AddProfileCommand = new DelegateCommand(AddProfile);
        EditProfileCommand = new DelegateCommand(EditProfile);
        DeleteProfileCommand = new DelegateCommand(DeleteProfile);
        AddExistingProfileCommand = new DelegateCommand(AddExistingProfile);
    }

    public ICommand BrowseCommand { get; set; }
    public ICommand AddProfileCommand { get; set; }
    public ICommand AddExistingProfileCommand { get; set; }
    public ICommand EditProfileCommand { get; set; }
    public ICommand DeleteProfileCommand { get; set; }
    public ObservableCollection<Profile> Profiles
    {
        get => _dataService.Config.Profiles;
    }

    public Profile? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (_selectedProfile == value)
                return;

            _selectedProfile = value;
            OnPropertyChanged(nameof(SelectedProfile));
        }
    }
    public ISettingsService SettingsService
    {
        get => _settingsService;
        set
        {
            if (value == _settingsService)
                return;

            _settingsService = value;
            OnPropertyChanged(nameof(SettingsService));
        }
    }

    public string Gamepath
    {
        get => _dataService.Config.Gamepath;
        set
        {
            if (_dataService.Config.Gamepath == value)
                return;

            _dataService.Config.Gamepath = value;
            OnPropertyChanged(nameof(Gamepath));
        }
    }

    private void Browse(object obj)
    {
        var tmpPath = _windowService.OpenFolderWindow(Gamepath);

        if (string.IsNullOrWhiteSpace(tmpPath)) return;

        Gamepath = tmpPath;
        SettingsService.ProfileUiEnabled = true;
    }

    private void AddProfile(object obj)
    {
        if (string.IsNullOrEmpty(Gamepath))
        {
            _windowService.NotifierWarning("Select a gamefolder, please");
            return;
        }           

        _textDialog.Name = string.Empty;
        _windowService.OpenWindowDialog(_textDialog, this);

        if (!string.IsNullOrEmpty(_textDialog.Name))
        {
            Profile profile = new() { Name = _textDialog.Name };
            _dataService.Config.Profiles.Add(profile);
            _dataService.SelectedProfile = profile;
            _dataService.SaveConfigAsync();
            _directoryService.CreateProfile(profile);
            SettingsService.MainUiEnabled = _dataService.SelectedProfile != null;
        }
    }
    private void AddExistingProfile(object obj)
    {
        if (string.IsNullOrEmpty(Gamepath))
        {
            _windowService.NotifierWarning("Select a gamefolder, please");
            return;
        }

        _directoryService.CheckExistingProfile();
    }
    private void EditProfile(object obj)
    {
        if (SelectedProfile is null)
        {
            _windowService.NotifierWarning("Select a profile, please.");
            return;
        }

        _textDialog.Name = SelectedProfile.Name;
        _windowService.OpenWindowDialog(_textDialog, this);

        if (!string.IsNullOrEmpty(_textDialog.Name) && _textDialog.Ok)
        {
            SelectedProfile.Name = _textDialog.Name;
            _dataService.SaveConfigAsync();
        }
    }
    private void DeleteProfile(object obj)
    {
        if (SelectedProfile is null)
        {
            _windowService.NotifierWarning("Select a profile, please.");
            return;
        }

        _notifyBoxYesNo.Message = $"Do you really want to delete the Profile \"{SelectedProfile.Name}\"";
        _notifyBoxYesNo.Title = "Delete Profile";
        _windowService.OpenWindowDialog(_notifyBoxYesNo, this);

        if (!_notifyBoxYesNo.Result) return;

        _directoryService.DeleteProfilePath(SelectedProfile);

        _dataService.Config.Profiles.Remove(SelectedProfile);

        if (_dataService.Config.Profiles.Count > 0)
            _dataService.SelectedProfile ??= _dataService.Config.Profiles.First();
        else
            _dataService.SelectedProfile = null;

        SettingsService.MainUiEnabled = _dataService.SelectedProfile != null;

        _dataService.SaveConfigAsync();
        
    }
}
