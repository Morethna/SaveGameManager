using NHotkey;
using NHotkey.Wpf;
using SaveGameManager.Core;
using SaveGameManager.Interfaces;
using SaveGameManager.Models;
using System;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SaveGameManager.Viewmodels;

public class SettingsDialogViewModel : ViewModelBase
{
    const string None = "None";
    const string SetYourHotkey = "Set your Hotkey...";

    const string TBImport = "tbImport";
    const string TBNext = "tbNext";
    const string TBPrev = "tbPrev";
    const string TBLoad = "tbLoad";

    private readonly IWindowService _windowService;
    private readonly IDirectoryService _directoryService;
    private readonly IDataService _dataService;

    private string _strLoadHotkey = None;
    private string _strImportHotkey = None;
    private string _strNextHotkey = None;
    private string _strRevHotkey = None;

    public SettingsDialogViewModel(IWindowService windowService, IDataService dataService, IDirectoryService directoryService)
    {
        SetImportHotkeyCommand = new DelegateCommand(SetImportHotkey);
        SetLoadHotkeyCommand = new DelegateCommand(SetLoadHotkey);
        SetNextHotkeyCommand = new DelegateCommand(SetNextHotkey);
        SetPrevHotkeyCommand = new DelegateCommand(SetPrevHotkey);
        LostFocusCommand = new DelegateCommand(LostFocus);
        GotFocusCommand = new DelegateCommand(GotFocus);

        _windowService = windowService;
        _directoryService = directoryService;
        _dataService = dataService;

        ImportHotkey = HotkeyText(Settings.Import);
        NextHotkey = HotkeyText(Settings.Next);
        PrevHotkey = HotkeyText(Settings.Prev);
        LoadHotkey = HotkeyText(Settings.Load);
    }
    public Settings Settings
    {
        get => _dataService.Config.Settings;
        private set
        {
            if (_dataService.Config.Settings == value)
                return;
            _dataService.Config.Settings = value;
        }
    }
    public string LoadHotkey
    {
        get => _strLoadHotkey;
        set
        {
            if (_strLoadHotkey == value)
                return;

            _strLoadHotkey = value;
            OnPropertyChanged(nameof(LoadHotkey));
        }
    }
    public string ImportHotkey
    {
        get => _strImportHotkey;
        set
        {
            if (_strImportHotkey == value)
                return;

            _strImportHotkey = value;
            OnPropertyChanged(nameof(ImportHotkey));
        }
    }
    public string NextHotkey
    {
        get => _strNextHotkey;
        set
        {
            if (_strNextHotkey == value)
                return;

            _strNextHotkey = value;
            OnPropertyChanged(nameof(NextHotkey));
        }
    }
    public string PrevHotkey
    {
        get => _strRevHotkey;
        set
        {
            if (_strRevHotkey == value)
                return;

            _strRevHotkey = value;
            OnPropertyChanged(nameof(PrevHotkey));
        }
    }

    public ICommand SetImportHotkeyCommand { get; set; }
    public ICommand SetLoadHotkeyCommand { get; set; }
    public ICommand SetNextHotkeyCommand { get; set; }
    public ICommand SetPrevHotkeyCommand { get; set; }
    public ICommand GotFocusCommand { get; set; }
    public ICommand LostFocusCommand { get; set; }

    private void SetImportHotkey(object obj)
    {
        if (obj is KeyEventArgs args)
        {
            var hk = HotkeyKeyDown(args);
            if (hk is null) return;

            Settings.Import = hk;
            ImportHotkey = HotkeyText(hk);
        }
    }
    private void SetLoadHotkey(object obj)
    {
        if (obj is KeyEventArgs args)
        {
            var hk = HotkeyKeyDown(args);
            if (hk is null) return;

            Settings.Load = hk;
            LoadHotkey = HotkeyText(hk);
        }
    }
    private void SetNextHotkey(object obj)
    {
        if (obj is KeyEventArgs args)
        {
            var hk = HotkeyKeyDown(args);
            if (hk is null) return;

            Settings.Next = hk;
            NextHotkey = HotkeyText(hk);
        }
    }
    private void SetPrevHotkey(object obj)
    {
        if (obj is KeyEventArgs args)
        {
            var hk = HotkeyKeyDown(args);
            if (hk is null) return;

            Settings.Prev = hk;
            PrevHotkey = HotkeyText(hk);
        }
    }
    private void GotFocus(object obj)
    {
        if (obj is RoutedEventArgs args)
        {
            var tb = GetTextboxName(args);
            _ = tb switch
            {
                TBImport => ImportHotkey = SetYourHotkey,
                TBNext => NextHotkey = SetYourHotkey,
                TBPrev => PrevHotkey = SetYourHotkey,
                TBLoad => LoadHotkey = SetYourHotkey,
                _ => throw new NotImplementedException()
            };
        }
    }
    private void LostFocus(object obj)
    {
        if (obj is RoutedEventArgs args)
        {
            var tb = GetTextboxName(args);
            _ = tb switch
            {
                TBImport => ImportHotkey = HotkeyText(Settings.Import),
                TBNext => NextHotkey = HotkeyText(Settings.Next),
                TBPrev => PrevHotkey = HotkeyText(Settings.Prev),
                TBLoad => LoadHotkey = HotkeyText(Settings.Load),
                _ => throw new NotImplementedException()
            }; ;
        }
    }

    internal static string HotkeyText(Hotkey hotkey)
    {
        var strHK = hotkey.ToString();
        if (string.IsNullOrEmpty(strHK)) return None;
        else return strHK;
    }
    internal static string GetTextboxName(RoutedEventArgs eventArgs)
    {
        if (eventArgs.OriginalSource is TextBox tb)
            return tb.Name;
        return "";
    }
    internal Hotkey? HotkeyKeyDown(KeyEventArgs e)
    {
        // Don't let the event pass further because we don't want
        // standard textbox shortcuts to work.
        e.Handled = true;
        
        // Get modifiers and key data
        var modifiers = Keyboard.Modifiers;
        var key = e.Key;

        // When Alt is pressed, SystemKey is used instead
        if (key == Key.System)
            key = e.SystemKey;

        // Pressing delete, backspace or escape without modifiers clears the current value
        if (modifiers == ModifierKeys.None && (key is Key.Delete or Key.Back or Key.Escape))
            return new();

        // If no actual key was pressed - return
        if (key == Key.LeftCtrl ||
            key == Key.RightCtrl ||
            key == Key.LeftAlt ||
            key == Key.RightAlt ||
            key == Key.LeftShift ||
            key == Key.RightShift ||
            key == Key.LWin ||
            key == Key.RWin ||
            key == Key.Clear ||
            key == Key.OemClear ||
            key == Key.Apps)
        {
            return null;
        }
        var newHK = new Hotkey(key, modifiers);
        if (!CheckHotkey(newHK))
        {
            _windowService.NotifierWarning($"Hotkey \"{newHK}\" is already in use.");
            return null;
        }
        // Update the value
        return newHK;
    }
    internal bool CheckHotkey(Hotkey hotkey)
    {
        if ((hotkey.Key == Settings.Import.Key && hotkey.Modifiers == Settings.Import.Modifiers) ||
            (hotkey.Key == Settings.Next.Key && hotkey.Modifiers == Settings.Next.Modifiers) ||
            (hotkey.Key == Settings.Prev.Key && hotkey.Modifiers == Settings.Prev.Modifiers) ||
            (hotkey.Key == Settings.Load.Key && hotkey.Modifiers == Settings.Load.Modifiers))
            return false;
        return true;
    }
}
