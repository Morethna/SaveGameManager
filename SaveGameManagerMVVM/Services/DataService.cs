using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SaveGameManagerMVVM.Models;
using SaveGameManagerMVVM.Core;
using SaveGameManagerMVVM.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace SaveGameManagerMVVM.Services;
public class DataService : OberservableObject, IDataService
{
    private Config _config = new Config();
    private Profile _selectedProfile = new Profile();
    private string _filePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SaveGameManager/profile.json";
    private string _path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SaveGameManager";
    private readonly IDirectoryService _directoryService;

    //private readonly ILogger<DataService> //_logger;

    #region ctor
    public DataService(IDirectoryService directoryService)
    {
        InitConfig();
        _directoryService = directoryService;
    }
    #endregion

    public Profile SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (_selectedProfile == value)
                return;

            _selectedProfile = value;
            Config.ActiveProfile = _selectedProfile.Id;
            OnPropertyChanged(nameof(SelectedProfile));
        }
    }

    public Config Config
    {
        get => _config;
    }

    #region internal methods
    internal void InitConfig()
    {
        try
        {
            //_logger.LogInformation("Initialize configfile.");

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
                //_logger.LogDebug($"Path for the config file has been created. Path: \"{_path}\"");
            }

            if (!File.Exists(_filePath))
            {
                SaveConfigAsync();
                //_logger.LogDebug($"Config file has been created. File: \"{_filePath}\"");
                return;
            }

            var text = File.ReadAllText(_filePath);
            var configBase = JsonConvert.DeserializeObject<ConfigBase>(text) ?? new ConfigBase();

            _config = new Config 
            {
                Gamepath = configBase.Gamepath,
                ActiveProfile = configBase.ActiveProfile
            };

            configBase.Profiles.ForEach(p =>
            {
                _config.Profiles.Add(new Profile
                {
                    Id = p.Id,
                    Name = p.Name,
                    CreationTime = p.CreationTime
                });
            });
        }
        catch (Exception ex)
        {
            //_logger.LogError($"Error while trying to initialize the configfile. File: \"{_filePath}\"", ex);
        }
    }
    #endregion

    #region public methods
    public async void SaveConfigAsync()
    {
        await File.WriteAllTextAsync(_filePath, JsonConvert.SerializeObject(_config));
    }

    public void SetGamefolder(string gamePath)
    {
        try
        {
            _config.Gamepath = gamePath;
            SaveConfigAsync();
            //_logger.LogDebug($"Gamepath has been changed. Path: Gamepath: \"{gamePath}\"");
        }
        catch (Exception ex)
        {
            //_logger.LogError($"Error while trying to set the gamepath. Gamepath: \"{gamePath}\"", ex);
        }
    }

    public void AddProfile(Profile profile)
    {
        try
        {
            _config.Profiles.Add(profile);
            SaveConfigAsync();
            //_logger.LogDebug($"Profile has been added. Profile: \"{profile.Name}\"");
        }
        catch (Exception ex)
        {
            //_logger.LogError($"Error while trying to add a Profile. Profile: \"{profile.Name}\"", ex);
        }
    }

    public void EditProfile(Profile profile)
    {
        try
        {
            var p = _config.Profiles.Where(p => p.Id == profile.Id).FirstOrDefault();

            if (p == null) return;

            _config.Profiles.Remove(p);
            _config.Profiles.Add(profile);
            SaveConfigAsync();
            //_logger.LogDebug($"Profile has been edited. Old: \"{p.Name}\" -> New: \"{profile.Name}\"");
        }
        catch (Exception ex)
        {
            //_logger.LogError($"Error while trying to edit a Profile. Profile: \"{profile.Name}\"", ex);
        }
    }

    public void DeleteProfile(Profile profile)
    {
        try
        {
            var p = _config.Profiles.Where(p => p.Id == profile.Id).FirstOrDefault();

            if (p == null) return;

            _config.Profiles.Remove(p);
            SaveConfigAsync();
            //_logger.LogDebug($"Profile has been edited. Old: \"{p.Name}\" -> New: \"{profile.Name}\"");
        }
        catch (Exception ex)
        {
            //_logger.LogError($"Error while trying to edit a Profile. Profile: \"{profile.Name}\"", ex);
        }
    }

    #endregion
}
