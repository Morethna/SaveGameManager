using SaveGameManager.Core;
using SaveGameManager.Interfaces;
using SaveGameManager.Models;
using System.Windows.Input;
using System;

namespace SaveGameManager.Viewmodels;
public class MainViewModel : ViewModelBase
{
    private Savegame? _selectedSaveGame;
    private Profile? _selectedProfile;
    private ISettingsService _settingsService;
    private readonly IDataService _dataService;
    private readonly IDirectoryService _directoryService;
    private readonly IWindowService _windowService;
    private readonly TextDialogViewModel _textDialog;
    private readonly ProfileDialogViewModel _profileDialog;
    private readonly AboutViewModel _aboutDialog;
    private readonly NotifyBoxYesNoViewModel _notifyBoxYesNo;
    private readonly SettingsDialogViewModel _settingsDialog;

    public MainViewModel(IDataService dataService, ISettingsService settingsService, IDirectoryService directoryService, IWindowService windowService,
        TextDialogViewModel textDialog, ProfileDialogViewModel profileDialog, AboutViewModel aboutDialog, NotifyBoxYesNoViewModel notifyBox, SettingsDialogViewModel settingsDialog) 
    {
        _dataService = dataService;
        _settingsService = settingsService;
        _directoryService = directoryService;
        _windowService = windowService;
        _textDialog = textDialog;
        _profileDialog = profileDialog;
        _aboutDialog = aboutDialog;
        _notifyBoxYesNo = notifyBox;
        _settingsDialog = settingsDialog;

        _selectedProfile = _dataService.SelectedProfile;
        _selectedSaveGame = _dataService.SelectedSaveGame;

        CreateSaveGameCommand = new DelegateCommand(ImportSaveGame);
        DeleteSaveGameCommand = new DelegateCommand(DeleteSaveGame);
        LoadSaveGameCommand = new DelegateCommand(LoadSaveGame);
        ReplaceSaveGameCommand = new DelegateCommand(ReplaceSaveGame);
        SelectedItemChangedCommand = new DelegateCommand(SelectedItemChanged);
        OpenSaveGameCommand = new DelegateCommand(OpenSaveGame);
        RenameSavegameCommand = new DelegateCommand(RenameSavegame);
        OpenProfileDialogCommand = new DelegateCommand(OpenProfileDialog);
        OpenAboutDialogCommand = new DelegateCommand(OpenAboutDialog);
        OpenSettingsDialogCommand = new DelegateCommand(OpenSettingsDialog);
        KeyDownCommand = new DelegateCommand(KeyDown);
        LoadProfileCommand = new DelegateCommand(LoadProfile);
    }

    private Savegame? SelectedSaveGame
    {
        get => _selectedSaveGame;
        set
        {
            if (_selectedSaveGame == value)
                return;
            _selectedSaveGame = value;
            _dataService.SelectedSaveGame = value;

            OnPropertyChanged(nameof(SelectedSaveGame));
        }
    }
    public Config Config { get => _dataService.Config; }

    public Profile? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (_selectedProfile == value)
                return;
            _selectedProfile = value;
            _dataService.SelectedProfile = value;

            if (_selectedProfile != null)
                _directoryService.LoadProfile(_selectedProfile);
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

    public ICommand CreateSaveGameCommand { get; set; }
    public ICommand DeleteSaveGameCommand { get; set; }
    public ICommand LoadSaveGameCommand { get; set; }
    public ICommand ReplaceSaveGameCommand { get; set; }
    public ICommand SelectedItemChangedCommand { get; set; }
    public ICommand OpenSaveGameCommand { get; set; }
    public ICommand RenameSavegameCommand { get; set; }
    public ICommand OpenProfileDialogCommand { get; set; }
    public ICommand OpenAboutDialogCommand { get; set; }
    public ICommand KeyDownCommand { get; set; }
    public ICommand LoadProfileCommand { get; set; }
    public ICommand OpenSettingsDialogCommand { get; set; }



    private void SelectedItemChanged(object obj) => SelectedSaveGame = (Savegame)obj;
    private void ImportSaveGame(object obj) =>_directoryService.CreateSaveGame(SelectedProfile);
    private void LoadSaveGame(object obj) => _directoryService.LoadSaveGame(SelectedSaveGame);
    private void LoadProfile(object obj) => _directoryService.LoadProfile(SelectedProfile);
    private void ReplaceSaveGame(object obj)
    {
        if (SelectedSaveGame == null) return;
        var repSG = SelectedSaveGame.Name;

        try
        {
            _notifyBoxYesNo.Title = "Replace Savegame";
            _notifyBoxYesNo.Message = $"Do you really want to replace \"{repSG}\"?";
            _windowService.OpenWindowDialog(_notifyBoxYesNo, this);

            if (!_notifyBoxYesNo.Result) return;

            _directoryService.ReplaceSavegame(SelectedSaveGame);
            _windowService.NotifierSuccess($"\"{repSG}\" has been replaced");
        }
        catch (Exception ex)
        {
            _windowService.NotifierError($"Something went wrong, while replacing the Savegame \"{repSG}\".\r\n{ex.Message}");
        }
}
    private void DeleteSaveGame(object? obj)
    {
        if (SelectedSaveGame == null) return;
        var delSG = SelectedSaveGame.Name;

        try
        {
            _notifyBoxYesNo.Title = "Delete Savegame";
            _notifyBoxYesNo.Message = $"Do you really want to delete \"{delSG}\"?";
            _windowService.OpenWindowDialog(_notifyBoxYesNo, this);

            if (!_notifyBoxYesNo.Result) return;
            
            _directoryService.DeleteSaveGame(SelectedSaveGame);
            SelectedProfile?.SaveGames.Remove(SelectedSaveGame);

            _windowService.NotifierSuccess($"\"{delSG}\" has been deleted.");
        }
        catch (Exception ex)
        {
            _windowService.NotifierError($"Something went wrong, while trying to delete the Savegame '{delSG}' from the filesystem.\r\n{ex.Message}");
        }
    }
    private void OpenSaveGame(object obj) => _directoryService.OpenSaveGame(SelectedSaveGame);
    private void RenameSavegame(object obj)
    {
        if (SelectedSaveGame == null) return;

        _textDialog.Name = SelectedSaveGame.Name;
        _windowService.OpenWindowDialog(_textDialog, this);

        if (!string.IsNullOrEmpty(_textDialog.Name) && _textDialog.Ok)
            _directoryService.RenameSaveGameFolder(SelectedSaveGame, _textDialog.Name);
            
    }
    private void OpenProfileDialog(object obj)
    {
        _windowService.OpenWindowDialog(_profileDialog, this);
        SelectedProfile = _dataService.SelectedProfile;
    }
    private void OpenAboutDialog(object obj) => _windowService.OpenWindowDialog(_aboutDialog, this);
    private void OpenSettingsDialog(object obj) => _windowService.OpenWindowDialog(_settingsDialog, this);

    private void KeyDown(object obj)
    {
        var key = (KeyEventArgs)obj;
        if (key.Key == Key.Delete)
            DeleteSaveGame(null);
    }
}
