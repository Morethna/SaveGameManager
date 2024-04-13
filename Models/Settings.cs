namespace SaveGameManager.Models;
public class Settings
{
    public bool StaysOnTop { get; set; } = false;
    public bool GlobalHotkeys { get; set; } = false;
    public bool CheckUpdates { get; set; } = true;
    public Hotkey Import { get; set; } = new();
    public Hotkey Load { get; set; } = new();
    public Hotkey Next { get; set; } = new();
    public Hotkey Prev { get; set; } = new();
}
