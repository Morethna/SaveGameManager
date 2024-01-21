using SaveGameManagerMVVM.Models;

namespace SaveGameManagerMVVM.Interfaces;
public interface IDirectoryService
{
    public string GameFolder { get; set; }
    public void DeleteProfilePath(Profile profile);
    public void CreateSaveGame(Profile profile);
    public void OpenSaveGame(Savegame savegame);
    public void DeleteSaveGame(Savegame savegame);
    public void LoadSaveGame(Savegame savegame);
    public void RenameSaveGameFolder(Savegame savegame, string newName);
    public void LoadProfile(Profile profile);
    public void ReplaceSavegame(Savegame savegame);
}
