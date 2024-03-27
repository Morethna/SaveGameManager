using SaveGameManager.Core;
using System.Text;
using System.Windows.Input;

namespace SaveGameManager.Viewmodels;

public class SettingsDialogViewModel : ViewModelBase
{
    private Hotkey? _hotkey;
    public SettingsDialogViewModel()
    {
        HotkeyTextBoxCommand = new DelegateCommand(HotkeyTextBoxPreviewKeyDown);
    }

    private Hotkey? Hotkey 
    {
        get => _hotkey;
        set
        {
            if (_hotkey == value)
                return;

            _hotkey = value;
            OnPropertyChanged(nameof(Hotkey));
        }
    }

    public ICommand HotkeyTextBoxCommand { get; set; }

    private void HotkeyTextBoxPreviewKeyDown(object obj)
    {
        // Don't let the event pass further because we don't want
        // standard textbox shortcuts to work.
        var e = (KeyEventArgs)obj;
        e.Handled = true;

        // Get modifiers and key data
        var modifiers = Keyboard.Modifiers;
        var key = e.Key;

        // When Alt is pressed, SystemKey is used instead
        if (key == Key.System)
        {
            key = e.SystemKey;
        }

        // Pressing delete, backspace or escape without modifiers clears the current value
        if (modifiers == ModifierKeys.None &&
            (key == Key.Delete || key == Key.Back || key == Key.Escape))
        {
            Hotkey = null;
            return;
        }

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
            return;
        }

        // Update the value
        Hotkey = new Hotkey(key, modifiers);
    }
}

public class Hotkey(Key key, ModifierKeys modifiers)
{
    public Key Key { get; } = key;

    public ModifierKeys Modifiers { get; } = modifiers;

    public string Text
    {
        get 
        {
            var str = new StringBuilder();

            if (Modifiers.HasFlag(ModifierKeys.Control))
                str.Append("Ctrl + ");
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                str.Append("Shift + ");
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                str.Append("Alt + ");
            if (Modifiers.HasFlag(ModifierKeys.Windows))
                str.Append("Win + ");

            str.Append(Key);

            return str.ToString();
        }
    }
}
