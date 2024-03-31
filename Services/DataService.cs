using Newtonsoft.Json;
using SaveGameManager.Models;
using SaveGameManager.Interfaces;
using System;
using System.IO;
using System.Linq;
using SaveGameManager.Viewmodels;
using SaveGameManager.Core;
using System.Text.Json;

namespace SaveGameManager.Services;
public class DataService : IDataService
{
    private Config _config = new();
    private Profile? _selectedProfile;
    private readonly string _filePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\SaveGameManager\profile.json";
    private readonly string _path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\SaveGameManager";
    private readonly IWindowService _windowService;
    private readonly NotifyBoxViewModel _notifyBox;

    //private readonly ILogger<DataService> //_logger;

    #region ctor
    public DataService(ISettingsService settings, IWindowService windowService, NotifyBoxViewModel notifyBox)
    {
        Settings = settings;
        _windowService = windowService;
        _notifyBox = notifyBox;
        InitConfig();       
    }
    #endregion

    public Profile? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (_selectedProfile == value)
                return;

            _selectedProfile = value;

            if (_selectedProfile != null)
                Config.ActiveProfile = _selectedProfile.Id;
        }
    }
    public Savegame? SelectedSaveGame { get; set; }
    public Config Config { get => _config; }
    private ISettingsService Settings { get; }

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

            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(_filePath)) ?? new Config();

            if (_config.Profiles.Count > 0)
                _selectedProfile = _config.Profiles.Where(p => p.Id == _config.ActiveProfile).First();

            Settings.MainUiEnabled = _selectedProfile != null;
            Settings.ProfileUiEnabled = !string.IsNullOrWhiteSpace(_config.Gamepath);
        }
        catch (Exception ex)
        {
            _notifyBox.Message = $"Something went wrong, while initializing the profil \"{_filePath}\".\r\n{ex.Message}";
            _notifyBox.Title = "Error";
            _windowService.OpenWindow(_notifyBox);
        }
    }
    #endregion

    #region public methods
    public async void SaveConfigAsync() => await File.WriteAllTextAsync(_filePath, JsonConvert.SerializeObject(_config));
    #endregion
}
