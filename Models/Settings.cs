using System.Collections.Generic;

namespace SaveGameManager.Models;
public class Settings
{
    public bool StaysOnTop { get; set; } = false;
    public bool GlobalHotkeys { get; set; } = false;
    public bool CheckUpdates { get; set; } = true;
    public Dictionary<string, Hotkey> Hotkeys { get; set; } = [];
}
