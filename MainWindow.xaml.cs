﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using SaveGameManager.Handler;
using SaveGameManager.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SaveGameManager
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private static Random random = new Random();
    private List<Profile> _profiles = new List<Profile>();
    private XmlHandler _xmlHandler;
    private DirectoryHandler _directoryHandler;
    public MainWindow()
    {
      InitializeComponent();
      HandleControls(false);
      _xmlHandler = new XmlHandler(_profiles);
      _xmlHandler.Init();
      _directoryHandler = new DirectoryHandler(_xmlHandler.GameFolder);

      if (!string.IsNullOrEmpty(_xmlHandler.GameFolder))
      {
        SetProfiles();
        HandleControls(true);
      }
    }

    private static string RandomString(int length)
    {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    private static TreeViewItem VisualUpwardSearch(DependencyObject source)
    {
      while (source != null && !(source is TreeViewItem))
        source = VisualTreeHelper.GetParent(source);

      return source as TreeViewItem;
    }
    private void HandleControls(bool enable)
    {
      btnImport.IsEnabled = enable;
      btnLoad.IsEnabled = enable;
      btnReplace.IsEnabled = enable;
    }

    private void SetProfiles()
    {
      var profile = cboProfile.SelectedItem as Profile;
      var profiles = cboProfile.ItemsSource as List<Profile>;

      cboProfile.ItemsSource = _profiles;

      if (_profiles != null && _profiles.Count > 0)
      {
        if (string.IsNullOrEmpty(_xmlHandler.ActiveProfile))
        {
          var first = _profiles.First();
          cboProfile.SelectedItem = first;
          _xmlHandler.ChangeProfile(first.Id);
          _directoryHandler.LoadProfile(first);
          tvSavegame.ItemsSource = first.SaveGames;
        }
        else
        {
          var first = _profiles.Where(p => p.Id == _xmlHandler.ActiveProfile).First();
          if (first != null)
          {
            if (profile == null || profile.Id != _xmlHandler.ActiveProfile)
              cboProfile.SelectedItem = first;
          }
        }
      }

      CollectionViewSource.GetDefaultView(cboProfile.ItemsSource).Refresh();
      CollectionViewSource.GetDefaultView(tvSavegame.ItemsSource).Refresh();
    }
    private void cboProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (cboProfile.SelectedItem == null)
      {
        CollectionViewSource.GetDefaultView(cboProfile.ItemsSource).Refresh();
        tvSavegame.ItemsSource = new List<Savegame>();
        CollectionViewSource.GetDefaultView(tvSavegame.ItemsSource).Refresh();
        return;
      }

      var profile = cboProfile.SelectedItem as Profile;
      _directoryHandler.LoadProfile(profile);
      _xmlHandler.ChangeProfile(profile.Id);

      if (profile != null)
        tvSavegame.ItemsSource = profile.SaveGames;
    }

    private void btnImport_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty(_xmlHandler.GameFolder))
      {
        MessageBox.Show("Select a gamefolder, please");
        return;
      }
      if (cboProfile.SelectedItem == null)
      {
        MessageBox.Show("Select a profile, please");
        return;
      }
      var dialog = new TextDialog($"savegame_{RandomString(8)}");
      if (dialog.ShowDialog() == true)
      {
        var profile = cboProfile.SelectedItem as Profile;
        _directoryHandler.CreateSaveGameFolder(profile, dialog.txtResponse.Text);

        CollectionViewSource.GetDefaultView(tvSavegame.ItemsSource).Refresh();
      }
    }

    private void btnLoad_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty(_xmlHandler.GameFolder))
      {
        MessageBox.Show("Select a gamefolder, please");
        return;
      }
      if (tvSavegame.SelectedItem == null)
      {
        MessageBox.Show("Select a savegame, please");
        return;
      }
      var savegame = tvSavegame.SelectedItem as Savegame;
      _directoryHandler.LoadSaveGame(savegame);

      MessageBox.Show($"Savefile '{savegame.Name}' loaded.");
    }


    private void tvSavegame_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

      if (treeViewItem != null)
      {
        treeViewItem.Focus();
        e.Handled = true;
      }
    }

    private void mtDelete_Click(object sender, RoutedEventArgs e)
    {
      if (tvSavegame.SelectedItem != null)
      {
        var savegame = tvSavegame.SelectedItem as Savegame;

        if (MessageBox.Show($"Do you really want to delete '{savegame.Name}'", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
          return;

        _directoryHandler.DeleteSaveGame(savegame);

        var profile = cboProfile.SelectedItem as Profile;
        profile.SaveGames.Remove(savegame);
        CollectionViewSource.GetDefaultView(tvSavegame.ItemsSource).Refresh();
      }
    }

    private void tvSavegame_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Delete)
      {
        mtDelete_Click(sender, e);
      }
    }

    private void mtRename_Click(object sender, RoutedEventArgs e)
    {
      var savegame = tvSavegame.SelectedItem as Savegame;
      var dialog = new TextDialog(savegame.Name);

      if (dialog.ShowDialog() == true)
      {
        _directoryHandler.RenameSaveGameFolder(savegame, dialog.ResponseText);

        ICollectionView view = CollectionViewSource.GetDefaultView(tvSavegame.ItemsSource);
        view.Refresh();
      }
    }

    private void mtOpenFolder_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (tvSavegame.SelectedItem != null)
        {
          var savegame = tvSavegame.SelectedItem as Savegame;
          if (Directory.Exists(savegame.Path))
            Process.Start("explorer.exe", savegame.Path);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while trying to add a profile to select the gamefolder'.\r\n{ex.Message}");
      }
    }

    private void btnReplace_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (tvSavegame.SelectedItem == null)
        {
          MessageBox.Show("Select a savegame, please");
          return;
        }

        var savegame = tvSavegame.SelectedItem as Savegame;

        if (MessageBox.Show($"Do you really want to replace '{savegame.Name}'", "Replace", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
          return;

        _directoryHandler.ReplaceSavegame(savegame);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Something went wrong, while trying to add a profile to select the gamefolder'.\r\n{ex.Message}");
      }
    }

    private void mtDelete_Loaded(object sender, RoutedEventArgs e)
    {
      if (tvSavegame.SelectedItem == null)
      {
        mtDelete.IsEnabled = false;
        mtOpenFolder.IsEnabled = false;
        mtRename.IsEnabled = false;
      }
      else
      {
        mtDelete.IsEnabled = true;
        mtOpenFolder.IsEnabled = true;
        mtRename.IsEnabled = true;
      }
    }

    private void tvSavegame_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      btnLoad_Click(sender, e);
    }

    private void btSettings_Click(object sender, RoutedEventArgs e)
    {
      cmSetting.StaysOpen = true;
      cmSetting.IsOpen = true;
    }

    private void mtAbout_Click(object sender, RoutedEventArgs e)
    {
      var dialog = new About();
      dialog.ShowDialog();
    }

    private void mtProfiles_Click(object sender, RoutedEventArgs e)
    {
      ProfileDialog profileWindow = new ProfileDialog(_xmlHandler);
      profileWindow.ShowDialog();

      SetProfiles();
      HandleControls(true);
      _directoryHandler.GameFolder = _xmlHandler.GameFolder;
    }
  }
}
