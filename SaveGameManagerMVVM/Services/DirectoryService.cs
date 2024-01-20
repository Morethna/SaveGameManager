using SaveGameManagerMVVM.Models;
using SaveGameManagerMVVM.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SaveGameManager.Handler
{
    public class DirectoryService : IDirectoryService
    {
        private string _gameFolder = string.Empty;

        #region ctor
        public DirectoryService(){}
        #endregion

        #region public properties
        public string GameFolder { get => _gameFolder; set => _gameFolder = value; }
        #endregion

        #region private properties
        private string SaveGameFolder { get => Path.Combine(_gameFolder, "SaveGameManager"); }
        #endregion

        #region private methode
        private void CleanUpSavegame(string folder)
        {
            if (Directory.Exists(folder))
            {
                foreach (var filename in Directory.GetFiles(folder))
                {
                    File.Delete(filename);
                }
            }
        }

        #endregion

        #region public methods
        public void DeleteProfilePath(Profile profile)
        {
            try
            {
                var path = Path.Combine(SaveGameFolder, profile.Id);
                if (Directory.Exists(path))
                {
                    foreach (var sg in profile.SaveGames)
                        DeleteSaveGame(sg);

                    Directory.Delete(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong, while trying to delete profile '{profile.Name}' on the filesystem.\r\n{ex.Message}");
            }
        }
        public void CreateSaveGameFolder(Profile profile, string name)
        {
            try
            {
                var saveGamePath = Path.Combine(SaveGameFolder, profile.Id, name);

                Directory.CreateDirectory(saveGamePath);

                foreach (var filename in Directory.GetFiles(_gameFolder))
                {
                    var dstFilename = Path.Combine(saveGamePath, filename.Replace(_gameFolder, saveGamePath));
                    File.Copy(filename, dstFilename);
                }
                profile.SaveGames.Add(new Savegame { Name = name, Path = saveGamePath });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong, while trying to create the Savegame '{name}' on the filesystem.\r\n{ex.Message}");
            }
        }
        public void DeleteSaveGame(Savegame savegame)
        {
            try
            {
                if (Directory.Exists(savegame.Path))
                {
                    foreach (string file in Directory.GetFiles(savegame.Path))
                    {
                        File.Delete(file);
                    }
                    Directory.Delete(savegame.Path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong, while trying to delete the Savegame '{savegame.Name}' from the filesystem.\r\n{ex.Message}");
            }
        }
        public void LoadSaveGame(Savegame savegame)
        {
            try
            {
                CleanUpSavegame(_gameFolder);
                if (Directory.Exists(savegame.Path))
                {
                    foreach (var filename in Directory.GetFiles(savegame.Path))
                    {
                        var dstFilename = Path.Combine(_gameFolder, filename.Replace($@"{savegame.Path}\", ""));
                        File.Copy(filename, dstFilename, true);
                    }
                }
                else
                    throw new DirectoryNotFoundException($"Directory {savegame.Path} was not found");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong, while loading the Savegame '{savegame.Name}'.\r\n{ex.Message}");
            }
        }
        public void RenameSaveGameFolder(Savegame savegame, string newName)
        {
            try
            {
                var newPath = savegame.Path.Replace(savegame.Name, newName);

                if (Directory.Exists(newPath))
                {
                    MessageBox.Show("This savegame already exists.");
                    return;
                }
                if (Directory.Exists(savegame.Path))
                {
                    Directory.Move(savegame.Path, newPath);
                }
                savegame.Path = newPath;
                savegame.Name = newName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong, while rename the Savegame '{savegame.Name}'.\r\n{ex.Message}");
            }
        }
        public void LoadProfile(Profile profile)
        {
            try
            {
                var saveGamePath = Path.Combine(SaveGameFolder, profile.Id);
                if (Directory.Exists(saveGamePath))
                {
                    foreach (string d in Directory.GetDirectories(saveGamePath))
                    {
                        var p = profile.SaveGames.Where(x => x.Path == d);
                        if (p.Count() == 0)
                            profile.SaveGames.Add(new Savegame { Name = d.Replace($@"{saveGamePath}\", ""), Path = d });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong, while loading the profile '{profile.Name}'.\r\n{ex.Message}");
            }
        }
        public void ReplaceSavegame(Savegame savegame)
        {
            try
            {
                CleanUpSavegame(savegame.Path);
                if (Directory.Exists(savegame.Path))
                {
                    foreach (var filename in Directory.GetFiles(_gameFolder))
                    {
                        var dstFilename = filename.Replace(_gameFolder, savegame.Path);
                        File.Copy(filename, dstFilename, true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong, while replace savegame '{savegame.Name}'.\r\n{ex.Message}");
            }
        }
        #endregion
    }
}
