using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
using System.Windows.Input;

namespace SaveGameManager.Models;

public class Hotkey
{
    [JsonConverter(typeof(StringEnumConverter))]
    public Key Key { get; }
    [JsonConverter(typeof(StringEnumConverter))]
    public ModifierKeys Modifiers { get; }

    [JsonConstructor]
    public Hotkey(Key key, ModifierKeys modifiers)
    {
        Key = key;
        Modifiers = modifiers;
    }
    public Hotkey() { }

    public override string ToString()
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
