﻿using Newtonsoft.Json;
using SaveGameManager.Core;
using System;
using System.Collections.ObjectModel;

namespace SaveGameManager.Models;
public class Profile : OberservableObject
{
    private string _name = string.Empty;
    private ObservableCollection<Savegame> _savegames = [];
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name
    {
        get => _name;
        set
        {
            if (_name == value) 
                return;
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }
    public string CreationTime { get; set; } = DateTime.Now.ToString();
    [JsonIgnore]
    public ObservableCollection<Savegame> SaveGames
    {
        get => _savegames;
        set
        {
            if (_savegames == value)
                return;
            _savegames = value;
            OnPropertyChanged(nameof(SaveGames));
        }
    }
}
