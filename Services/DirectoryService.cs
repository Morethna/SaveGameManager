﻿using SaveGameManager.Models;
using SaveGameManager.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using SaveGameManager.Viewmodels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SaveGameManager.Services;

namespace SaveGameManager.Handler;
public class DirectoryService(IDataService dataService, IWindowService windowService, ISettingsService settings, NotifyBoxViewModel notifyBox) : IDirectoryService
{
    private static readonly Random random = new();

    #region private properties
    private string SaveGameFolder { get => Path.Combine(GameFolder, "SaveGameManager"); }
    private string GameFolder { get => dataService.Config.Gamepath; }
    private IWindowService WindowService { get; } = windowService;
    private ISettingsService Settings { get; } = settings;
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

    public void CheckExistingProfile()
    {
        try
        {
            if (!Directory.Exists(SaveGameFolder))
            {
                WindowService.NotifierWarning($"Savegamefolder \"{SaveGameFolder}\" not found");
                return;
            }
            var tmpPath = WindowService.OpenFolderWindow(SaveGameFolder);

            if (string.IsNullOrEmpty(tmpPath)) return;

            if (!Path.Exists(tmpPath))
            {
                WindowService.NotifierWarning($"Profilepath \"{tmpPath}\" not found");
                return;
            }

            var directories = Directory.GetDirectories(SaveGameFolder).Where(x => x == tmpPath).FirstOrDefault();

            if (directories is null)
            {
                WindowService.NotifierWarning($"Profilepath isn't a subfolder of the SaveGameFolder \"{tmpPath}\"");
                return;
            }

            var id = tmpPath.Replace(@$"{SaveGameFolder}\", "");

            if (dataService.Config.Profiles.Any(x => x.Id == id))
            {
                WindowService.NotifierWarning($"The profile \"{id}\" is already in the profilelist");
                return;
            }
            var name = $"Imported_{RandomString(5)}";
            var profil = new Profile
            {
                CreationTime = Directory.GetCreationTime(tmpPath).ToString(),
                Id = id,
                Name = name
            };

            dataService.Config.Profiles.Add(profil);
            dataService.SelectedProfile = profil;
            Settings.MainUiEnabled = dataService.SelectedProfile != null;

            WindowService.NotifierSuccess($"\"{name}\" imported.");
        }
        catch (Exception ex)
        {
            notifyBox.Message = $"Something went wrong, while linking an existing profile.\r\n{ex.Message}";
            notifyBox.Title = "Error";
            WindowService.OpenWindow(notifyBox);
        }
    }

    public void DeleteProfilePath(Profile profile)
    {
        try
        {
            var path = Path.Combine(SaveGameFolder, profile.Id);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
        catch (Exception ex)
        {
            notifyBox.Message = $"Something went wrong, while deleting profile \"{profile.Name}\".\r\n{ex.Message}";
            notifyBox.Title = "Error";
            WindowService.OpenWindow(notifyBox);
        }
    }
    public void CreateSaveGame(Profile profile)
    {
        try
        {
            if (string.IsNullOrEmpty(GameFolder))
            {
                WindowService.NotifierWarning("Select a gamefolder, please");
                return;
            }

            var name = RandomString(8);

            var saveGamePath = Path.Combine(SaveGameFolder, profile.Id, name);
            Directory.CreateDirectory(saveGamePath);

            foreach (var filename in Directory.GetFiles(GameFolder))
            {
                var dstFilename = Path.Combine(saveGamePath, filename.Replace(GameFolder, saveGamePath));
                File.Copy(filename, dstFilename);
            }
            profile.SaveGames.Add(new Savegame { Name = name, Path = saveGamePath });

            WindowService.NotifierSuccess($"Imported savegame");
        }
        catch (Exception ex)
        {
            notifyBox.Message = $"Something went wrong, while creating a new savegame.\r\n{ex.Message}";
            notifyBox.Title = "Error";
            WindowService.OpenWindow(notifyBox);
       }     
    }
    public void CreateProfile(Profile profile)
    {
        try
        {
            if (string.IsNullOrEmpty(GameFolder))
            {
                WindowService.NotifierWarning("Select a gamefolder, please");
                return;
            }

            var profilePath = Path.Combine(SaveGameFolder, profile.Id);
            Directory.CreateDirectory(profilePath);

        }
        catch (Exception ex)
        {
            notifyBox.Message = $"Something went wrong, while creating the profile \"{profile.Name}\".\r\n{ex.Message}";
            notifyBox.Title = "Error";
            WindowService.OpenWindow(notifyBox);
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
            notifyBox.Message = $"Something went wrong, while deleting the savegame \"{savegame.Name}\".\r\n{ex.Message}";
            notifyBox.Title = "Error";
            WindowService.OpenWindow(notifyBox);
        }
    }
    public void LoadSaveGame(Savegame savegame)
    {
        try
        {
            CleanUpSavegame(GameFolder);
            if (Directory.Exists(savegame.Path))
            {
                foreach (var filename in Directory.GetFiles(savegame.Path))
                {
                    var dstFilename = Path.Combine(GameFolder, filename.Replace($@"{savegame.Path}\", ""));
                    File.Copy(filename, dstFilename, true);
                }
                WindowService.NotifierSuccess($"\"{savegame.Name}\" has been loaded.");
            }
            else
                throw new DirectoryNotFoundException($"Directory {savegame.Path} was not found");
        }
        catch (Exception ex)
        {
            notifyBox.Message = $"Something went wrong, while loading the savegame \"{savegame.Name}\".\r\n{ex.Message}";
            notifyBox.Title = "Error";
            WindowService.OpenWindow(notifyBox);
        }
    }
    public void RenameSaveGameFolder(Savegame savegame, string newName)
    {
        try
        {
            var newPath = savegame.Path.Replace(savegame.Name, newName);

            if (Directory.Exists(newPath))
            {
                WindowService.NotifierWarning("This savegame already exists.");
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
            notifyBox.Message = $"Something went wrong, while renaming the Savegame \"{newName}\".\r\n{ex.Message}";
            notifyBox.Title = "Error";
            WindowService.OpenWindow(notifyBox);
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
            notifyBox.Message = $"Something went wrong, while loading the profile \"{profile.Name}\".\r\n{ex.Message}";
            notifyBox.Title = "Error";
            WindowService.OpenWindow(notifyBox);
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
                WindowService.NotifierWarning($"Savegame doesn't exist on the filesystem'. Path: \"{savegame.Path}\"");
                return;
            }

            Process.Start("explorer.exe", savegame.Path);
        }
        catch (Exception ex)
        {
            notifyBox.Message = $"Something went wrong, while trying to open the savegame folder \"{savegame.Path}\".\r\n{ex.Message}";
            notifyBox.Title = "Error";
            WindowService.OpenWindow(notifyBox);
        }
    }
    #endregion
}
