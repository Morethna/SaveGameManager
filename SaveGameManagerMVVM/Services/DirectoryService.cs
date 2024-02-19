using SaveGameManagerMVVM.Models;
using SaveGameManagerMVVM.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using SaveGameManagerMVVM.Viewmodels;

namespace SaveGameManager.Handler
{
    public class DirectoryService : IDirectoryService
    {
        private static Random random = new Random();
        private readonly IDataService _dataService;
        private readonly IWindowService _windowService;

        #region ctor
        public DirectoryService(IDataService dataService, IWindowService windowService)
        {
            _dataService = dataService;
            _windowService = windowService;
        }
        #endregion

        #region private properties
        private string SaveGameFolder { get => Path.Combine(GameFolder, "SaveGameManager"); }
        private string GameFolder { get => _dataService.Config.Gamepath; }
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

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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
        public void CreateSaveGame(Profile profile)
        {
            var name = RandomString(8);

            if (string.IsNullOrEmpty(GameFolder))
            {
                MessageBox.Show("Select a gamefolder, please");
                return;
            }

            var saveGamePath = Path.Combine(SaveGameFolder, profile.Id, name);
            Directory.CreateDirectory(saveGamePath);

            foreach (var filename in Directory.GetFiles(GameFolder))
            {
                var dstFilename = Path.Combine(saveGamePath, filename.Replace(GameFolder, saveGamePath));
                File.Copy(filename, dstFilename);
            }
            profile.SaveGames.Add(new Savegame { Name = name, Path = saveGamePath });
        }
        public void CreateProfile(Profile profile)
        {
            try
            {
                if (string.IsNullOrEmpty(GameFolder))
                {
                    MessageBox.Show("Select a gamefolder, please");
                    return;
                }

                var profilePath = Path.Combine(SaveGameFolder, profile.Id);
                Directory.CreateDirectory(profilePath);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong, while trying to create the Profile '{profile.Name}' on the filesystem.\r\n{ex.Message}");
            }
        }
        public void DeleteSaveGame(Savegame savegame)
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
        public void LoadSaveGame(Savegame savegame)
        {
            CleanUpSavegame(GameFolder);
            if (Directory.Exists(savegame.Path))
            {
                foreach (var filename in Directory.GetFiles(savegame.Path))
                {
                    var dstFilename = Path.Combine(GameFolder, filename.Replace($@"{savegame.Path}\", ""));
                    File.Copy(filename, dstFilename, true);
                }
            }
            else
                throw new DirectoryNotFoundException($"Directory {savegame.Path} was not found");

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
                if (profile is null)
                    return;

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
            CleanUpSavegame(savegame.Path);
            if (Directory.Exists(savegame.Path))
            {
                foreach (var filename in Directory.GetFiles(GameFolder))
                {
                    var dstFilename = filename.Replace(GameFolder, savegame.Path);
                    File.Copy(filename, dstFilename, true);
                }
            }
        }
        public void OpenSaveGame(Savegame savegame)
        {
            try
            {
                if (!Directory.Exists(savegame.Path))
                {
                    MessageBox.Show($"Savegame doesn't exist on the filesystem'. Path: \"{savegame.Path}\"");
                    return;
                }

                Process.Start("explorer.exe", savegame.Path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong, while trying to add a profile to select the gamefolder'.\r\n{ex.Message}");
            }
        }
        #endregion
    }
}
