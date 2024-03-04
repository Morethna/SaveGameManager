﻿using SaveGameManagerMVVM.Core;
using SaveGameManagerMVVM.Interfaces;
using System.Windows;
using SaveGameManagerMVVM.Models;
using System.Windows.Input;
using System;
using System.Diagnostics;

namespace SaveGameManagerMVVM.Viewmodels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private ISettingsService _settingsService;
        private readonly IDirectoryService _directoryService;
        private readonly IWindowService _windowService;
        private readonly TextDialogViewModel _textDialog;
        private readonly ProfileDialogViewModel _profileDialog;
        private readonly AboutViewModel _aboutDialog;
        private readonly NotificationBoxViewModel _notificationBox;
        private Profile _selectedProfile;

        public MainViewModel(IDataService dataService, ISettingsService settingsService, IDirectoryService directoryService, IWindowService windowService,
            TextDialogViewModel textDialog, ProfileDialogViewModel profileDialog, AboutViewModel aboutDialog, NotificationBoxViewModel notificationBox) 
        {
            _dataService = dataService;
            _settingsService = settingsService;
            _directoryService = directoryService;
            _windowService = windowService;
            _textDialog = textDialog;
            _profileDialog = profileDialog;
            _aboutDialog = aboutDialog;
            _notificationBox = notificationBox;

            _selectedProfile = _dataService.SelectedProfile;

            CreateSaveGameCommand = new DelegateCommand(ImportSaveGame);
            DeleteSaveGameCommand = new DelegateCommand(DeleteSaveGame);
            LoadSaveGameCommand = new DelegateCommand(LoadSaveGame);
            ReplaceSaveGameCommand = new DelegateCommand(ReplaceSaveGame);
            SelectedItemChangedCommand = new DelegateCommand(SelectedItemChanged);
            OpenSaveGameCommand = new DelegateCommand(OpenSaveGame);
            RenameSavegameCommand = new DelegateCommand(RenameSavegame);
            OpenProfileDialogCommand = new DelegateCommand(OpenProfileDialog);
            OpenAboutDialogCommand = new DelegateCommand(OpenAboutDialog);
            KeyDownCommand = new DelegateCommand(KeyDown);
            LoadProfileCommand = new DelegateCommand(LoadProfile);
        }

        public Savegame? SelectedSaveGame
        {
            get => _dataService.SelectedSaveGame;
            set => _dataService.SelectedSaveGame = value;
        }

        public ICommand CreateSaveGameCommand { get; set; }
        public ICommand DeleteSaveGameCommand { get; set; }
        public ICommand LoadSaveGameCommand { get; set; }
        public ICommand ReplaceSaveGameCommand { get; set; }
        public ICommand SelectedItemChangedCommand { get; set; }
        public ICommand OpenSaveGameCommand { get; set; }
        public ICommand RenameSavegameCommand { get; set; }
        public ICommand OpenProfileDialogCommand { get; set; }
        public ICommand OpenAboutDialogCommand { get; set; }
        public ICommand KeyDownCommand { get; set; }
        public ICommand LoadProfileCommand { get; set; }

        public Profile SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (_selectedProfile == value)
                    return;

                _selectedProfile = value;
                _dataService.SelectedProfile = _selectedProfile;

                _directoryService.LoadProfile(_selectedProfile);
                OnPropertyChanged(nameof(SelectedProfile));
            }
        }
        public Config Config { get => _dataService.Config; }
        public ISettingsService SettingsService
        {
            get => _settingsService;
            set
            {
                if (value == _settingsService)
                    return;

                _settingsService = value;
                OnPropertyChanged(nameof(SettingsService));
            }
        }

        private void SelectedItemChanged(object obj) => SelectedSaveGame = (Savegame)obj;
        private void ImportSaveGame(object obj)
        {
            try
            {
                _directoryService.CreateSaveGame(SelectedProfile);
                _windowService.NotifierSuccess($"Imported savegame");
            }
            catch (Exception ex)
            {
                _windowService.NotifierError($"Something went wrong, while trying to create a new Savegame.\r\n{ex.Message}");
            }
        }
        private void LoadSaveGame(object obj)
        {
            try
            {
                if (SelectedSaveGame == null) return;

                _directoryService.LoadSaveGame(SelectedSaveGame);
                _windowService.NotifierSuccess($"\"{SelectedSaveGame.Name}\" has been loaded.");
            }
            catch (Exception ex)
            {
                _windowService.NotifierError($"Something went wrong, while loading the Savegame \"{SelectedSaveGame?.Name}\".\r\n{ex.Message}");
            }
        }
        private void LoadProfile(object obj) => _directoryService.LoadProfile(SelectedProfile);
        private void ReplaceSaveGame(object obj)
        {
            if (SelectedSaveGame == null) return;
            var repSG = SelectedSaveGame.Name;

            try
            {
                _notificationBox.Title = "Replace Savegame";
                _notificationBox.Message = $"Do you really want to replace \"{repSG}\"?";
                _windowService.OpenWindowDialog(IWindowService.Windows.NotificationBox, _notificationBox, IWindowService.Windows.MainWindow);

                if (!_notificationBox.Result) return;

                _directoryService.ReplaceSavegame(SelectedSaveGame);
                _windowService.NotifierSuccess($"\"{repSG}\" has been replaced");

            }
            catch (Exception ex)
            {
                _windowService.NotifierError($"Something went wrong, while trying to replace the Savegame \"{repSG}\".\r\n{ex.Message}");
            }
}
        private void DeleteSaveGame(object? obj)
        {
            if (SelectedSaveGame == null) return;
            var delSG = SelectedSaveGame.Name;

            try
            {
                _notificationBox.Title = "Delete Savegame";
                _notificationBox.Message = $"Do you really want to delete \"{delSG}\"?";
                _windowService.OpenWindowDialog(IWindowService.Windows.NotificationBox, _notificationBox, IWindowService.Windows.MainWindow);

                if (!_notificationBox.Result) return;
                
                _directoryService.DeleteSaveGame(SelectedSaveGame);
                SelectedProfile.SaveGames.Remove(SelectedSaveGame);

                _windowService.NotifierSuccess($"\"{delSG}\" has been deleted.");
            }
            catch (Exception ex)
            {
                _windowService.NotifierError($"Something went wrong, while trying to delete the Savegame '{delSG}' from the filesystem.\r\n{ex.Message}");
            }
        }
        private void OpenSaveGame(object obj)
        {
            if (SelectedSaveGame != null)
                _directoryService.OpenSaveGame(SelectedSaveGame);
        }
        private void RenameSavegame(object obj)
        {
            if (SelectedSaveGame == null) return;

            _textDialog.Name = SelectedSaveGame.Name;
            _windowService.OpenWindowDialog(IWindowService.Windows.Textdialog, _textDialog, IWindowService.Windows.MainWindow);

            if (!string.IsNullOrEmpty(_textDialog.Name) && _textDialog.Ok)
                _directoryService.RenameSaveGameFolder(SelectedSaveGame, _textDialog.Name);
                
        }
        private void OpenProfileDialog(object obj)
        {
            _windowService.OpenWindowDialog(IWindowService.Windows.ProfileDialog, _profileDialog, IWindowService.Windows.MainWindow);
            SelectedProfile = _dataService.SelectedProfile;
        }
        private void OpenAboutDialog(object obj)
        {
            _windowService.OpenWindowDialog(IWindowService.Windows.About, _aboutDialog, IWindowService.Windows.MainWindow);
        }
        private void KeyDown(object obj)
        {
            var key = (KeyEventArgs)obj;
            if (key.Key == Key.Delete)
                DeleteSaveGame(null);
        }
    }
}
