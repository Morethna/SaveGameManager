using SaveGameManagerMVVM.Models;

namespace SaveGameManagerMVVM.Interfaces;
public interface IDataService
{
    public Config Config { get; }
    public Profile SelectedProfile { get; set; }
    public Savegame? SelectedSaveGame { get; set; }
    public void SaveConfigAsync();
    public void SetGamefolder(string gamePath);
    public void EditProfile(Profile profile);
    public void DeleteProfile(Profile profile);
}
