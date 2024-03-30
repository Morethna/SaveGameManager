using NHotkey;
using SaveGameManager.Core;
using SaveGameManager.Interfaces;
using SaveGameManager.Models;
using System;
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
    private string _strLoadHotkey = None;
    private string _strImportHotkey = None;
    private string _strNextHotkey = None;
    private string _strRevHotkey = None;

    private Hotkey _load = new();
    private Hotkey _import = new();
    private Hotkey _next = new();
    private Hotkey _prev = new();

    public SettingsDialogViewModel(IWindowService windowService)
    {
        SetHotkeyCommand = new DelegateCommand(SetHotkey);
        LostFocusCommand = new DelegateCommand(LostFocus);
        GotFocusCommand = new DelegateCommand(GotFocus);
        _windowService = windowService;
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

    private Hotkey Load
    {
        get => _load;
        set
        {
            if (_load == value) return;
            _load = value;

            var str = _load.ToString();
            LoadHotkey = string.IsNullOrEmpty(str) ? None : str;
        }
    }
    private Hotkey Import
    {
        get => _import;
        set
        {
            if (_import == value) return;
            _import = value;

            var str = _import.ToString();
            ImportHotkey = string.IsNullOrEmpty(str) ? None : str;
        }
    }
    private Hotkey Next
    {
        get => _next;
        set
        {
            if (_next == value) return;
            _next = value;

            var str = _next.ToString();
            NextHotkey = string.IsNullOrEmpty(str) ? None : str;
        }
    }
    private Hotkey Prev
    {
        get => _prev;
        set
        {
            if (_prev == value) return;
            _prev = value;

            var str = _prev.ToString();
            PrevHotkey = string.IsNullOrEmpty(str) ? None : str;
        }
    }

    public ICommand SetHotkeyCommand { get; set; }
    public ICommand GotFocusCommand { get; set; }
    public ICommand LostFocusCommand { get; set; }

    private void SetHotkey(object obj)
    {
        if (obj is KeyboardEventArgs args)
        {
            var tb = GetTextboxName(args);
            _ = tb switch
            {
                TBImport => Import = HotkeyKeyDown((KeyEventArgs)obj, Import),
                TBNext => Next = HotkeyKeyDown((KeyEventArgs)obj, Next),
                TBPrev => Prev = HotkeyKeyDown((KeyEventArgs)obj, Prev),
                TBLoad => Load = HotkeyKeyDown((KeyEventArgs)obj, Load),
                _ => throw new NotImplementedException()
            };
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
                TBImport => Import.Key is Key.None ? ImportHotkey = None : ImportHotkey = Import.ToString(),
                TBNext => Next.Key is Key.None ? NextHotkey = None : NextHotkey = Next.ToString(),
                TBPrev => Prev.Key is Key.None ? PrevHotkey = None : PrevHotkey = Prev.ToString(),
                TBLoad => Load.Key is Key.None ? LoadHotkey = None : LoadHotkey = Load.ToString(),
                _ => throw new NotImplementedException()
            }; ;
        }
    }
    private static string GetTextboxName(RoutedEventArgs eventArgs)
    {
        if (eventArgs.OriginalSource is TextBox tb)
            return tb.Name;
        return "";
    }


    private Hotkey HotkeyKeyDown(KeyEventArgs e, Hotkey hotkey)
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
            return hotkey;
        }
        if (hotkey.Key == key && hotkey.Modifiers == modifiers)
            return hotkey;

        var newHK = new Hotkey(key, modifiers);

        if (!CheckHotkey(newHK))
        {
            _windowService.NotifierWarning($"Hotkey \"{newHK}\" is already in use.");
            return hotkey;
        }

        // Update the value
        return newHK;
    }

    private bool CheckHotkey(Hotkey hotkey)
    {
        if ((hotkey.Key == Import.Key && hotkey.Modifiers == Import.Modifiers) ||
            (hotkey.Key == Next.Key && hotkey.Modifiers == Next.Modifiers) ||
            (hotkey.Key == Prev.Key && hotkey.Modifiers == Prev.Modifiers) ||
            (hotkey.Key == Load.Key && hotkey.Modifiers == Load.Modifiers))
            return false;
        return true;
    }
}
