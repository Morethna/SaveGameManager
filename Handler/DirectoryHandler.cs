using SaveGameManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SaveGameManager.Handler
{
  public class DirectoryHandler
  {
    private string _gameFolder;
    private string _saveGameFolder;
    public DirectoryHandler(string gameFolder)
    {
      _gameFolder = gameFolder;
      _saveGameFolder = Path.Combine(_gameFolder, "SaveGameManager");
    }

    public string GameFolder { set => _gameFolder = value; }

    public void DeleteProfilePath(Profile profile)
    {
      var path = Path.Combine(_saveGameFolder, profile.Id);
      if (Directory.Exists(path))
        Directory.Delete(path);
    }

    public void CreateSaveGameFolder(Profile profile, string name)
    {
      var saveGamePath = Path.Combine(_saveGameFolder, profile.Id, name);
      profile.SaveGames.Add(new Savegame { Name = name, Path = saveGamePath }) ;

      Directory.CreateDirectory(saveGamePath);

      foreach (var filename in Directory.GetFiles(_gameFolder))
      {
        var dstFilename = Path.Combine(saveGamePath, filename.Replace(_gameFolder, saveGamePath));
        File.Copy(filename, dstFilename);
      }
    }

    public void LoadSaveGame(Profile profile)
    {
      var saveGamePath = Path.Combine(_saveGameFolder, profile.Id, profile.Name);
      if (Directory.Exists(saveGamePath))
      {
        foreach (var filename in Directory.GetFiles(saveGamePath))
        {
          var dstFilename = Path.Combine(_gameFolder, filename.Replace(saveGamePath, ""));
          File.Copy(filename, dstFilename);
        }
      }
      else
        throw new DirectoryNotFoundException($"Directory {saveGamePath} was not found"); 
    }

    public void DeleteSaveGameFolder(Profile profile)
    {
      var path = Path.Combine(_saveGameFolder, profile.Id, profile.Name);
      if (Directory.Exists(path))
        Directory.Delete(path);
    }

    public void RenameSaveGameFolder(Profile profile, string newName)
    {
      var oldPath = Path.Combine(_saveGameFolder, profile.Id, profile.Name);
      var newPath = Path.Combine(_saveGameFolder, profile.Id, newName);
      if (Directory.Exists(oldPath))
        Directory.Move(oldPath, newPath);
    }

    public void LoadProfile(Profile profile)
    {
      var saveGamePath = Path.Combine(_saveGameFolder, profile.Id);
      if (Directory.Exists(saveGamePath))
      {
        foreach(string d in Directory.GetDirectories(saveGamePath))
        {
          profile.SaveGames.Add(new Savegame { Name = d.Replace($@"{saveGamePath}\", ""), Path = saveGamePath });
        }
      }
    }
  }
}
