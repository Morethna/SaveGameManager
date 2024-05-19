using SaveGameManager.Core;
using SaveGameManager.Interfaces;
using SaveGameManager.Models;
using System.Windows.Input;
using System;
using NHotkey.Wpf;
using NHotkey;
using System.Linq;
using SaveGameManager.Enums;
using System.Collections.ObjectModel;
using SaveGameManager.Views;

namespace SaveGameManager.Viewmodels;
public class MainViewModel : ViewModelBase
{
    private string _filter = string.Empty;
    private Profile? _selectedProfile;
    private Savegame? _selectedSavegame;
    private IUiSettingsService _settingsService;
    private readonly IDataService _dataService;
    private readonly IDirectoryService _directoryService;
    private readonly IWindowService _windowService;
    private readonly TextDialogViewModel _textDialog;
    private readonly ProfileDialogViewModel _profileDialog;
    private readonly AboutViewModel _aboutDialog;
    private readonly NotifyBoxYesNoViewModel _notifyBoxYesNo;
    private readonly SettingsDialogViewModel _settingsDialog;
    private readonly NotifyBoxViewModel _errNotify;

    public MainViewModel(IDataService dataService, IUiSettingsService settingsService, IDirectoryService directoryService, IWindowService windowService,
        TextDialogViewModel textDialog, ProfileDialogViewModel profileDialog, AboutViewModel aboutDialog, NotifyBoxYesNoViewModel notifyBox, SettingsDialogViewModel settingsDialog,
        NotifyBoxViewModel errNotify)
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
        _errNotify = errNotify;

        _selectedProfile = _dataService.SelectedProfile;
        _selectedSavegame = _dataService.SelectedSaveGame;

        CreateSaveGameCommand = new DelegateCommand(ImportSaveGame);
        DeleteSaveGameCommand = new DelegateCommand(DeleteSaveGame);
        LoadSaveGameCommand = new DelegateCommand(LoadSaveGame);
        ReplaceSaveGameCommand = new DelegateCommand(ReplaceSaveGame);
        OpenSaveGameCommand = new DelegateCommand(OpenSaveGame);
        RenameSavegameCommand = new DelegateCommand(RenameSavegame);
        OpenProfileDialogCommand = new DelegateCommand(OpenProfileDialog);
        OpenAboutDialogCommand = new DelegateCommand(OpenAboutDialog);
        OpenSettingsDialogCommand = new DelegateCommand(OpenSettingsDialog);
        KeyDownCommand = new DelegateCommand(KeyDown);
        LoadCommand = new DelegateCommand(Load);
        SortCommand = new DelegateCommand(Sort);
    }
    public static Array SortEnumArray
    {
        get => Enum.GetValues(typeof(SortEnum));
    }

    public string Filter
    {
        get => _filter;
        set
        {
            if (_filter == value)
                return;
            _filter = value;
            FilterSavegames(_filter);
            OnPropertyChanged(nameof(Filter));
        }
    }

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
    public Savegame? SelectedSaveGame
    {
        get => _selectedSavegame;
        set
        {
            if (_selectedSavegame == value)
                return;
            _selectedSavegame = value;
            _dataService.SelectedSaveGame = value;

            OnPropertyChanged(nameof(SelectedSaveGame));
        }
    }

    public Config Config { get => _dataService.Config; }

    public IUiSettingsService SettingsService
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
    public ICommand OpenSaveGameCommand { get; set; }
    public ICommand RenameSavegameCommand { get; set; }
    public ICommand OpenProfileDialogCommand { get; set; }
    public ICommand OpenAboutDialogCommand { get; set; }
    public ICommand KeyDownCommand { get; set; }
    public ICommand LoadCommand { get; set; }
    public ICommand OpenSettingsDialogCommand { get; set; }
    public ICommand SortCommand { get; set; }
    public ICommand FilterCommand { get; set; }


    private void ImportSaveGame(object obj)
    {
        _directoryService.CreateSaveGame(SelectedProfile);
        FilterSavegames(Filter);
    }
    private void LoadSaveGame(object obj) => _directoryService.LoadSaveGame(SelectedSaveGame);
    private void Load(object obj)
    {

        _directoryService.LoadProfile(SelectedProfile);
        SetGlobalHotkeys();
    }
    private void ReplaceSaveGame(object obj)
    {
        if (SelectedSaveGame == null)
        {
            _windowService.NotifierWarning($"Select a savegame");
            return;
        }

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
    private void OpenSettingsDialog(object obj)
    {
        _windowService.OpenWindowDialog(_settingsDialog, this);
        SetGlobalHotkeys();
    }
    private void KeyDown(object obj)
    {
        var key = (KeyEventArgs)obj;
        if (key.Key == Key.Delete)
            DeleteSaveGame(null);
    }
    private void Sort(object obj)
    {
        if (SelectedProfile is not null)
            SelectedProfile.SaveGames = new ObservableCollection<Savegame>(_directoryService.SortSavegames(SelectedProfile.SaveGames));
    }
    private void FilterSavegames(string filter)
    {
        if (SelectedProfile is null) return;

        _directoryService.LoadProfile(SelectedProfile);

        if (string.IsNullOrEmpty(filter)) return;

        var sgList = new ObservableCollection<Savegame>(SelectedProfile.SaveGames.Where(x => x.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase)));
        SelectedProfile.SaveGames = sgList;
    }
    internal void HotkeyImport(object obj, HotkeyEventArgs e)
    {
        if (e.Name is "Import")
            ImportSaveGame(obj);
        e.Handled = true;
    }
    internal void HotkeyLoad(object obj, HotkeyEventArgs e)
    {
        if (e.Name is "Load")
            LoadSaveGame(obj);
        e.Handled = true;
    }
    internal void HotkeyNext(object obj, HotkeyEventArgs e)
    {
        e.Handled = true;

        if (e.Name is not "Next")
            return;

        if (SelectedProfile is null || SelectedProfile.SaveGames.Count is 0)
            return;

        if (SelectedSaveGame is null)
        {
            SelectedSaveGame = SelectedProfile.SaveGames.First();
            return;
        }
        var index = SelectedProfile.SaveGames.IndexOf(SelectedSaveGame);
        if (index == SelectedProfile.SaveGames.Count - 1) return;

        SelectedSaveGame = SelectedProfile.SaveGames[index + 1];
    }
    internal void HotkeyPrev(object obj, HotkeyEventArgs e)
    {
        e.Handled = true;

        if (e.Name is not "Prev")
            return;

        if (SelectedProfile is null || SelectedProfile.SaveGames.Count is 0) return;

        if (SelectedSaveGame is null)
        {
            SelectedSaveGame = SelectedProfile.SaveGames.First();
            return;
        }
        var index = SelectedProfile.SaveGames.IndexOf(SelectedSaveGame);
        if (index == 0) return;

        SelectedSaveGame = SelectedProfile.SaveGames[index - 1];

    }
    internal void SetGlobalHotkeys()
    {
        Hotkey SetSingleHotkey(Hotkey hotkey, string name, EventHandler<HotkeyEventArgs> eventArgs)
        {
            try
            {
                if (hotkey.Key is Key.None || Config.Settings.GlobalHotkeys is false)
                    HotkeyManager.Current.Remove(name);
                else
                    HotkeyManager.Current.AddOrReplace(name, hotkey.Key, hotkey.Modifiers, eventArgs);

                return hotkey;
            }
            catch (HotkeyAlreadyRegisteredException ex)
            {
                _errNotify.Message = $"Something went wrong, while trying to add \"{hotkey}\" to \"{name}\".\r\n{ex.Message}";
                _errNotify.Title = "Error";
                _windowService.OpenWindow(_errNotify);
                return new();
            }
        }

        Config.Settings.Import = SetSingleHotkey(Config.Settings.Import, "Import", HotkeyImport);
        Config.Settings.Load = SetSingleHotkey(Config.Settings.Load, "Load", HotkeyLoad);
        Config.Settings.Next = SetSingleHotkey(Config.Settings.Next, "Next", HotkeyNext);
        Config.Settings.Prev = SetSingleHotkey(Config.Settings.Prev, "Prev", HotkeyPrev);

    }
}
