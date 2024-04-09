using SaveGameManager.Models;

namespace SaveGameManager.Interfaces;
public interface IDataService
{
    public Config Config { get; }
    public Profile? SelectedProfile { get; set; }
    public Savegame? SelectedSaveGame { get; set; }
    public void SaveConfigAsync();
}
