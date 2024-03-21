using SaveGameManager.Core;
using SaveGameManager.Interfaces;
using SaveGameManager.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace SaveGameManager.Viewmodels;
public class ProfileDialogViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private readonly ISettingsService _settingsService;
    private readonly IDirectoryService _directoryService;
    private readonly IWindowService _windowService;
    private readonly TextDialogViewModel _textDialog;
    private Profile? _selectedProfile;

    public ProfileDialogViewModel(IDataService dataService,
        ISettingsService settingsService,
        IDirectoryService directoryService,
        IWindowService windowService,
        TextDialogViewModel textDialog)
    {
        _dataService = dataService;
        _settingsService = settingsService;
        _directoryService = directoryService;
        _windowService = windowService;
        _textDialog = textDialog;

        BrowseCommand = new DelegateCommand(Browse);
        AddProfileCommand = new DelegateCommand(AddProfile);
        EditProfileCommand = new DelegateCommand(EditProfile);
        DeleteProfileCommand = new DelegateCommand(DeleteProfile);
    }

    public ICommand BrowseCommand { get; set; }
    public ICommand AddProfileCommand { get; set; }
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

    public string Gamepath 
    { 
        get  => _dataService.Config.Gamepath;
        set 
        {
            if (_dataService.Config.Gamepath == value)
                return;

            _dataService.Config.Gamepath = value;
            OnPropertyChanged(nameof(Gamepath));
        } 
    }

    private void Browse (object obj)
    {
        var tmpPath = _windowService.OpenFolderWindow(Gamepath);

        if (!string.IsNullOrWhiteSpace(tmpPath))
            Gamepath = tmpPath;
    }

    private void AddProfile(object obj)
    {
        _textDialog.Name = string.Empty;
        _windowService.OpenWindowDialog(IWindowService.Windows.Textdialog, _textDialog, IWindowService.Windows.MainWindow);

        if (!string.IsNullOrEmpty(_textDialog.Name))
        {
            Profile profile = new() { Name = _textDialog.Name };
            _dataService.Config.Profiles.Add(profile);
            _dataService.SelectedProfile = profile;
            _dataService.SaveConfigAsync();
            _directoryService.CreateProfile(profile);
        }
    }

    private void EditProfile(object obj)
    {
        if (SelectedProfile is null)
        {
            _windowService.NotifierWarning("Select a profile, please.");
            return;
        }

        _textDialog.Name = SelectedProfile.Name;
        _windowService.OpenWindowDialog(IWindowService.Windows.Textdialog, _textDialog, IWindowService.Windows.ProfileDialog);

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
            MessageBox.Show("Select a profile, please.");
            return;
        }

        if (MessageBox.Show($"Do you really want to delete the Profile \"{SelectedProfile.Name}\"",
            "Delete Profile", MessageBoxButton.YesNo) == MessageBoxResult.No)
            return;

        _directoryService.DeleteProfilePath(SelectedProfile);

        _dataService.Config.Profiles.Remove(SelectedProfile);

        if (_dataService.SelectedProfile is null)
            _dataService.SelectedProfile = _dataService.Config.Profiles.First();

        _dataService.SaveConfigAsync();
        
    }
}
