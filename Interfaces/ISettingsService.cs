namespace SaveGameManager.Interfaces;

public interface ISettingsService
{
    public bool ProfileUiEnabled { get; set; }
    public bool MainUiEnabled { get; set; }
}
